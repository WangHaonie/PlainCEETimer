#pragma once

#include <Windows.h>

#define HOOK_OPENNCTHEMEDATA_ARGS "comctl32.dll", "uxtheme.dll", nullptr, 49, true
#define HOOK_GETSYSCOLOR_ARGS "comctl32.dll", "user32.dll", "GetSysColor", 0, false

#define DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 19

cexport(void) EnableDarkModeForApp();
cexport(void) EnableDarkModeForWindowFrame(HWND hWnd, BOOL after20h1);
cexport(void) SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled);
cexport(DWORD) GetSystemAccentColor();
cexport(void) ComctlHookSysColor(COLORREF crFore, COLORREF crBack);
cexport(void) ComctlUnhookSysColor();