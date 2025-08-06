#pragma once

#include <Windows.h>

struct LnkInfo
{
    LPCWSTR lnkPath;
    LPCWSTR target;
    LPCWSTR args;
    LPCWSTR workingDir;
    WORD hotkey;
    int showCmd;
    LPCWSTR description;
    LPCWSTR iconPath;
    int iconIndex;
};

cexport(void) InitializeShellLink();
cexport(void) ShellLinkCreateLnk(LnkInfo shLnkInfo);
cexport(void) ShellLinkQueryLnk(LnkInfo* lpshLnkInfo);
cexport(void) ReleaseShellLink();