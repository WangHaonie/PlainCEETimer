#ifndef PCH_H
#define PCH_H

#include "framework.h"
#include "resource.h"
#include <sdkddkver.h>

#define cexport(ret) extern "C" __declspec(dllexport) ret WINAPI

inline bool IsPositive(int i) noexcept
{
    return i > 0;
}

inline bool EnsurePositive(int a, int b, int c) noexcept
{
    return IsPositive(a) && IsPositive(b) && IsPositive(c);
}

#endif 