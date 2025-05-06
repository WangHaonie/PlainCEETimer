#pragma once

#include <dwmapi.h>

extern "C"
{
	__declspec(dllexport) void __stdcall FlushDarkWindow(HWND hWnd, int type);
}