#pragma once

#include <Windows.h>

struct SystemDisplay
{
    int index;
    LPCWSTR deviceName;
    LPCWSTR deviceId;
    LPCWSTR dosPath;
    RECT bounds;
    double refreshRate;
};

using EnumDisplayProc = BOOL (CALLBACK*)(SystemDisplay info);

cexport(void) EnumSystemDisplays(EnumDisplayProc lpfnEnum);