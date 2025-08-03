#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

cexport(void) InitializeTaskbarList(HWND hWnd);
cexport(void) TLstSetTaskbarProgressState(TBPFLAG tbpFlags);
cexport(void) TLstSetTaskbarProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal);
cexport(void) ReleaseTaskbarList();