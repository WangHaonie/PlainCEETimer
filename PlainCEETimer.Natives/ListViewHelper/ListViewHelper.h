#pragma once

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>

cexport(void) FlushListViewTheme(HWND hLV, BOOL enabled);
cexport(void) SelectAllItems(HWND hLV, BOOL selected);