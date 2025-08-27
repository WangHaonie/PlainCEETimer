#pragma once

#include <commdlg.h>

cexport(BOOL) RunFontDialog(HWND hWndOwner, LPLOGFONT lpLogFont, LPFRHOOKPROC lpfnHookProc, int nSizeMin, int nSizeMax);