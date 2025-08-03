#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

typedef struct tagSHLNKINFO
{
    LPCWSTR pszLnkPath;
    LPCWSTR pszFile;
    LPCWSTR pszArgs;
    LPCWSTR pszWorkDir;
    WORD wHotkey;
    int iShowCmd;
    LPCWSTR pszDescr;
    LPCWSTR pszIconPath;
    int iIcon;
} SHLNKINFO, *LPSHLNKINFO;

cexport(void) InitializeShellLink();
cexport(void) ShLkCreateLnk(SHLNKINFO shLnkInfo);
cexport(void) ShLkQueryLnk(LPSHLNKINFO lpshLnkInfo);
cexport(void) ReleaseShellLink();