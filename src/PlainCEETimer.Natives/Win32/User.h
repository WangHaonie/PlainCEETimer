#pragma once

#include <Windows.h>

cexport(LPCWSTR) GetLogonUserName();
cexport(BOOL) RunProcessAsLogonUser(LPCWSTR path, LPCWSTR args, LPDWORD lpExitCode);