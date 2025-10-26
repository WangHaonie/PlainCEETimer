// This file contains code from
// https://github.com/stevemk14ebr/PolyHook_2_0/blob/master/sources/IatHook.cpp
// which is licensed under the MIT License. Copyright (c) 2018 Stephen Eckels
// See PolyHook_2_0-LICENSE for more information.

#pragma once

#include <cstdint>

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

inline PIMAGE_THUNK_DATA FindAddressByName(void* moduleBase, PIMAGE_THUNK_DATA impName, PIMAGE_THUNK_DATA impAddr, const char* funcName)
{
    for (; impName->u1.Ordinal; ++impName, ++impAddr)
    {
        if (IMAGE_SNAP_BY_ORDINAL(impName->u1.Ordinal))
            continue;

        auto import = RVA2VA<PIMAGE_IMPORT_BY_NAME>(moduleBase, impName->u1.AddressOfData);
        if (strcmp(import->Name, funcName) != 0)
            continue;
        return impAddr;
    }
    return nullptr;
}

inline PIMAGE_THUNK_DATA FindAddressByOrdinal(void* moduleBase, PIMAGE_THUNK_DATA impName, PIMAGE_THUNK_DATA impAddr, uint16_t ordinal)
{
    for (; impName->u1.Ordinal; ++impName, ++impAddr)
    {
        if (IMAGE_SNAP_BY_ORDINAL(impName->u1.Ordinal) && IMAGE_ORDINAL(impName->u1.Ordinal) == ordinal)
            return impAddr;
    }
    return nullptr;
}

inline PIMAGE_THUNK_DATA FindDelayLoadThunkInModule(void* moduleBase, const char* dllName, const char* funcName, uint16_t ordinal)
{
    auto imports = DataDirectoryFromModuleBase<PIMAGE_DELAYLOAD_DESCRIPTOR>(moduleBase, IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT);
    for (; imports->DllNameRVA; ++imports)
    {
        if (_stricmp(RVA2VA<LPCSTR>(moduleBase, imports->DllNameRVA), dllName) != 0)
            continue;

        auto impName = RVA2VA<PIMAGE_THUNK_DATA>(moduleBase, imports->ImportNameTableRVA);
        auto impAddr = RVA2VA<PIMAGE_THUNK_DATA>(moduleBase, imports->ImportAddressTableRVA);

        if (funcName)
            return FindAddressByName(moduleBase, impName, impAddr, funcName);
        else
            return FindAddressByOrdinal(moduleBase, impName, impAddr, ordinal);
    }
    return nullptr;
}

inline bool __stdcall ReplaceFunction(LPCSTR targetModuleName, const char* importedModuleName, const char* importedFuncName, uint16_t importedFuncOrdinal, void* pNewFunc, void** ppOldFunc)
{
    HMODULE hModule = GetModuleHandleA(targetModuleName);

    if (!hModule)
    {
        hModule = LoadLibraryExA(targetModuleName, nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);
    }

    if (hModule)
    {
        auto* addr = FindDelayLoadThunkInModule(hModule, importedModuleName, importedFuncName, importedFuncOrdinal);

        if (addr)
        {
            DWORD oldProtect = 0;

            if (VirtualProtect(&addr->u1.Function, sizeof(IMAGE_THUNK_DATA), PAGE_READWRITE, &oldProtect))
            {
                if (ppOldFunc)
                {
                    *ppOldFunc = reinterpret_cast<void*>(addr->u1.Function);
                }

                addr->u1.Function = reinterpret_cast<ULONG_PTR>(pNewFunc);
                VirtualProtect(&addr->u1.Function, sizeof(IMAGE_THUNK_DATA), oldProtect, &oldProtect);
                return true;
            }
        }
    }

    return false;
}