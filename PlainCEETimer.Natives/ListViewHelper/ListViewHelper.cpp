#include "pch.h"
#include "ListViewHelper.h"
#include "ThemeManager/ThemeManager.h"

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
                        SetTextColor(nmcd->hdc, LVHForeColor);
                        return CDRF_DODEFAULT;
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

void SelectAllItems(HWND hLV, BOOL selected)
{
    ListView_SetItemState(hLV, -1, selected ? LVIS_SELECTED : 0 , LVIS_SELECTED);
}

void FlushListViewTheme(HWND hLV, COLORREF crHeaderFore, BOOL enabled)
{
    HWND hTT = ListView_GetToolTips(hLV);

    if (enabled)
    {
        LVHForeColor = crHeaderFore;
        SetWindowSubclass(hLV, ListViewNativeWindow, reinterpret_cast<UINT_PTR>(hLV), 0);
        SetTheme(hLV, 2);
        SetTheme(ListView_GetHeader(hLV), 2);
        SetTheme(hTT, 0);
    }
    else
    {
        SetTheme(hLV, 3);
    }

    SetWindowPos(hTT, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
}

BOOL HasScrollBar(HWND hWnd, BOOL isVertical)
{
    return ((GetWindowLongPtr(hWnd, GWL_STYLE)) & (isVertical ? WS_VSCROLL : WS_HSCROLL)) != 0;
}