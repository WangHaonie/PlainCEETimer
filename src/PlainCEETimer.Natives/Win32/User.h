#pragma once

#include <Windows.h>

NATIVES_EXPORT LPCWSTR NATIVESAPI PnGetLogonUserName();
NATIVES_EXPORT BOOL NATIVESAPI PnRunProcessAsLogonUser(LPWSTR path, LPWSTR args, LPDWORD lpExitCode);
