#pragma once

#include <Windows.h>

#define HOOK_MESSAGEBOXW_ARGS "comdlg32.dll", "user32.dll", "MessageBoxW", 0, IMAGE_DIRECTORY_ENTRY_IMPORT

cexport(void) ListViewSelectAllItems(HWND hLV, BOOL selected);
cexport(void) SetTopMostWindow(HWND hWnd);
cexport(BOOL) MenuGetItemCheckStateByPosition(HMENU hMenu, UINT item);
cexport(BOOL) MenuCheckRadioItemByPosition(HMENU hMenu, UINT item);
cexport(LPCWSTR) GetWindowTextEx(HWND hWnd);
cexport(LPCWSTR) GetWindowClassName(HWND hWnd);
cexport(void) RemoveWindowExStyle(HWND hWnd, LONG_PTR dwExStyle);
cexport(void) ComdlgHookMessageBox(HOOKPROC lpfnCbtHookProc);
cexport(void) ComdlgUnhookMessageBox();
cexport(BOOL) IsDialog(LPCREATESTRUCT lpCreateStruct);