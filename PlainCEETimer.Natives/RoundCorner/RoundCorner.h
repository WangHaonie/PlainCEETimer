#pragma once

#include <dwmapi.h>

cexport(void) SetRoundCornerRegion(HWND hWnd, int wndWidth, int wndHeight, int radius);
cexport(void) SetRoundCornerModern(HWND hWnd);