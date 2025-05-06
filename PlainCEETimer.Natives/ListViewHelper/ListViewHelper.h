#pragma once

#define LV_INITNOW WM_USER + 13

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>

extern "C"
{
	__declspec(dllexport) void __stdcall SelectAllItems(HWND hLV, int isSelected);
	__declspec(dllexport) HWND __stdcall GetHeader(HWND hLV);
	__declspec(dllexport) void __stdcall FlushHeaderTheme(HWND hLV, int hFColor);
}

/*

ListView 深色主题 参考：

win32-darkmode/win32-darkmode/ListViewUtil.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/ListViewUtil.h

*/

static COLORREF LVHForeColor;

static LRESULT CALLBACK ListViewNativeWindow(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR)
{
	switch (uMsg)
	{
		case WM_NOTIFY:
		{
			if (reinterpret_cast<LPNMHDR>(lParam)->code == NM_CUSTOMDRAW)
			{
				LPNMCUSTOMDRAW nmcd = reinterpret_cast<LPNMCUSTOMDRAW>(lParam);

				switch (nmcd->dwDrawStage)
				{
					case CDDS_PREPAINT:
						return CDRF_NOTIFYITEMDRAW;
					case CDDS_ITEMPREPAINT:
					{
						SetTextColor(nmcd->hdc, LVHForeColor);
						return CDRF_DODEFAULT;
					}
				}
			}
		}
		break;

		case WM_NCDESTROY:
		{
			RemoveWindowSubclass(hWnd, ListViewNativeWindow, uIdSubclass);
		}
		break;
	}

	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}