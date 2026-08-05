#pragma once

#include <commdlg.h>
#include <Windows.h>

NATIVES_EXPORT BOOL NATIVESAPI RunColorDialog(HWND hWndOwner, LPCCHOOKPROC lpfnHookProc, LPCOLORREF lpColor, LPCOLORREF lpCustomColors);
NATIVES_EXPORT BOOL NATIVESAPI RunFontDialog(HWND hWndOwner, LPCFHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, INT nSizeLimit);