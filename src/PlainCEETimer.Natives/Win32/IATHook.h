// This file contains code from
// https://github.com/stevemk14ebr/PolyHook_2_0/blob/master/sources/IatHook.cpp
// which is licensed under the MIT License. Copyright (c) 2018 Stephen Eckels
// See PolyHook_2_0-LICENSE for more information.

#pragma once

#include <cstdint>
#include <utils.h>
#include <Windows.h>

template <typename TFunc>
struct IAT_HOOK_DATA
{
    PIMAGE_THUNK_DATA pThunk = nullptr;
    TFunc OldFunc = nullptr;
    TFunc NewFunc = nullptr;
    bool Initialized = false;
    bool Hooked = false;
};

/*

IAT Hook 参考：

win32-darkmode/win32-darkmode/IatHook.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/IatHook.h

*/

template <typename T, typename T1, typename T2>
constexpr T RVA2VA(T1 base, T2 rva)
{
    return reinterpret_cast<T>(reinterpret_cast<ULONG_PTR>(base) + rva);
}

template <typename T>
constexpr T DataDirectoryFromModuleBase(void* moduleBase, size_t entryID)
{
    auto dosHdr = reinterpret_cast<PIMAGE_DOS_HEADER>(moduleBase);
    auto ntHdr = RVA2VA<PIMAGE_NT_HEADERS>(moduleBase, dosHdr->e_lfanew);
    auto dataDir = ntHdr->OptionalHeader.DataDirectory;
    return RVA2VA<T>(moduleBase, dataDir[entryID].VirtualAddress);
}

inline PIMAGE_THUNK_DATA FindAddress(void* moduleBase, PIMAGE_THUNK_DATA impName, PIMAGE_THUNK_DATA impAddr, const char* funcName, uint16_t ordinal)
{
    bool hasName = !String_IsNullOrEmpty(funcName);

    for (; impName->u1.Ordinal; ++impName, ++impAddr)
    {
        if (IMAGE_SNAP_BY_ORDINAL(impName->u1.Ordinal))
        {
            if (!hasName
                && IMAGE_ORDINAL(impName->u1.Ordinal) == ordinal)
            {
                return impAddr;
            }
        }
        else
        {
            if (hasName
                && String_Equals(RVA2VA<PIMAGE_IMPORT_BY_NAME>(moduleBase, impName->u1.AddressOfData)->Name, funcName, true))
            {
                return impAddr;
            }
        }
    }

    return nullptr;
}

inline PIMAGE_THUNK_DATA FindIatThunkInModule(void* moduleBase, const char* dllName, const char* funcName, uint16_t ordinal)
{
    auto imports = DataDirectoryFromModuleBase<PIMAGE_IMPORT_DESCRIPTOR>(moduleBase, IMAGE_DIRECTORY_ENTRY_IMPORT);

    for (; imports->Name; ++imports)
    {
        if (String_Equals(RVA2VA<LPCSTR>(moduleBase, imports->Name), dllName, true))
        {
            return FindAddress(moduleBase,
                RVA2VA<PIMAGE_THUNK_DATA>(moduleBase, imports->OriginalFirstThunk),
                RVA2VA<PIMAGE_THUNK_DATA>(moduleBase, imports->FirstThunk),
                funcName, ordinal);
        }
    }

    return nullptr;
}

inline PIMAGE_THUNK_DATA FindDelayLoadThunkInModule(void* moduleBase, const char* dllName, const char* funcName, uint16_t ordinal)
{
    auto imports = DataDirectoryFromModuleBase<PIMAGE_DELAYLOAD_DESCRIPTOR>(moduleBase, IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT);

    for (; imports->DllNameRVA; ++imports)
    {
        if (String_Equals(RVA2VA<LPCSTR>(moduleBase, imports->DllNameRVA), dllName, true))
        {
            return FindAddress(moduleBase,
                RVA2VA<PIMAGE_THUNK_DATA>(moduleBase, imports->ImportNameTableRVA),
                RVA2VA<PIMAGE_THUNK_DATA>(moduleBase, imports->ImportAddressTableRVA),
                funcName, ordinal);
        }
    }

    return nullptr;
}

template <typename TFunc>
inline bool __stdcall ReplaceFunctionCore(PIMAGE_THUNK_DATA addr, TFunc pNewFunc)
{
    if (addr)
    {
        DWORD oldProtect = 0;

        if (VirtualProtect(&addr->u1.Function, sizeof(void*), PAGE_READWRITE, &oldProtect))
        {
            addr->u1.Function = reinterpret_cast<ULONGLONG>(pNewFunc);
            VirtualProtect(&addr->u1.Function, sizeof(void*), oldProtect, &oldProtect);
            return true;
        }
    }

    return false;
}

template <typename TFunc>
inline bool __stdcall InitializeIatHook(
    LPCSTR targetModuleName,
    const char* importedModuleName,
    const char* importedFuncName,
    uint16_t importedFuncOrdinal,
    bool fDelayImport,
    IAT_HOOK_DATA<TFunc>& data)
{
    if (data.Initialized)
    {
        return true;
    }

    HMODULE hMod = GetModuleHandleA(targetModuleName);

    if (!hMod)
    {
        hMod = LoadLibraryExA(targetModuleName, nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);
    }

    if (!hMod)
    {
        return false;
    }

    auto addr = fDelayImport
        ? FindDelayLoadThunkInModule(hMod, importedModuleName, importedFuncName, importedFuncOrdinal)
        : FindIatThunkInModule(hMod, importedModuleName, importedFuncName, importedFuncOrdinal);

    if (!addr)
    {
        return false;
    }

    data.pThunk = addr;
    data.OldFunc = reinterpret_cast<TFunc>(addr->u1.Function);
    data.Initialized = true;
    return true;
}

template <typename TFunc>
inline bool __stdcall ReplaceFunction(IAT_HOOK_DATA<TFunc>& data, TFunc newFunc)
{
    if (data.Hooked)
    {
        return true;
    }

    if (!data.Initialized || !data.pThunk)
    {
        return false;
    }

    if (ReplaceFunctionCore(data.pThunk, newFunc))
    {
        data.NewFunc = newFunc;
        data.Hooked = true;
        return true;
    }

    return false;
}

template <typename TFunc>
inline bool __stdcall RestoreFunction(IAT_HOOK_DATA<TFunc>& data)
{
    if (!data.Hooked)
    {
        return true;
    }

    if (!data.Initialized || !data.pThunk)
    {
        return false;
    }

    if (ReplaceFunctionCore(data.pThunk, data.OldFunc))
    {
        data.Hooked = false;
        return true;
    }

    return false;
}