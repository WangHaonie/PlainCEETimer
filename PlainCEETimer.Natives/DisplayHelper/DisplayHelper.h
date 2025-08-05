#pragma once

#include <Windows.h>

typedef struct tagSYSDISPLAY
{
	int iIndex;
	LPCWSTR pszDeviceName;
	LPCWSTR pszDeviceId;
	LPCWSTR pszDosPath;
	RECT rcDisplay;
	double dRefreshRate;
} SYSDISPLAY;

using EnumDisplayProc = BOOL (CALLBACK*)(SYSDISPLAY info);

cexport(void) EnumSystemDisplays(EnumDisplayProc lpfnEnum);