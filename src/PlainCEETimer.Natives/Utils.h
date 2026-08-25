#pragma once

#include <combaseapi.h>
#include <wchar.h>
#include <Windows.h>
#include <strsafe.h>

#define HEAPALLOC(type)             CastToP(type*, HeapAllocEx(sizeof(type)))
#define HEAPALLOC_M(type, count)    CastToP(type*, HeapAllocEx(count * sizeof(type)))
#define HEAPFREE(lpMem)             HeapFreeEx(CastToP(LPVOID*, &lpMem))

#define CLEARMEM(lpMem)             ZeroMemory(lpMem, sizeof(*lpMem))

#define CASE(val, ret)              case val: return ret
#define CASE_A(val, l, r, ret)      case val: l = r; return ret
#define CASE_AB(val, l, r)          case val: l = r; break

#define String_IsNullOrEmpty(str)   (!str || !*str)
#define WString_IsNullOrEmpty(str)  String_IsNullOrEmpty(str)

#define RECT_IS_ZEROCX(rc)          ((rc.right - rc.left) == 0)

#define INITFUNC(p, mn, pn)         (p || (!p && LoadFunction(&p, mn, pn)))

inline void DDump(const wchar_t* format, ...)
{
#ifdef _DEBUG
    constexpr int count = 512;
    static WCHAR buffer[count];
    va_list args;
    va_start(args, format);

    LPWSTR current = nullptr;
    size_t remain = 0;

    if (SUCCEEDED(StringCchVPrintfExW(buffer, count, &current, &remain, STRSAFE_DEFAULT, format, args)))
    {
        if (remain >= 2)
        {
            current[0] = L'\n';
            current[1] = L'\0';
        }
        else
        {
            buffer[count - 2] = L'\n';
            buffer[count - 1] = L'\0';
        }

        OutputDebugStringW(buffer);
    }
#endif
}

inline bool __cdecl String_Equals(const char* strA, const char* strB, bool bIgnoreCase)
{
    if (!strA || !strB) return false;
    if (strA == strB) return true;
    if (bIgnoreCase) return _stricmp(strA, strB) == 0;
    return strcmp(strA, strB) == 0;
}

inline bool __cdecl WString_StartsWith(const wchar_t* strA, const wchar_t* strB)
{
    if (!strA || !strB) return false;
    if (strA == strB) return true;
    return _wcsnicmp(strA, strB, wcslen(strB)) == 0;
}

inline bool __cdecl WString_Equals(const wchar_t* strA, const wchar_t* strB, bool bIgnoreCase)
{
    if (!strA || !strB) return false;
    if (strA == strB) return true;
    if (bIgnoreCase) return _wcsicmp(strA, strB) == 0;
    return wcscmp(strA, strB) == 0;
}

inline LONGLONG __cdecl RoundToNearestS(LONGLONG value, LONGLONG base)
{
    LONGLONG remain = value % base;
    if (remain == 0) return value;

    if (remain >= base / 2)
        return value + (base - remain);
    else
        return value - remain;
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

inline HMODULE __stdcall PnGetModuleHandleA(LPCSTR lpModuleName, DWORD dwFlags)
{
    HMODULE hmod = GetModuleHandleA(lpModuleName);
    if (!hmod) hmod = LoadLibraryExA(lpModuleName, nullptr, dwFlags);
    return hmod;
}

template<typename TFunc>
inline bool __stdcall LoadFunction(TFunc* lpProc, LPCSTR lpModuleName, LPCSTR lpProcName)
{
    if (lpProc)
    {
        if (*lpProc)
        {
            return true;
        }

        if (lpProcName && !String_IsNullOrEmpty(lpModuleName))
        {
            HMODULE hmod = PnGetModuleHandleA(lpModuleName, LOAD_LIBRARY_SEARCH_SYSTEM32);

            if (hmod)
            {
                FARPROC addr = GetProcAddress(hmod, lpProcName);

                if (addr)
                {
                    *lpProc = CastToP(TFunc, addr);
                    return true;
                }
            }
        }
    }

    return false;
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

inline int __stdcall LoadStringExW(HINSTANCE hInstance, UINT uID, LPWSTR* ppBuffer)
{
    if (ppBuffer)
    {
        return LoadString(hInstance, uID, CastToP(LPWSTR, ppBuffer), 0);
    }

    return 0;
}

inline void __stdcall SetDlgItemTextFromResW(HWND hDlg, int nIDDlgItem, LPWSTR lpResString, int cch)
{
    if (hDlg && cch > 0 && lpResString)
    {
        LPWSTR buffer = CastToP(LPWSTR, _malloca((cch + 1) * sizeof(WCHAR)));

        if (buffer)
        {
            StringCchCopy(buffer, cch, lpResString);
            SetDlgItemText(hDlg, nIDDlgItem, buffer);
        }

        _freea(buffer);
    }
}