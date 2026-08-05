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

NATIVES_EXPORT void NATIVESAPI InitializeShellLink();
NATIVES_EXPORT void NATIVESAPI ShellLinkCreateLnk(LPLNKFILEINFO lpLnkFileInfo);
NATIVES_EXPORT void NATIVESAPI ShellLinkQueryLnk(LPLNKFILEINFO lpLnkFileInfo);
NATIVES_EXPORT void NATIVESAPI ReleaseShellLink();