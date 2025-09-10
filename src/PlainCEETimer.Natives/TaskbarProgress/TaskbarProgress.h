#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

cexport(void) InitializeTaskbarList(HWND hWnd);
cexport(void) TaskbarListSetProgressState(TBPFLAG tbpFlags);
cexport(void) TaskbarListSetProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal);
cexport(void) ReleaseTaskbarList();