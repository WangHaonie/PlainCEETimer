#pragma once

cexport(HWND) AllocConsoleForApp(BOOL bRefresh, PHANDLE phStdIn, PHANDLE phStdOut, PHANDLE phStdErr);
cexport(void) KillProcessTree(DWORD dwProcessId);