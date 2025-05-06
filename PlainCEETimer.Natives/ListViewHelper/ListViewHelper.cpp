#include "pch.h"
#include "ListViewHelper.h"

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

HWND GetHeader(HWND hLV)
{
	return ListView_GetHeader(hLV);
}

HWND GetToolTips(HWND hLV)
{
	HWND hTT = ListView_GetToolTips(hLV);
	SetWindowPos(hTT, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
	return hTT;
}

void FlushHeaderTheme(HWND hLV, int hFColor)
{
	LVHForeColor = (COLORREF)hFColor;
	SetWindowSubclass(hLV, ListViewNativeWindow, reinterpret_cast<UINT_PTR>(hLV), 0);
}

void SelectAllItems(HWND hLV, int selected)
{
	ListView_SetItemState(hLV, -1, selected == 0 ? 0 : LVIS_SELECTED, LVIS_SELECTED);
}