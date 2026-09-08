#pragma once

#include <Windows.h>

#define HOOK_COMDLG32_MESSAGEBOXW_ARGS			COMDLG32_DLL, USER32_DLL, nameof(MessageBoxW), 0, false

#define HMBF_GETMSGBOX							0
#define HMBF_REPMSGBOX							1

DeclDelegateType(MessageBoxW);

NATIVES_EXPORT void NATIVESAPI PnListViewSelectAllItems(HWND hLV, BOOL selected);
NATIVES_EXPORT void NATIVESAPI PnSetTopMostWindow(HWND hWnd);
NATIVES_EXPORT BOOL NATIVESAPI PnMenuGetItemCheckState(HMENU hMenu, UINT item, BOOL bByPos);
NATIVES_EXPORT BOOL NATIVESAPI PnMenuUncheckItem(HMENU hMenu, UINT item, BOOL bByPos);
NATIVES_EXPORT LPCWSTR NATIVESAPI PnGetWindowText(HWND hWnd);
NATIVES_EXPORT void NATIVESAPI PnRemoveWindowExStyle(HWND hWnd, LONG_PTR dwExStyle);
NATIVES_EXPORT BOOL NATIVESAPI PnCheckWindowExStyle(HWND hWnd, LONG_PTR dwExStyle);
NATIVES_EXPORT void NATIVESAPI PnHookMessageBox(HOOKPROC lpfnCbtProc, fnMessageBoxW lpfnMessageBoxW, DWORD dwHookFlag);
NATIVES_EXPORT void NATIVESAPI PnUnhookMessageBox();
NATIVES_EXPORT BOOL NATIVESAPI PnIsDialog(LPCREATESTRUCT lpCreateStruct);
NATIVES_EXPORT void NATIVESAPI PnRemoveWindowIcon(HWND hWnd);
NATIVES_EXPORT void NATIVESAPI PnHookGetMessage(HOOKPROC lpfnGetMsgProc, DWORD dwThreadId);
NATIVES_EXPORT void NATIVESAPI PnUnhookGetMessage();
NATIVES_EXPORT int NATIVESAPI CDCCM_WmContextMenu(HWND hWnd, WPARAM wParam, LPARAM lParam);
