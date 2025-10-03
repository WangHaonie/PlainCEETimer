#pragma once

#include <dwmapi.h>

#define DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 19

cexport(void) SetWindowFrameTheme(HWND hWnd, BOOL newStyle);
cexport(void) InitializeAppTheme();
cexport(void) SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled);
cexport(DWORD) GetSystemAccentColor();
