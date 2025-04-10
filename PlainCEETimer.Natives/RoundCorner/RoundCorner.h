#pragma once

#include <dwmapi.h>

extern "C"
{
	__declspec(dllexport) void __stdcall SetRoundCornerRegion(HWND hWnd, int wndWidth, int wndHeight, int radius);
	__declspec(dllexport) void __stdcall SetRoundCornerModern(HWND hWnd);
}