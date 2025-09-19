#pragma once

#include <Windows.h>

struct LnkInfo
{
    LPCWSTR lnkPath;
    LPCWSTR target;
    LPCWSTR args;
    LPCWSTR workingDir;
    WORD hotkey;
    INT showCmd;
    LPCWSTR description;
    LPCWSTR iconPath;
    INT iconIndex;
};

cexport(void) InitializeShellLink();
cexport(void) ShellLinkCreateLnk(LnkInfo shLnkInfo);
cexport(void) ShellLinkQueryLnk(LnkInfo* lpshLnkInfo);
cexport(void) ReleaseShellLink();