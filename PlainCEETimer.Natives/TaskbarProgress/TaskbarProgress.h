#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

extern "C"
{
	__declspec(dllexport) void __stdcall InitilizeTaskbarList(HWND hWnd, int enable);
	__declspec(dllexport) void __stdcall SetTaskbarProgressState(int tbpFlags);
	__declspec(dllexport) void __stdcall SetTaskbarProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal);
	__declspec(dllexport) void __stdcall ReleaseTaskbarList();
}