#pragma once

#include <dwmapi.h>

cexport(void) FlushWindow(HWND hWnd, BOOL newStyle);
cexport(void) FlushApp();
cexport(void) SetWindowBorderColor(HWND hWnd, BOOL enabled, COLORREF color);
