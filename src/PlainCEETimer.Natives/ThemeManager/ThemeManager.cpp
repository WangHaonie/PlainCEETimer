#include "pch.h"
#include "Win32UI/Win32UI.h"
#include "IATHook.h"
#include <dwmapi.h>

/*

SetPreferredAppMode 参考：

win32-darkmode/win32-darkmode/DarkMode.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/DarkMode.h

*/

using fnSetPreferredAppMode = int (WINAPI*)(int preferredAppMode);
using fnOpenNcThemeData = HTHEME (WINAPI*)(HWND hWnd, LPCWSTR pszClassList);
using fnGetSysColor = DWORD (WINAPI*)(int nIndex);
fnSetPreferredAppMode g_SetPreferredAppMode = nullptr;
fnOpenNcThemeData g_OpenNcThemeData = nullptr;
fnGetSysColor g_GetSysColor = nullptr;
static COLORREF g_crFore = 0;
static COLORREF g_crBack = 0;
static bool isHookEnabled = false;

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
    if (wcscmp(pszClassList, L"ScrollBar") == 0)
    {
        hWnd = nullptr;
        pszClassList = L"DarkMode_Explorer::ScrollBar";
    }

    return g_OpenNcThemeData(hWnd, pszClassList);
};

static DWORD WINAPI GetSysColorNew(int nIndex)
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

        default:
        {
            return g_GetSysColor(nIndex);
        }
    }
}

void EnableDarkModeForApp()
{
    if (!g_SetPreferredAppMode)
    {
        HMODULE hUxtheme = LoadLibraryEx(L"uxtheme.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);

        if (hUxtheme)
        {
            auto addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(135));

            if (addr)
            {
                g_SetPreferredAppMode = reinterpret_cast<fnSetPreferredAppMode>(addr);
                g_SetPreferredAppMode(2);
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(49)))
            {
                g_OpenNcThemeData = reinterpret_cast<fnOpenNcThemeData>(addr);
                ReplaceFunction(HOOK_OPENNCTHEMEDATA_ARGS, OpenNcThemeDataNew, nullptr);
            }
        }
    }
}

void CommonHookSysColor(COLORREF crFore, COLORREF crBack)
{
    if (!isHookEnabled)
    {
        void* o = nullptr;

        if (ReplaceFunction(HOOK_GETSYSCOLOR_ARGS, GetSysColorNew, &o))
        {
            g_crFore = crFore;
            g_crBack = crBack;

            if (!g_GetSysColor && o)
            {
                g_GetSysColor = reinterpret_cast<fnGetSysColor>(o);
            }

            isHookEnabled = true;
        }
    }
}

void CommonUnhookSysColor()
{
    if (isHookEnabled)
    {
        ReplaceFunction(HOOK_GETSYSCOLOR_ARGS, g_GetSysColor, nullptr);
        isHookEnabled = false;
    }
}

/*

窗体标题栏深色样式 参考：

c# - WinForms Dark title bar on Windows 10 - Stack Overflow
https://stackoverflow.com/a/62811758

*/

void EnableDarkModeForWindowFrame(HWND hWnd, BOOL after20h1)
{
    if (hWnd)
    {
        int enabled = 1;
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
