#ifndef PCH_H
#define PCH_H

#include "framework.h"
#include "resource.h"
#include <sdkddkver.h>
#include <combaseapi.h>

#define cexport(ret) extern "C" __declspec(dllexport) ret WINAPI

inline bool __cdecl IsStringNullOrEmptyW(const wchar_t* str) noexcept
{
    return !str || !*str;
}

inline bool __cdecl StringStartsWithW(const wchar_t* strA, const wchar_t* strB)
{
    return _wcsnicmp(strA, strB, wcslen(strB)) == 0;
}

template<typename TInterface>
inline void __stdcall ReleasePPI(TInterface** ppi)
{
    if (ppi && *ppi)
    {
        (*ppi)->Release();
        *ppi = nullptr;
    }
}

/*

关于字符串内存分配：

由于本 DLL 主要用于 C# Interop，在传出字符串时，会调用 CoTaskMemAlloc 分配新的内存来储存字符串以便 CLR 进行 Marshal。
因此不用担心未调用 CoTaskMemFree，因为这将由 CLR 自动调用 (前提是签名必须是 string 而不是 IntPtr)：

Default Marshalling Behavior - .NET Framework | Microsoft Learn
https://learn.microsoft.com/en-us/dotnet/framework/interop/default-marshalling-behavior#unmanaged-signature

"CLR 始终使用 Windows 上的 CoTaskMemFree 方法和其他平台上的 free 方法来释放内存"

*/

inline wchar_t* __stdcall CoTaskStrAllocW(const wchar_t* str)
{
    auto size = (wcslen(str) + 1) * sizeof(wchar_t);
    auto ptr = (wchar_t*)CoTaskMemAlloc(size);

    if (ptr)
    {
        memcpy(ptr, str, size);
    }

    return ptr;
}

#endif 