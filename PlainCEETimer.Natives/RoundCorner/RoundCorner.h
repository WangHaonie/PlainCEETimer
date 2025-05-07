#pragma once

#include <dwmapi.h>

cexport void stdcall SetRoundCornerRegion(HWND hWnd, int wndWidth, int wndHeight, int radius);
cexport void stdcall SetRoundCornerModern(HWND hWnd);