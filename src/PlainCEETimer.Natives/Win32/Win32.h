#pragma once

cexport(HWND) AllocConsoleForApp(BOOL fRefresh, PHANDLE phStdIn, PHANDLE phStdOut, PHANDLE phStdErr);
cexport(void) KillProcessTree(DWORD dwProcessId);