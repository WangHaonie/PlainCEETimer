#pragma once

#include <Windows.h>

#define HOOK_OPENNCTHEMEDATA_ARGS "comctl32.dll", "uxtheme.dll", nullptr, 49, true
#define HOOK_GETSYSCOLOR_ARGS "comctl32.dll", "user32.dll", "GetSysColor", 0, false
#define HOOK_OPENTHEMEDATAFORDPI_ARGS "comctl32.dll", "uxtheme.dll", "OpenThemeDataForDpi", 0, true
#define HOOK_GETSYSCOLORBRUSH_ARGS "comdlg32.dll", "user32.dll", "GetSysColorBrush", 0, false

#define DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 19

cexport(void) EnableDarkModeForApp(BOOL enabled);
cexport(void) ComctlHookSysColor(COLORREF crFore, COLORREF crBack);
cexport(void) ComctlUnhookSysColor();
cexport(void) ComctlHookOpenTheme();
cexport(void) ComctlUnhookOpenTheme();
cexport(void) ComdlgHookGetSysColorBrush();
cexport(void) ComdlgUnhookGetSysColorBrush();
cexport(void) EnableDarkModeForWindowFrame(HWND hWnd, BOOL after20h1, BOOL enabled);
cexport(void) SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled);
cexport(DWORD) GetSystemAccentColor();