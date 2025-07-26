#include "pch.h"
#include "ThemeManager.h"
#include "IATHook.h"

/*

SetPreferredAppMode 参考：

win32-darkmode/win32-darkmode/DarkMode.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/DarkMode.h

*/

using fnSetPreferredAppMode = int (WINAPI*)(int preferredAppMode);
using fnOpenNcThemeData = HTHEME (WINAPI*)(HWND hWnd, LPCWSTR pszClassList);
fnSetPreferredAppMode SetPreferredAppMode = nullptr;
fnOpenNcThemeData OpenNcThemeData = nullptr;

/*

将非 Explorer 主题的 ScrollBar 应用深色主题 参考：

win32-darkmode/win32-darkmode/DarkMode.h at cc26549b65b25d6f3168a80238792545bd401271 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/cc26549b65b25d6f3168a80238792545bd401271/win32-darkmode/DarkMode.h#L152


非常感谢 ysc3839 的耐心协助：

【C# WinForms】IATHook causes System.AccessViolationException · Issue #32 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/issues/32

*/

static void FixScrollBar()
{
    HMODULE hComctl = LoadLibraryEx(L"comctl32.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);

    if (hComctl)
    {
        auto* addr = FindDelayLoadThunkInModule(hComctl, "uxtheme.dll", 49);

        if (addr)
        {
            DWORD oldProtect;

            if (VirtualProtect(addr, sizeof(IMAGE_THUNK_DATA), PAGE_READWRITE, &oldProtect))
            {
                auto MyOpenThemeData = [](HWND hWnd, LPCWSTR classList) -> HTHEME
                {
                    if (wcscmp(classList, L"ScrollBar") == 0)
                    {
                        hWnd = nullptr;
                        classList = L"DarkMode_Explorer::ScrollBar";
                    }

                    return OpenNcThemeData(hWnd, classList);
                };

                addr->u1.Function = reinterpret_cast<ULONG_PTR>(static_cast<fnOpenNcThemeData>(MyOpenThemeData));
                VirtualProtect(addr, sizeof(IMAGE_THUNK_DATA), oldProtect, &oldProtect);
            }
        }
    }
}

/*

窗体标题栏深色样式 参考：

c# - WinForms Dark title bar on Windows 10 - Stack Overflow
https://stackoverflow.com/a/62811758

*/

void FlushWindow(HWND hWnd, BOOL newStyle)
{
    int enabled = 1;
    DwmSetWindowAttribute(hWnd, newStyle ? 20 : 19, &enabled, sizeof(enabled));
}

void FlushApp()
{
    if (!SetPreferredAppMode)
    {
        HMODULE hUxtheme = LoadLibraryEx(L"uxtheme.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);

        if (hUxtheme)
        {
            auto addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(135));

            if (addr)
            {
                SetPreferredAppMode = reinterpret_cast<fnSetPreferredAppMode>(addr);
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(49)))
            {
                OpenNcThemeData = reinterpret_cast<fnOpenNcThemeData>(addr);
            }
        }
    }

    if (SetPreferredAppMode)
    {
        SetPreferredAppMode(2);
        FixScrollBar();
    }
}