#pragma once

#include <Windows.h>

cexport(void) ListViewSelectAllItems(HWND hLV, BOOL selected);
cexport(void) SetTopMostWindow(HWND hWnd);
cexport(BOOL) MenuGetItemCheckStateByPosition(HMENU hMenu, UINT item);
cexport(BOOL) MenuCheckRadioItemByPosition(HMENU hMenu, UINT item);
cexport(LPCWSTR) GetWindowTextEx(HWND hWnd);