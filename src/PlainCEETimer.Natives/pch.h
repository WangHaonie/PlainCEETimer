#ifndef PCH_H
#define PCH_H

#include "framework.h"
#include "resource.h"
#include <sdkddkver.h>
#include <combaseapi.h>

#define cexport(ret) extern "C" __declspec(dllexport) ret WINAPI

inline bool IsNullOrEmpty(const wchar_t* str) noexcept
{
    return !str || !*str;
}

inline wchar_t* CoTaskStrAlloc(const wchar_t* str)
{
    auto length = wcslen(str);
    auto size = (length + 1) * sizeof(wchar_t);
    auto ptr = (wchar_t*)CoTaskMemAlloc(size);

    if (ptr)
    {
        memcpy(ptr, str, size);
    }

    return ptr;
}

#endif 