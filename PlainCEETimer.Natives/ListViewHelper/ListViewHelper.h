#pragma once

#include <CommCtrl.h>
#include <Windows.h>
#include <Uxtheme.h>
#include <vssym32.h>
#include <cstdint>

#define LV_INITNOW WM_USER + 13

extern "C"
{
	__declspec(dllexport) void __stdcall SelectAllItems(HWND hLV, int isSelected);
	__declspec(dllexport) HWND __stdcall GetHeader(HWND hLV);
	__declspec(dllexport) void __stdcall FlushHeaderTheme(HWND hLV, HWND hLVH);
}

/*

ListView 深色主题 参考：

win32-darkmode/win32-darkmode/ListViewUtil.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/ListViewUtil.h

*/

struct ListViewSubClassData
{
	HWND hHeader;
	COLORREF HeaderTextColor;
};

static LRESULT CALLBACK ListViewNativeWindow(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
	auto data = reinterpret_cast<ListViewSubClassData*>(dwRefData);

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
						SetTextColor(nmcd->hdc, data->HeaderTextColor);
						return CDRF_DODEFAULT;
					}
				}
			}
		}
		break;

		case LV_INITNOW:
		{
			HTHEME hTheme = OpenThemeData(nullptr, L"DarkMode_ItemsView");

			if (hTheme)
			{
				COLORREF color;

				if (SUCCEEDED(GetThemeColor(hTheme, 0, 0, TMT_TEXTCOLOR, &color)))
				{
					ListView_SetTextColor(hWnd, color);
				}

				if (SUCCEEDED(GetThemeColor(hTheme, 0, 0, TMT_FILLCOLOR, &color)))
				{
					ListView_SetTextBkColor(hWnd, color);
					ListView_SetBkColor(hWnd, color);
				}

				CloseThemeData(hTheme);
			}

			hTheme = OpenThemeData(data->hHeader, L"Header");

			if (hTheme)
			{
				GetThemeColor(hTheme, HP_HEADERITEM, 0, TMT_TEXTCOLOR, &(data->HeaderTextColor));
				CloseThemeData(hTheme);
			}
		}
		break;

		case WM_NCDESTROY:
		{
			RemoveWindowSubclass(hWnd, ListViewNativeWindow, uIdSubclass);
			delete data;
		}
		break;
	}

	return DefSubclassProc(hWnd, uMsg, wParam, lParam);
}