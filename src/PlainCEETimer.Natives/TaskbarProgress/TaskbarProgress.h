#pragma once

#include <ShObjIdl.h>
#include <Windows.h>

cexport(void) InitializeTaskbarList();
cexport(void) TaskbarListSetProgressState(HWND hWnd, TBPFLAG tbpFlags);
cexport(void) TaskbarListSetProgressValue(HWND hWnd, ULONGLONG ullCompleted, ULONGLONG ullTotal);
cexport(void) ReleaseTaskbarList();