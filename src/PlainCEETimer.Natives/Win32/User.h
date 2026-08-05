#pragma once

#include <Windows.h>

NATIVES_EXPORT LPCWSTR NATIVESAPI GetLogonUserName();
NATIVES_EXPORT BOOL NATIVESAPI RunProcessAsLogonUser(LPWSTR path, LPWSTR args, LPDWORD lpExitCode);