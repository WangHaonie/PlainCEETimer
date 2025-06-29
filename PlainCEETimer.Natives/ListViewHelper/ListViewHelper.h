#pragma once

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>

cexport(void) FlushListViewTheme(HWND hLV, COLORREF crHeaderFore, int enable);
cexport(void) SelectAllItems(HWND hLV, int isSelected);
cexport(BOOL) HasScrollBar(HWND hWnd, int isVertical);