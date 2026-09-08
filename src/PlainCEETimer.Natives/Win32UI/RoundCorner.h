#pragma once

#include <Windows.h>

NATIVES_EXPORT void NATIVESAPI PnSetRoundCorner(HWND hWnd, int width, int height, int radius);
NATIVES_EXPORT void NATIVESAPI PnSetRoundCornerEx(HWND hWnd, BOOL smallCorner);
