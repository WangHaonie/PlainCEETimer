#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

cexport(void) InitializeTaskbarList(HWND hWnd);
cexport(void) TaskListSetProgressState(TBPFLAG tbpFlags);
cexport(void) TaskListSetProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal);
cexport(void) ReleaseTaskbarList();