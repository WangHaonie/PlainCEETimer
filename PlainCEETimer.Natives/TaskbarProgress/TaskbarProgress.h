#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

cexport void stdcall InitilizeTaskbarList(HWND hWnd, int enable);
cexport void stdcall SetTaskbarProgressState(TBPFLAG tbpFlags);
cexport void stdcall SetTaskbarProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal);
cexport void stdcall ReleaseTaskbarList();