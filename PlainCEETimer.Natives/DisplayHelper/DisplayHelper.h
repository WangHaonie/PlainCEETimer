#pragma once

#include <Windows.h>

using EnumDisplayProc = BOOL (CALLBACK*)(RECT lprcMonitor, LPCWSTR device, LPCWSTR path, LPCWSTR did);

cexport(void) EnumSystemDisplays(EnumDisplayProc lpfnEnum);