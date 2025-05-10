#pragma once

#include <dwmapi.h>

cexport(void) FlushWindow(HWND hWnd, int type);
cexport(void) FlushApp(int preferredAppMode);
cexport(void) SetTheme(HWND hWnd, int type);
