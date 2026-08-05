#pragma once

#include <Windows.h>

#define HOOK_COMCTL32_OPENNCTHEMEDATA_ARGS			"comctl32.dll", "uxtheme.dll", nullptr, 49, true
#define HOOK_COMCTL32_GETSYSCOLOR_ARGS				"comctl32.dll", "user32.dll", "GetSysColor", 0, false
#define HOOK_COMCTL32_OPENTHEMEDATAFORDPI_ARGS		"comctl32.dll", "uxtheme.dll", "OpenThemeDataForDpi", 0, true
#define HOOK_COMDLG32_GETSYSCOLORBRUSH_ARGS			"comdlg32.dll", "user32.dll", "GetSysColorBrush", 0, false

#define DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1	19

NATIVES_EXPORT void NATIVESAPI EnableDarkModeForApp(BOOL enabled);
NATIVES_EXPORT void NATIVESAPI ComctlHookSysColor(COLORREF crFore, COLORREF crBack);
NATIVES_EXPORT void NATIVESAPI ComctlUnhookSysColor();
NATIVES_EXPORT void NATIVESAPI ComctlHookOpenTheme();
NATIVES_EXPORT void NATIVESAPI ComctlUnhookOpenTheme();
NATIVES_EXPORT void NATIVESAPI ComdlgHookGetSysColorBrush();
NATIVES_EXPORT void NATIVESAPI ComdlgUnhookGetSysColorBrush();
NATIVES_EXPORT void NATIVESAPI EnableDarkModeForWindowFrame(HWND hWnd, BOOL after20h1, BOOL enabled);
NATIVES_EXPORT void NATIVESAPI SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled);
NATIVES_EXPORT DWORD NATIVESAPI GetSystemAccentColor();