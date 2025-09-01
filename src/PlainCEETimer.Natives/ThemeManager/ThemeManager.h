#pragma once

#include <dwmapi.h>

cexport(void) SetWindowFrameTheme(HWND hWnd, BOOL newStyle);
cexport(void) InitializeAppTheme();
cexport(void) SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled);
cexport(DWORD) GetSystemAccentColor();
