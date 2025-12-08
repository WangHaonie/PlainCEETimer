#pragma once

#include <Windows.h>

cexport(void) SetRoundCorner(HWND hWnd, int width, int height, int radius);
cexport(void) SetRoundCornerEx(HWND hWnd, BOOL smallCorner);