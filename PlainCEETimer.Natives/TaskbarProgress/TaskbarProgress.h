#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

cexport(void) InitializeTaskbarList(HWND hWnd, int enable);
cexport(void) SetTaskbarProgressState(TBPFLAG tbpFlags);
cexport(void) SetTaskbarProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal);
cexport(void) ReleaseTaskbarList();