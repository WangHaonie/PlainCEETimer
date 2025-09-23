#pragma once

#include <Windows.h>

typedef struct tagLNKFILEINFO
{
    LPCWSTR lnkPath;
    LPCWSTR pszTarget;
    LPCWSTR pszArgs;
    LPCWSTR pszWorkingDir;
    WORD wHotkey;
    INT iShowCmd;
    LPCWSTR pszDescription;
    LPCWSTR pszIconPath;
    INT iIcon;
} LNKFILEINFO, *LPLNKFILEINFO;

cexport(void) InitializeShellLink();
cexport(void) ShellLinkCreateLnk(LNKFILEINFO lnkFileInfo);
cexport(void) ShellLinkQueryLnk(LPLNKFILEINFO lpLnkFileInfo);
cexport(void) ReleaseShellLink();