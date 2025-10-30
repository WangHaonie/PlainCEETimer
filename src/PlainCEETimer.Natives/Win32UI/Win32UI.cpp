#include "pch.h"
#include "Win32UI.h"
#include "Utils.h"
#include <CommCtrl.h>
#include <Uxtheme.h>

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
    if (hWnd)
    {
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }
}

void RemoveWindowExStyles(HWND hWnd, LONG_PTR dwExStyles)
{
    SetWindowLongPtr(hWnd, GWL_EXSTYLE, GetWindowLongPtr(hWnd, GWL_EXSTYLE) & ~dwExStyles);
    SetWindowPos(hWnd, nullptr, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER);
}

BOOL MenuGetItemCheckStateByPosition(HMENU hMenu, UINT item)
{
    if (hMenu)
    {
        MENUITEMINFO mii = { sizeof(mii) };
        mii.fMask = MIIM_STATE;

        if (GetMenuItemInfo(hMenu, item, TRUE, &mii) && (mii.fState & MFS_CHECKED) != 0)
        {
            return TRUE;
        }
    }

    return FALSE;
}

BOOL MenuCheckRadioItemByPosition(HMENU hMenu, UINT item)
{
    if (hMenu)
    {
        return CheckMenuRadioItem(hMenu, 0, GetMenuItemCount(hMenu) - 1, item, MF_BYPOSITION);
    }

    return FALSE;
}

LPCWSTR GetWindowTextEx(HWND hWnd)
{
    if (hWnd)
    {
        int length = GetWindowTextLength(hWnd) + 1;
        LPWSTR buffer = CoTaskStrAllocW(length, nullptr);
        
        if (buffer)
        {
            GetWindowText(hWnd, buffer, length);
            return buffer;
        }
    }

    return nullptr;
}

LPCWSTR GetWindowClassName(HWND hWnd)
{
    if (hWnd)
    {
        LPWSTR buffer = CoTaskStrAllocW(256, nullptr); // lpszClassName 最大长度
        GetClassName(hWnd, buffer, 256);
        return buffer;
    }

    return nullptr;
}