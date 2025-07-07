#pragma once

#include <Windows.h>

typedef void (CALLBACK *EnumDisplayProc)(RECT lprcMonitor, LPCWSTR device, LPCWSTR path, LPCWSTR did);

cexport(void) EnumSystemDisplays(EnumDisplayProc lpfnEnum);