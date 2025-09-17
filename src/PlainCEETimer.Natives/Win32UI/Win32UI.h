#pragma once

#include <Windows.h>

cexport(void) ListViewSelectAllItems(HWND hLV, BOOL selected);
cexport(BOOL) MenuGetItemCheckStateByPosition(HMENU hMenu, int iItemIndex);