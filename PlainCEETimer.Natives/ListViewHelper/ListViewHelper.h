#pragma once

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>

cexport void stdcall FlushHeaderTheme(HWND hLV, COLORREF hFColor, int enable);
cexport void stdcall SelectAllItems(HWND hLV, int isSelected);