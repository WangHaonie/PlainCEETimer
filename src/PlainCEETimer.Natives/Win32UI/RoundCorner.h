#pragma once

#include <Windows.h>

NATIVES_EXPORT void NATIVESAPI SetRoundCorner(HWND hWnd, int width, int height, int radius);
NATIVES_EXPORT void NATIVESAPI SetRoundCornerEx(HWND hWnd, BOOL smallCorner);