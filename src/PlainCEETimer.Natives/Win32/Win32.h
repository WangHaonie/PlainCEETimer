#pragma once

NATIVES_EXPORT HWND NATIVESAPI PnAllocConsole(BOOL bRefresh, PHANDLE phStdIn, PHANDLE phStdOut, PHANDLE phStdErr);
NATIVES_EXPORT void NATIVESAPI PnKillProcessTree(DWORD dwProcessId);
NATIVES_EXPORT int NATIVESAPI PnLoadStringInternal(UINT uID, LPWSTR* ppBuffer);
