#include "pch.h"
#include "Control.h"
#include "Utils.h"
#include "Win32/IATHook.h"
#include <CommCtrl.h>
#include <Windows.h>
#include <windowsx.h>

static HOOKPROC g_MsgBoxCbtProc = nullptr;
static HOOKPROC g_GetMsgProc = nullptr;
static HHOOK g_hGetMsgProc = nullptr;

DeclIatData(MessageBoxW, Comdlg);

static LRESULT CALLBACK CbtMessageBoxHookProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    if (g_MsgBoxCbtProc && nCode >= 0)
    {
        g_MsgBoxCbtProc(nCode, wParam, lParam);
    }

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

static LRESULT CALLBACK GetMsgHookProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    if (g_GetMsgProc && nCode >= 0)
    {
        g_GetMsgProc(nCode, wParam, lParam);
    }

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

static int WINAPI MessageBoxW_(HWND hWnd, LPCWSTR lpText, LPCWSTR lpCaption, UINT uType)
{
    HHOOK hk = SetWindowsHookEx(WH_CBT, CbtMessageBoxHookProc, nullptr, GetCurrentThreadId());
    auto ret = MessageBoxW(hWnd, lpText, lpCaption, uType);
    UnhookWindowsHookEx(hk);
    return ret;
}

//
// 使用 WinAPI 高效全选 ListView 所有项 参考：
//
// ListView_SetItemState 宏 （commctrl.h） - Win32 apps | Microsoft Learn
// https://learn.microsoft.com/zh-cn/windows/win32/api/commctrl/nf-commctrl-listview_setitemstate
//

void NATIVESAPI ListViewSelectAllItems(HWND hLV, BOOL selected)
{
    if (hLV)
    {
        ListView_SetItemState(hLV, -1, selected ? LVIS_SELECTED : 0, LVIS_SELECTED);
    }
}

void NATIVESAPI SetTopMostWindow(HWND hWnd)
{
    if (hWnd)
    {
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }
}

BOOL NATIVESAPI MenuGetItemCheckState(HMENU hMenu, UINT item, BOOL fByPosition)
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

BOOL NATIVESAPI MenuUncheckItem(HMENU hMenu, UINT item, BOOL fByPosition)
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

LPCWSTR NATIVESAPI GetWindowTextEx(HWND hWnd)
{
    if (hWnd)
    {
        int length = GetWindowTextLength(hWnd) + 1;
        LPWSTR buffer = CoTaskStrAllocW(length);
        
        if (buffer)
        {
            GetWindowText(hWnd, buffer, length);
            return buffer;
        }
    }

    return nullptr;
}

void NATIVESAPI RemoveWindowExStyle(HWND hWnd, LONG_PTR dwExStyle)
{
    SetWindowLongPtr(hWnd, GWL_EXSTYLE, GetWindowLongPtr(hWnd, GWL_EXSTYLE) & ~dwExStyle);
    SetWindowPos(hWnd, nullptr, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER);
}

BOOL NATIVESAPI CheckWindowExStyle(HWND hWnd, LONG_PTR dwExStyle)
{
    if ((GetWindowLongPtr(hWnd, GWL_EXSTYLE) & dwExStyle) == dwExStyle)
    {
        return TRUE;
    }

    return FALSE;
}

void NATIVESAPI PnHookMessageBox(HOOKPROC lpfnCbtProc, fnMessageBoxW lpfnMessageBoxW, DWORD dwHookFlag)
{
    if (!InitializeIatHook(HOOK_COMDLG32_MESSAGEBOXW_ARGS, IatHookComdlgMessageBoxW))
    {
        return;
    }

    if (dwHookFlag == HMBF_GETMSGBOX && lpfnCbtProc)
    {
        if (!g_MsgBoxCbtProc && ReplaceFunction(IatHookComdlgMessageBoxW, MessageBoxW_))
        {
            g_MsgBoxCbtProc = lpfnCbtProc;
            return;
        }
    }

    if (dwHookFlag == HMBF_REPMSGBOX && lpfnMessageBoxW && !g_MsgBoxCbtProc)
    {
        ReplaceFunction(IatHookComdlgMessageBoxW, lpfnMessageBoxW);
    }
}

void NATIVESAPI PnUnhookMessageBox()
{
    if (RestoreFunction(IatHookComdlgMessageBoxW))
    {
        g_MsgBoxCbtProc = nullptr;
    }
}

BOOL NATIVESAPI IsDialog(LPCREATESTRUCT lpCreateStruct)
{
    auto style = WS_POPUP | WS_CAPTION | DS_3DLOOK | DS_MODALFRAME;
    auto ex = WS_EX_DLGMODALFRAME;

    if ((lpCreateStruct->style & style) == style && (lpCreateStruct->dwExStyle & ex) == ex)
    {
        return TRUE;
    }

    return FALSE;
}

void NATIVESAPI RemoveWindowIcon(HWND hWnd)
{
    if (hWnd)
    {
        SendMessage(hWnd, WM_SETICON, ICON_BIG, 0);
        SendMessage(hWnd, WM_SETICON, ICON_SMALL, 0);
        SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_DLGMODALFRAME);
        SetWindowPos(hWnd, nullptr, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }
}

void NATIVESAPI PnHookGetMessage(HOOKPROC lpfnGetMsgProc, DWORD dwThreadId)
{
    if (lpfnGetMsgProc && !g_GetMsgProc)
    {
        g_hGetMsgProc = SetWindowsHookEx(WH_GETMESSAGE, GetMsgHookProc, nullptr, dwThreadId);
        g_GetMsgProc = lpfnGetMsgProc;
    }
}

void NATIVESAPI PnUnhookGetMessage()
{
    if (g_hGetMsgProc)
    {
        UnhookWindowsHookEx(g_hGetMsgProc);
        g_GetMsgProc = nullptr;
        g_hGetMsgProc = nullptr;
    }
}

int NATIVESAPI CDCCM_WmContextMenu(HWND hWnd, WPARAM wParam, LPARAM lParam)
{
    if (hWnd && wParam)
    {
        HWND hCtrl = CastP(HWND, wParam);

        if (GetDlgCtrlID(hCtrl) == COLOR_CURRENT)
        {
            static HMODULE hmod = GetModuleHandle(LIBRARYNAME);
            HMENU hMenu = LoadMenu(hmod, MAKEINTRESOURCE(IDR_COLORDLG_COLORCURRENT_MENU));

            if (hMenu)
            {
                HMENU hPopup = GetSubMenu(hMenu, 0);

                if (hPopup)
                {
                    static NMHDR nmhdr;

                    int cmd = TrackPopupMenuEx(hPopup,
                        TPM_LEFTALIGN | TPM_RIGHTBUTTON | TPM_RETURNCMD,
                        GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam), hWnd, nullptr);

                    if (cmd > 0)
                    {
                        nmhdr.hwndFrom = hCtrl;
                        nmhdr.idFrom = COLOR_CURRENT;
                        nmhdr.code = cmd;
                        SNDMSG(hCtrl, WM_NOTIFY, wParam, CastP(LPARAM, &nmhdr));
                    }

                    return cmd;
                }
            }
        }
    }

    return -1;
}
