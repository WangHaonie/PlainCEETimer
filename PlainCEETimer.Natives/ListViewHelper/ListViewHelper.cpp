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

void SelectAllItems(HWND hLV, int isSelected)
{
	ListView_SetItemState(hLV, -1, isSelected ? 0 : LVIS_SELECTED, LVIS_SELECTED);
}
