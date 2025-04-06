#pragma once

#include <CommCtrl.h>
#include <Windows.h>

extern "C"
{
	__declspec(dllexport) void __stdcall SelectAllItems(HWND hLV, int state);
}