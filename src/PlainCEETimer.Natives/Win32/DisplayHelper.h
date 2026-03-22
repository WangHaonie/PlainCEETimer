#pragma once

#include <Windows.h>

struct SystemDisplay
{
    int index;
    LPCWSTR deviceName;
    LPCWSTR deviceId;
    LPCWSTR dosPath;
    POINTL position;
    LONG width;
    LONG height;
    double refreshRate;
};

using EnumDisplayProc = BOOL(CALLBACK*)(SystemDisplay info);

struct EnumDisplayData
{
    EnumDisplayProc lpfnEnum;
    int index;
};

cexport(BOOL) EnumSystemDisplays(EnumDisplayProc lpfnEnum);