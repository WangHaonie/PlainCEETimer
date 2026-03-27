#include "pch.h"
#include "Control.h"
#include "Utils.h"
#include "Win32/IATHook.h"
#include <CommCtrl.h>
#include <Windows.h>

static fnMessageBoxW g_MessageBoxW = nullptr;
static fnMessageBoxW g_UserMessageBoxW = nullptr;
static HOOKPROC g_MsgBoxCbtProc = nullptr;

static LRESULT CALLBACK CbtMessageBoxHookProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    if (g_MsgBoxCbtProc)
    {
        LRESULT hr = g_MsgBoxCbtProc(nCode, wParam, lParam);

        if (SUCCEEDED(hr))
        {
            return hr;
        }
    }

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

static int WINAPI MessageBoxNew(HWND hWnd, LPCWSTR lpText, LPCWSTR lpCaption, UINT uType)
{
    HHOOK hk = SetWindowsHookEx(WH_CBT, CbtMessageBoxHookProc, nullptr, GetCurrentThreadId());
    auto ret = g_MessageBoxW(hWnd, lpText, lpCaption, uType);
    UnhookWindowsHookEx(hk);
    return ret;
}

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

BOOL MenuGetItemCheckState(HMENU hMenu, UINT item, BOOL fByPosition)
{
    if (hMenu)
    {
        MENUITEMINFO mii = { sizeof(mii) };
        mii.fMask = MIIM_STATE;

        if (GetMenuItemInfo(hMenu, item, fByPosition, &mii) && (mii.fState & MFS_CHECKED) != 0)
        {
            return TRUE;
        }
    }

    return FALSE;
}

BOOL MenuUncheckItem(HMENU hMenu, UINT item, BOOL fByPosition)
{
    if (hMenu)
    {
        MENUITEMINFO mii = { sizeof(mii) };
        mii.fMask = MIIM_STATE | MIIM_FTYPE;

        if (GetMenuItemInfo(hMenu, item, fByPosition, &mii))
        {
            mii.fState = 0;
            mii.fType &= ~MFT_RADIOCHECK;
            return SetMenuItemInfo(hMenu, item, TRUE, &mii);
        }
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

void RemoveWindowExStyle(HWND hWnd, LONG_PTR dwExStyle)
{
    SetWindowLongPtr(hWnd, GWL_EXSTYLE, GetWindowLongPtr(hWnd, GWL_EXSTYLE) & ~dwExStyle);
    SetWindowPos(hWnd, nullptr, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER);
}

void ComdlgHookMessageBox(HOOKPROC lpfnCbtProc, fnMessageBoxW lpfnMessageBoxW, DWORD dwHookFlag)
{
    if (dwHookFlag == HMBF_GETMSGBOX && lpfnCbtProc)
    {
        if (!g_MsgBoxCbtProc && ReplaceFunction<fnMessageBoxW>(HOOK_MESSAGEBOXW_ARGS, MessageBoxNew, &g_MessageBoxW) && g_MessageBoxW)
        {
            g_MsgBoxCbtProc = lpfnCbtProc;
            return;
        }
    }

    if (dwHookFlag == HMBF_REPMSGBOX && lpfnMessageBoxW && !g_MsgBoxCbtProc)
    {
        ReplaceFunction<fnMessageBoxW>(HOOK_MESSAGEBOXW_ARGS, lpfnMessageBoxW, &g_MessageBoxW);
        g_UserMessageBoxW = lpfnMessageBoxW;
    }
}

void ComdlgUnhookMessageBox()
{
    if (g_MessageBoxW)
    {
        ReplaceFunction<fnMessageBoxW>(HOOK_MESSAGEBOXW_ARGS, g_MessageBoxW, nullptr);
        g_MessageBoxW = nullptr;
        g_MsgBoxCbtProc = nullptr;
        g_UserMessageBoxW = nullptr;
    }
}

BOOL IsDialog(LPCREATESTRUCT lpCreateStruct)
{
    auto style = WS_POPUP | WS_CAPTION | DS_3DLOOK | DS_MODALFRAME;
    auto ex = WS_EX_DLGMODALFRAME;

    if ((lpCreateStruct->style & style) == style && (lpCreateStruct->dwExStyle & ex) == ex)
    {
        return TRUE;
    }

    return FALSE;
}

void RemoveWindowIcon(HWND hWnd)
{
    if (hWnd)
    {
        SendMessage(hWnd, WM_SETICON, ICON_BIG, 0);
        SendMessage(hWnd, WM_SETICON, ICON_SMALL, 0);
        SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_DLGMODALFRAME);
        SetWindowPos(hWnd, nullptr, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }
}
