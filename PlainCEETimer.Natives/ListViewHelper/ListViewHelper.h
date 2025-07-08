#pragma once

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>

cexport(void) FlushListViewTheme(HWND hLV, COLORREF crHeaderFore, BOOL enabled);
cexport(void) SelectAllItems(HWND hLV, BOOL selected);
cexport(BOOL) HasScrollBar(HWND hWnd, BOOL isVertical);