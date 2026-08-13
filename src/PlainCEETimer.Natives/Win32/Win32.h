#pragma once

NATIVES_EXPORT HWND NATIVESAPI AllocConsoleForApp(BOOL bRefresh, PHANDLE phStdIn, PHANDLE phStdOut, PHANDLE phStdErr);
NATIVES_EXPORT void NATIVESAPI KillProcessTree(DWORD dwProcessId);
NATIVES_EXPORT int NATIVESAPI LoadStringInternal(UINT uID, LPWSTR* ppBuffer);