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
cexport(void) ShellLinkCreateLnk(SHLNKINFO shLnkInfo);
cexport(void) ShellLinkExportLnk(LPSHLNKINFO lpshLnkInfo);
cexport(void) ReleaseShellLink();