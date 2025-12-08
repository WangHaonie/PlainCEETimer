#pragma once

#include <Windows.h>

typedef struct tagLNKFILEINFO
{
    LPCWSTR lnkPath;
    LPWSTR pszTarget;
    LPWSTR pszArgs;
    LPWSTR pszWorkingDir;
    WORD wHotkey;
    INT iShowCmd;
    LPWSTR pszDescription;
    LPWSTR pszIconPath;
    INT iIcon;
} LNKFILEINFO, *LPLNKFILEINFO;

cexport(void) InitializeShellLink();
cexport(void) ShellLinkCreateLnk(LNKFILEINFO lnkFileInfo);
cexport(void) ShellLinkQueryLnk(LPLNKFILEINFO lpLnkFileInfo);
cexport(void) ReleaseShellLink();