#pragma once

#include <dwmapi.h>

cexport(void) SetRoundCornerRegion(HWND hWnd, int width, int height, int radius);
cexport(void) SetRoundCornerModern(HWND hWnd, BOOL isSmall);