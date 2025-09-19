#pragma once

#include <commdlg.h>

cexport(BOOL) RunColorDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, COLORREF* lpColor, COLORREF* lpCustomColors);
cexport(BOOL) RunFontDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, INT nSizeLimit);