#pragma once

#include <combaseapi.h>
#include <wchar.h>
#include <Windows.h>
#include <strsafe.h>

#define HEAPALLOC(type)             CastToP(type*, HeapAllocEx(sizeof(type)))
#define HEAPALLOC_M(type, count)    CastToP(type*, HeapAllocEx(count * sizeof(type)))
#define HEAPFREE(lpMem)             HeapFreeEx(CastToP(LPVOID*, &lpMem))

inline bool __cdecl String_IsNullOrEmpty(const char* str) noexcept
{
    return !str || !*str;
}

inline bool __cdecl String_Equals(const char* strA, const char* strB, bool bIgnoreCase)
{
    if (!strA || !strB)
    {
        return false;
    }

    if (strA == strB)
    {
        return true;
    }

    if (bIgnoreCase)
    {
        return _stricmp(strA, strB) == 0;
    }

    return strcmp(strA, strB) == 0;
}

inline bool __cdecl WString_IsNullOrEmpty(const wchar_t* str) noexcept
{
    return !str || !*str;
}

inline bool __cdecl WString_StartsWith(const wchar_t* strA, const wchar_t* strB)
{
    if (!strA || !strB)
    {
        return false;
    }

    if (strA == strB)
    {
        return true;
    }

    return _wcsnicmp(strA, strB, wcslen(strB)) == 0;
}

inline bool __cdecl WString_Equals(const wchar_t* strA, const wchar_t* strB, bool bIgnoreCase)
{
    if (!strA || !strB)
    {
        return false;
    }

    if (strA == strB)
    {
        return true;
    }

    if (bIgnoreCase)
    {
        return _wcsicmp(strA, strB) == 0;
    }

    return wcscmp(strA, strB) == 0;
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

inline LPVOID __stdcall HeapAllocEx(SIZE_T dwBytes)
{
    return HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, dwBytes);
}

inline BOOL __stdcall HeapFreeEx(LPVOID* ppMem)
{
    if (ppMem && *ppMem)
    {
        BOOL result = HeapFree(GetProcessHeap(), 0, *ppMem);
        if (result) *ppMem = nullptr;
        return result;
    }

    return TRUE;
}

/*

关于字符串内存分配：

由于本 DLL 导出函数主要用于 C# Interop，在传出字符串时，会调用 CoTaskMemAlloc 分配新的内存来储存字符串以便 CLR 进行 Marshal。
因此不用担心未调用 CoTaskMemFree，因为这将由 CLR 自动调用 (前提是签名必须是 string 而不是 IntPtr)：

Default Marshalling Behavior - .NET Framework | Microsoft Learn
https://learn.microsoft.com/en-us/dotnet/framework/interop/default-marshalling-behavior#unmanaged-signature

"CLR 始终使用 Windows 上的 CoTaskMemFree 方法和其他平台上的 free 方法来释放内存"

*/

inline LPWSTR __stdcall CoTaskStrAllocW(size_t strLength)
{
    size_t s = strLength * sizeof(WCHAR);
    LPWSTR ptr = CastToP(LPWSTR, CoTaskMemAlloc(s));
    return ptr;
}

inline LPWSTR __stdcall CoTaskStrDupW(LPCWSTR str)
{
    size_t count = lstrlen(str) + 1;
    LPWSTR ptr = CoTaskStrAllocW(count);
    StringCchCopy(ptr, count, str);
    return ptr;
}