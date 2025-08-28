#pragma once

#include <commdlg.h>

cexport(BOOL) RunColorDialog(HWND hWndOwner, COLORREF* lpColor, LPFRHOOKPROC lpfnHookProc, COLORREF* lpCustomColors);
cexport(BOOL) RunFontDialog(HWND hWndOwner, LPLOGFONT lpLogFont, LPFRHOOKPROC lpfnHookProc, int nSizeMin, int nSizeMax);