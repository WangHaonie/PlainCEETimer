#pragma once

#include <commdlg.h>

cexport(BOOL) RunColorDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, COLORREF* lpColor, COLORREF* lpCustomColors);
cexport(BOOL) RunFontDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, int nSizeLimit);
cexport(BOOL) GetMenuItemCheckStateByPosition(HMENU hMenu, int iItemIndex);