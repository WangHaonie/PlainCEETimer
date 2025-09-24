#pragma once

#include <commdlg.h>

cexport(BOOL) RunColorDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, COLORREF* lpColor, COLORREF* lpCustColors);
cexport(BOOL) RunFontDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, INT nSizeLimit);