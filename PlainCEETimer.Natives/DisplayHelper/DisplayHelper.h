#pragma once

#include <Windows.h>

typedef void (CALLBACK *EnumDisplayProc)(RECT lprcMonitor, LPCWSTR device, LPCWSTR path);

cexport(void) EnumSystemDisplays(EnumDisplayProc lpfnEnum);