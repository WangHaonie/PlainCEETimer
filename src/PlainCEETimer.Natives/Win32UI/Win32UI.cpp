#include "pch.h"
#include "Win32UI.h"
#include <CommCtrl.h>

/*

使用 WinAPI 高效全选 ListView 所有项 参考：

ListView_SetItemState 宏 （commctrl.h） - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/commctrl/nf-commctrl-listview_setitemstate

*/

void ListViewSelectAllItems(HWND hLV, BOOL selected)
{
    if (hLV)
    {
        ListView_SetItemState(hLV, -1, selected ? LVIS_SELECTED : 0, LVIS_SELECTED);
    }
}

BOOL MenuGetItemCheckStateByPosition(HMENU hMenu, int iItemIndex)
{
    MENUITEMINFO mii = { sizeof(mii) };
    mii.fMask = MIIM_STATE;

    if (GetMenuItemInfo(hMenu, iItemIndex, TRUE, &mii) && (mii.fState & MFS_CHECKED) != 0)
    {
        return TRUE;
    }

    return FALSE;
}
