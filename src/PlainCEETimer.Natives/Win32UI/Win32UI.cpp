#include "pch.h"
#include "Win32UI.h"
#include <CommCtrl.h>
#include <string>

using namespace std;

//
// 使用 WinAPI 高效全选 ListView 所有项 参考：
//
// ListView_SetItemState 宏 （commctrl.h） - Win32 apps | Microsoft Learn
// https://learn.microsoft.com/zh-cn/windows/win32/api/commctrl/nf-commctrl-listview_setitemstate
//

void ListViewSelectAllItems(HWND hLV, BOOL selected)
{
    if (hLV)
    {
        ListView_SetItemState(hLV, -1, selected ? LVIS_SELECTED : 0, LVIS_SELECTED);
    }
}

void SetTopMostWindow(HWND hWnd)
{
    SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
}

BOOL MenuGetItemCheckStateByPosition(HMENU hMenu, UINT item)
{
    MENUITEMINFO mii = { sizeof(mii) };
    mii.fMask = MIIM_STATE;

    if (GetMenuItemInfo(hMenu, item, TRUE, &mii) && (mii.fState & MFS_CHECKED) != 0)
    {
        return TRUE;
    }

    return FALSE;
}

BOOL MenuCheckRadioItemByPosition(HMENU hMenu, UINT item)
{
    return CheckMenuRadioItem(hMenu, 0, GetMenuItemCount(hMenu) - 1, item, MF_BYPOSITION);
}

LPCWSTR GetWindowTextEx(HWND hWnd)
{
    wstring buffer;
    buffer.reserve(GetWindowTextLength(hWnd) + 1);
    GetWindowText(hWnd, &buffer[0], (int)buffer.capacity());
    return _wcsdup(buffer.c_str());
}
