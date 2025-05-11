#pragma once

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>

cexport(void) FlushHeaderTheme(HWND hLV, COLORREF hFColor);
cexport(void) SelectAllItems(HWND hLV, int isSelected);