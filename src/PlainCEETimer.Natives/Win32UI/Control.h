#pragma once

#include <Windows.h>

#define HOOK_MESSAGEBOXW_ARGS "comdlg32.dll", "user32.dll", "MessageBoxW", 0, false

cexport(void) ListViewSelectAllItems(HWND hLV, BOOL selected);
cexport(void) SetTopMostWindow(HWND hWnd);
cexport(BOOL) MenuGetItemCheckState(HMENU hMenu, UINT item, BOOL fByPosition);
cexport(BOOL) MenuUncheckItem(HMENU hMenu, UINT item, BOOL fByPosition);
cexport(LPCWSTR) GetWindowTextEx(HWND hWnd);
cexport(LPCWSTR) GetWindowClassName(HWND hWnd);
cexport(void) RemoveWindowExStyle(HWND hWnd, LONG_PTR dwExStyle);
cexport(void) ComdlgHookMessageBox(HOOKPROC lpfnCbtHookProc);
cexport(void) ComdlgUnhookMessageBox();
cexport(BOOL) IsDialog(LPCREATESTRUCT lpCreateStruct);