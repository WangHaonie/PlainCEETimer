#pragma once

#include <commdlg.h>
#include <Windows.h>

NATIVES_EXPORT BOOL NATIVESAPI PnRunColorDialog(HWND hWndOwner, LPCCHOOKPROC lpfnHookProc, LPCOLORREF lpColor, LPCOLORREF lpCustomColors);
NATIVES_EXPORT BOOL NATIVESAPI PnRunFontDialog(HWND hWndOwner, LPCFHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, INT nSizeLimit);
