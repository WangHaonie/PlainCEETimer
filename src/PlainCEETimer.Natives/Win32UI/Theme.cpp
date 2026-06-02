#include "pch.h"
#include "Theme.h"
#include "utils.h"
#include "Win32/IATHook.h"
#include <dwmapi.h>
#include <Uxtheme.h>
#include <Windows.h>

/*

Win32 深色模式 API 相关 参考：

win32-darkmode/win32-darkmode/DarkMode.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/DarkMode.h

*/

using fnSetPreferredAppMode = int (WINAPI*)(int preferredAppMode);
using fnOpenNcThemeData = HTHEME (WINAPI*)(HWND hWnd, LPCWSTR pszClassList);
using fnFlushMenuThemes = void (WINAPI*)();
using fnGetSysColor = DWORD (WINAPI*)(int nIndex);

static COLORREF g_crFore = 0;
static COLORREF g_crBack = 0;
static BOOL g_fUseDark = FALSE;

static fnSetPreferredAppMode g_SetPreferredAppMode = nullptr;
static fnOpenNcThemeData g_OpenNcThemeData = nullptr;
static fnFlushMenuThemes g_FlushMenuThemes = nullptr;
static fnGetSysColor g_GetSysColor = nullptr;

static IAT_HOOK_DATA<fnOpenNcThemeData> IatHookOpenNcThemeData = {};
static IAT_HOOK_DATA<fnGetSysColor> IatHookGetSysColor = {};

/*

将非 Explorer 主题的 ScrollBar 应用深色主题 参考：

win32-darkmode/win32-darkmode/DarkMode.h at cc26549b65b25d6f3168a80238792545bd401271 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/cc26549b65b25d6f3168a80238792545bd401271/win32-darkmode/DarkMode.h#L152


非常感谢 ysc3839 的耐心协助：

【C# WinForms】IATHook causes System.AccessViolationException · Issue #32 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/issues/32

*/

static HTHEME WINAPI OpenNcThemeDataNew(HWND hWnd, LPCWSTR pszClassList)
{
    if (g_fUseDark && WString_Equals(pszClassList, WC_SCROLLBAR, false))
    {
        hWnd = nullptr;
        pszClassList = L"DarkMode_Explorer::ScrollBar";
    }

    return g_OpenNcThemeData(hWnd, pszClassList);
};

static DWORD WINAPI GetSysColorNew(int nIndex)
{
    if (g_fUseDark)
    {
        switch (nIndex)
        {
            case COLOR_WINDOW:
            {
                return g_crBack;
            }

            case COLOR_WINDOWTEXT:
            {
                return g_crFore;
            }
        }
    }

    return g_GetSysColor(nIndex);
}

void EnableDarkModeForApp(BOOL enabled)
{
    g_fUseDark = enabled;

    if (!g_SetPreferredAppMode)
    {
        HMODULE hUxtheme = LoadLibraryEx(L"uxtheme.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);

        if (hUxtheme)
        {
            auto addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(135));

            if (addr)
            {
                g_SetPreferredAppMode = reinterpret_cast<fnSetPreferredAppMode>(addr);
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(49)))
            {
                g_OpenNcThemeData = reinterpret_cast<fnOpenNcThemeData>(addr);
                
                if (InitializeIatHook(HOOK_OPENNCTHEMEDATA_ARGS, IatHookOpenNcThemeData))
                {
                    ReplaceFunction(IatHookOpenNcThemeData, OpenNcThemeDataNew);
                    IatHookOpenNcThemeData.OldFunc = g_OpenNcThemeData;
                }
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(136)))
            {
                g_FlushMenuThemes = reinterpret_cast<fnFlushMenuThemes>(addr);
            }
        }
    }

    if (g_SetPreferredAppMode) g_SetPreferredAppMode(enabled ? 2 : 0);
    if (g_FlushMenuThemes) g_FlushMenuThemes();
}

void ComctlHookSysColor(COLORREF crFore, COLORREF crBack)
{
    if (!InitializeIatHook(HOOK_GETSYSCOLOR_ARGS, IatHookGetSysColor))
    {
        return;
    }

    if (!g_GetSysColor)
    {
        g_GetSysColor = IatHookGetSysColor.OldFunc;
    }

    if (ReplaceFunction(IatHookGetSysColor, GetSysColorNew))
    {
        g_crFore = crFore;
        g_crBack = crBack;
    }
}

void ComctlUnhookSysColor()
{
    RestoreFunction(IatHookGetSysColor);
    g_crFore = 0;
    g_crBack = 0;
}

/*

窗体标题栏深色样式 参考：

c# - WinForms Dark title bar on Windows 10 - Stack Overflow
https://stackoverflow.com/a/62811758

*/

void EnableDarkModeForWindowFrame(HWND hWnd, BOOL after20h1, BOOL enabled)
{
    if (hWnd)
    {
        DwmSetWindowAttribute(hWnd, after20h1 ? DWMWA_USE_IMMERSIVE_DARK_MODE : DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, &enabled, sizeof(enabled));
    }
}

void SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled)
{
    if (hWnd)
    {
        COLORREF c = enabled ? color : DWMWA_COLOR_DEFAULT;
        DwmSetWindowAttribute(hWnd, DWMWA_BORDER_COLOR, &c, sizeof(c));
    }
}

DWORD GetSystemAccentColor()
{
    DWORD result = 0;
    BOOL flag = FALSE;
    DwmGetColorizationColor(&result, &flag);
    return result;
}
