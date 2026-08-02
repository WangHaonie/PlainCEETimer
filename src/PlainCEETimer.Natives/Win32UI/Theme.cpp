#include "pch.h"
#include "Control.h"
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
using fnOpenNcThemeData = decltype(&OpenThemeData);
using fnFlushMenuThemes = void (WINAPI*)();
DeclDelegateType(OpenThemeDataForDpi);
DeclDelegateType(GetSysColor);
DeclDelegateType(GetSysColorBrush);

static COLORREF g_crFore = 0;
static COLORREF g_crBack = 0;
static BOOL g_fUseDark = FALSE;

DeclDelegateField(SetPreferredAppMode);
DeclDelegateField(OpenNcThemeData);
DeclDelegateField(OpenThemeDataForDpi);
DeclDelegateField(FlushMenuThemes);
DeclDelegateField(GetSysColor);
DeclDelegateField(GetSysColorBrush);

DeclIatData(OpenNcThemeData, Comctl);
DeclIatData(OpenThemeDataForDpi, Comctl);
DeclIatData(GetSysColor, Comctl);
DeclIatData(GetSysColorBrush, Comdlg);

/*

将非 Explorer 主题的 ScrollBar 应用深色主题 参考：

win32-darkmode/win32-darkmode/DarkMode.h at cc26549b65b25d6f3168a80238792545bd401271 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/cc26549b65b25d6f3168a80238792545bd401271/win32-darkmode/DarkMode.h#L152


非常感谢 ysc3839 的耐心协助：

【C# WinForms】IATHook causes System.AccessViolationException · Issue #32 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/issues/32

*/

static HTHEME WINAPI OpenNcThemeData_(HWND hWnd, LPCWSTR pszClassList)
{
    if (g_fUseDark && WString_Equals(pszClassList, WC_SCROLLBAR, true))
    {
        hWnd = nullptr;
        pszClassList = L"DarkMode_Explorer::ScrollBar";
    }

    return g_OpenNcThemeData(hWnd, pszClassList);
};

static void HandleListViewCheckBoxes(HWND& hWnd, LPCWSTR& pszClassList)
{
    if (WString_Equals(pszClassList, WC_BUTTON, true) && !hWnd)
    {
        pszClassList = L"DarkMode_Explorer::Button";
    }
}

static void HandleColorDlgLumArrow(int& nIndex)
{
    if (nIndex == COLOR_BTNTEXT)
    {
        nIndex = COLOR_WINDOW;
    }
}

static HTHEME WINAPI OpenThemeDataForDpi_(HWND hWnd, LPCWSTR pszClassList, UINT dpi)
{
    HandleListViewCheckBoxes(hWnd, pszClassList);
    return g_OpenThemeDataForDpi(hWnd, pszClassList, dpi);
};

static DWORD WINAPI GetSysColor_(int nIndex)
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

static HBRUSH WINAPI GetSysColorBrush_(int nIndex)
{
    HandleColorDlgLumArrow(nIndex);
    return g_GetSysColorBrush(nIndex);
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
                g_SetPreferredAppMode = CastToP(fnSetPreferredAppMode, addr);
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(49)))
            {
                g_OpenNcThemeData = CastToP(fnOpenNcThemeData, addr);
                
                if (InitializeIatHook(HOOK_COMCTL32_OPENNCTHEMEDATA_ARGS, IatHookComctlOpenNcThemeData))
                {
                    ReplaceFunction(IatHookComctlOpenNcThemeData, OpenNcThemeData_);
                    IatHookComctlOpenNcThemeData.OldFunc = g_OpenNcThemeData;
                }
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(136)))
            {
                g_FlushMenuThemes = CastToP(fnFlushMenuThemes, addr);
            }
        }
    }

    if (g_SetPreferredAppMode) g_SetPreferredAppMode(enabled ? 2 : 0);
    if (g_FlushMenuThemes) g_FlushMenuThemes();
}

void ComctlHookSysColor(COLORREF crFore, COLORREF crBack)
{
    if (!InitializeIatHook(HOOK_COMCTL32_GETSYSCOLOR_ARGS, IatHookComctlGetSysColor))
    {
        return;
    }

    if (!g_GetSysColor)
    {
        g_GetSysColor = IatHookComctlGetSysColor.OldFunc;
    }

    if (ReplaceFunction(IatHookComctlGetSysColor, GetSysColor_))
    {
        g_crFore = crFore;
        g_crBack = crBack;
    }
}

void ComctlUnhookSysColor()
{
    UnhookIat(IatHookComctlGetSysColor);
    g_crFore = 0;
    g_crBack = 0;
}

void ComctlHookOpenTheme()
{
    HookIat(HOOK_COMCTL32_OPENTHEMEDATAFORDPI_ARGS,
        IatHookComctlOpenThemeDataForDpi,
        g_OpenThemeDataForDpi,
        OpenThemeDataForDpi_
    );
}

void ComctlUnhookOpenTheme()
{
    UnhookIat(IatHookComctlOpenThemeDataForDpi);
}

void ComdlgHookGetSysColorBrush()
{
    HookIat(HOOK_COMDLG32_GETSYSCOLORBRUSH_ARGS,
        IatHookComdlgGetSysColorBrush,
        g_GetSysColorBrush,
        GetSysColorBrush_
    );
}

void ComdlgUnhookGetSysColorBrush()
{
    UnhookIat(IatHookComdlgGetSysColorBrush);
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
        DwmSetWindowAttribute(hWnd, after20h1
            ? DWMWA_USE_IMMERSIVE_DARK_MODE
            : DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1,
            &enabled, sizeof(enabled));
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
