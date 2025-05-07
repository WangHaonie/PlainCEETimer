#include "pch.h"
#include "ListViewHelper.h"
#include "ThemeManager/ThemeManager.h"

/*

使用 WinAPI 高效全选 ListView 所有项 参考：

c# - Setting ListViewItem's Checked state using WinAPI - Stack Overflow
https://stackoverflow.com/a/37146677

c - how to select a line in listview using win32API - Stack Overflow
https://stackoverflow.com/q/22177635

ListView_SetItemState 宏 （commctrl.h） - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/commctrl/nf-commctrl-listview_setitemstate

ListViewItem.cs
https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ListViewItem.cs,753

*/

void FlushHeaderTheme(HWND hLV, COLORREF hFColor, int enable)
{
	HWND hTT = ListView_GetToolTips(hLV);
	LVHForeColor = hFColor;
	
	if (enable)
	{
		SetWindowSubclass(hLV, ListViewNativeWindow, reinterpret_cast<UINT_PTR>(hLV), 0);
		SetTheme(hLV, 0);
		SetTheme(ListView_GetHeader(hLV), 2);
		SetTheme(hTT, 0);
	}
	else
	{
		SetTheme(hLV, 3);
	}

	SetWindowPos(hTT, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
}

void SelectAllItems(HWND hLV, int selected)
{
	ListView_SetItemState(hLV, -1, selected ? LVIS_SELECTED : 0 , LVIS_SELECTED);
}