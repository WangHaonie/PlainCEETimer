#pragma once

#include <ShObjIdl_core.h>
#include <Windows.h>

NATIVES_EXPORT void NATIVESAPI InitializeTaskbarList();
NATIVES_EXPORT void NATIVESAPI TaskbarListSetProgressState(HWND hWnd, TBPFLAG tbpFlags);
NATIVES_EXPORT void NATIVESAPI TaskbarListSetProgressValue(HWND hWnd, ULONGLONG ullCompleted, ULONGLONG ullTotal);
NATIVES_EXPORT void NATIVESAPI ReleaseTaskbarList();