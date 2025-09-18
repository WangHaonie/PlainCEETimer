#pragma once

#include <WtsApi32.h>

cexport(LPCWSTR) GetLogonUserName();
cexport(BOOL) RunProcessAsLogonUser(LPCWSTR cli, LPDWORD lpExitCode);