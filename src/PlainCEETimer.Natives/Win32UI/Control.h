#pragma once

#include <Windows.h>

#define HOOK_COMDLG32_MESSAGEBOXW_ARGS			"comdlg32.dll", "user32.dll", "MessageBoxW", 0, false

#define HMBF_GETMSGBOX							0
#define HMBF_REPMSGBOX							1

DeclDelegateType(MessageBoxW);

NATIVES_EXPORT void NATIVESAPI ListViewSelectAllItems(HWND hLV, BOOL selected);
NATIVES_EXPORT void NATIVESAPI SetTopMostWindow(HWND hWnd);
NATIVES_EXPORT BOOL NATIVESAPI MenuGetItemCheckState(HMENU hMenu, UINT item, BOOL fByPosition);
NATIVES_EXPORT BOOL NATIVESAPI MenuUncheckItem(HMENU hMenu, UINT item, BOOL fByPosition);
NATIVES_EXPORT LPCWSTR NATIVESAPI GetWindowTextEx(HWND hWnd);
NATIVES_EXPORT LPCWSTR NATIVESAPI GetWindowClassName(HWND hWnd);
NATIVES_EXPORT void NATIVESAPI RemoveWindowExStyle(HWND hWnd, LONG_PTR dwExStyle);
NATIVES_EXPORT BOOL NATIVESAPI CheckWindowExStyle(HWND hWnd, LONG_PTR dwExStyle);
NATIVES_EXPORT void NATIVESAPI ComdlgHookMessageBox(HOOKPROC lpfnCbtProc, fnMessageBoxW lpfnMessageBoxW, DWORD dwHookFlag);
NATIVES_EXPORT void NATIVESAPI ComdlgUnhookMessageBox();
NATIVES_EXPORT BOOL NATIVESAPI IsDialog(LPCREATESTRUCT lpCreateStruct);
NATIVES_EXPORT void NATIVESAPI RemoveWindowIcon(HWND hWnd);
NATIVES_EXPORT void NATIVESAPI HookGetMessage(HOOKPROC lpfnGetMsgProc, DWORD dwThreadId);
NATIVES_EXPORT void NATIVESAPI UnhookGetMessage();
