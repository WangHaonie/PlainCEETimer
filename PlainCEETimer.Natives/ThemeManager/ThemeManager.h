#pragma once

#include <dwmapi.h>

cexport(void) FlushWindow(HWND hWnd, BOOL newStyle);
cexport(void) FlushApp();
