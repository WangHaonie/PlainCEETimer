#pragma once

#include <Windows.h>
#include <commdlg.h>
#include <dwmapi.h>

#define HOOK_OPENNCTHEMEDATA_ARGS "comctl32.dll", "uxtheme.dll", nullptr, 49, IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT
#define HOOK_GETSYSCOLOR_ARGS "comctl32.dll", "user32.dll", "GetSysColor", 0, IMAGE_DIRECTORY_ENTRY_IMPORT
#define HOOK_MESSAGEBOXW_ARGS "comdlg32.dll", "user32.dll", "MessageBoxW", 0, IMAGE_DIRECTORY_ENTRY_IMPORT

#define DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 19

// Common Dialogs 相关

cexport(BOOL) RunColorDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, LPCOLORREF lpColor, LPCOLORREF lpCustomColors);
cexport(BOOL) RunFontDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, INT nSizeLimit);

// Win32 主题样式相关

cexport(void) SetRoundCorner(HWND hWnd, int width, int height, int radius);
cexport(void) SetRoundCornerEx(HWND hWnd, BOOL smallCorner);
cexport(void) EnableDarkModeForApp();
cexport(void) EnableDarkModeForWindowFrame(HWND hWnd, BOOL after20h1);
cexport(void) SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled);
cexport(DWORD) GetSystemAccentColor();
cexport(void) ComctlHookSysColor(COLORREF crFore, COLORREF crBack);
cexport(void) ComctlUnhookSysColor();

// Win32 Control 相关

cexport(void) ListViewSelectAllItems(HWND hLV, BOOL selected);
cexport(void) SetTopMostWindow(HWND hWnd);
cexport(BOOL) MenuGetItemCheckStateByPosition(HMENU hMenu, UINT item);
cexport(BOOL) MenuCheckRadioItemByPosition(HMENU hMenu, UINT item);
cexport(LPCWSTR) GetWindowTextEx(HWND hWnd);
cexport(LPCWSTR) GetWindowClassName(HWND hWnd);
cexport(void) RemoveWindowExStyle(HWND hWnd, LONG_PTR dwExStyle);
cexport(void) ComdlgHookMessageBox(HOOKPROC lpfnCbtHookProc);
cexport(void) ComdlgUnhookMessageBox();
cexport(BOOL) IsMessageBox(LPCREATESTRUCT lpCreateStruct);