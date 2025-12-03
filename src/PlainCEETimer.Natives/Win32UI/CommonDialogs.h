#pragma once

#include <Windows.h>
#include <commdlg.h>

cexport(BOOL) RunColorDialog(HWND hWndOwner, LPCCHOOKPROC lpfnHookProc, LPCOLORREF lpColor, LPCOLORREF lpCustomColors);
cexport(BOOL) RunFontDialog(HWND hWndOwner, LPCFHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, INT nSizeLimit);