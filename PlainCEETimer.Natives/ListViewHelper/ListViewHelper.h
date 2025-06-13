#pragma once

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>

cexport(void) FlushListViewTheme(HWND hLV, COLORREF colorHFore, int enable);
cexport(void) SelectAllItems(HWND hLV, int isSelected);