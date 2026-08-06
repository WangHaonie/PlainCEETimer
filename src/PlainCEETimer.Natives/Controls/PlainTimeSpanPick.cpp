#include "pch.h"
#include "../Utils.h"
#include "PlainTimeSpanPick.h"
#include "resource.h"
#include <algorithm>
#include <cstdlib>
#include <strsafe.h>
#include <windowsx.h>

#define PTSPPART_DAYS               0
#define PTSPPART_HOURS              1
#define PTSPPART_MINUTES            2
#define PTSPPART_SECONDS            3

#define PTSPPART_MIN                PTSPPART_DAYS
#define PTSPPART_MAX                PTSPPART_SECONDS

#define PTSP_NUMERIC_FORMAT         L"%d"

/* Fast Access Macros - begin */
#define get_SelectedIndex()         lpState->iSelected
#define set_SelectedIndex(value)    get_SelectedIndex() = value

#define GetSegmentAt(index)         lpState->lpNumerics[index]
#define get_SelectedSegment()       GetSegmentAt(get_SelectedIndex())

#define ClearEditBuffer()           PtspClearStringBuffer(lpState->lpBufferEdit)

#define Invalidate()                InvalidateRect(hWnd, nullptr, TRUE)
/* Fast Access Macros - end */

typedef struct tagPTSPLITERALSEG
{
    LPWSTR pszText;
} PTSPLITERALSEG, *LPPTSPLITERALSEG;

typedef struct tagPTSPNUMERICSEG
{
    DWORD dwPart;
    INT nValue;
    INT nValueMax;
    RECT rcBounds;
} PTSPNUMERICSEG, *LPPTSPNUMERICSEG;

typedef struct tagPTSPSTATE
{
    CTRLCOLORS crCtrlColors;
    INT iSelected;
    LPWSTR lpBufferEdit;
    HFONT hFont;
    LPPTSPLITERALSEG lpLiterals;
    LPPTSPNUMERICSEG lpNumerics;
} PTSPSTATE, *LPPTSPSTATE;

static void RestoreCtrlColors(LPCTRLCOLORS colors)
{
    if (colors)
    {
        colors->backText = GetSysColor(COLOR_WINDOW);
        colors->foreText = GetSysColor(COLOR_WINDOWTEXT);
        colors->foreTextDisabled = GetSysColor(COLOR_GRAYTEXT);
    }
}

static void NotifyValueChanged(HWND hWnd)
{
    HWND hParent = GetParent(hWnd);
    if (!hParent) hParent = hWnd;

    SendMessage(hParent, WM_COMMAND,
        MAKEWPARAM(CastToS(UINT, GetWindowLongPtr(hWnd, GWLP_ID)), PTSPN_VALUECHANGE),
        CastToP(LPARAM, hWnd));
}

static void PtspFreeMemory(LPPTSPSTATE lpState)
{
    if (lpState)
    {
        HEAPFREE(lpState->lpBufferEdit);
        HEAPFREE(lpState->lpLiterals);
        HEAPFREE(lpState->lpNumerics);
    }
}

static void PtspClearStringBuffer(LPWSTR buffer)
{
    if (buffer) *buffer = L'\0';
}

static void PtspDrawText(HDC hdc, LPCWSTR text, LPCTRLCOLORS colors, LPRECT prc, int& x, int& y, bool isEnabled, bool isSelected)
{
    if (text && colors)
    {
        RECT rcText = {};
        DrawText(hdc, text, -1, &rcText, DT_CALCRECT | DT_NOPREFIX | DT_SINGLELINE);
        int cx = rcText.right - rcText.left;
        int cy = rcText.bottom - rcText.top;

        RECT rcBounds = { x, y, x + cx, y + cy };
        if (prc) *prc = rcBounds;

        if (isSelected)
        {
            FillRect(hdc, &rcBounds, GetSysColorBrush(COLOR_HIGHLIGHT));
            SetTextColor(hdc, GetSysColor(COLOR_HIGHLIGHTTEXT));
        }
        else
        {
            SetTextColor(hdc, isEnabled ? colors->foreText : colors->foreTextDisabled);
        }

        DrawText(hdc, text, -1, &rcBounds, DT_NOPREFIX | DT_SINGLELINE | DT_VCENTER);
        x += cx;
    }
}

static void PtspCreateNewState(LPPTSPSTATE lpState)
{
    if (lpState)
    {
        PtspFreeMemory(lpState);
        RestoreCtrlColors(CastToP(LPCTRLCOLORS, lpState));
        PTSPSTATE& state = *lpState;
        state.iSelected = -1;
        state.lpBufferEdit = HEAPALLOC_M(WCHAR, PTSP_EDIT_BUFFER);

        PZPCWSTR literals = HEAPALLOC_M(LPCWSTR, PTSP_SEGS_COUNT);
        literals[PTSPPART_DAYS] = L"天";
        literals[PTSPPART_HOURS] = L"时";
        literals[PTSPPART_MINUTES] = L"分";
        literals[PTSPPART_SECONDS] = L"秒";
        state.lpLiterals = CastToP(LPPTSPLITERALSEG, literals);

        LPPTSPNUMERICSEG numerics = HEAPALLOC_M(PTSPNUMERICSEG, PTSP_SEGS_COUNT);
        numerics[PTSPPART_DAYS] = { PTSPPART_DAYS, 0, 65535 };
        numerics[PTSPPART_HOURS] = { PTSPPART_HOURS, 0, 23 };
        numerics[PTSPPART_MINUTES] = { PTSPPART_MINUTES, 0, 59 };
        numerics[PTSPPART_SECONDS] = { PTSPPART_SECONDS, 0, 59 };
        state.lpNumerics = numerics;
    }
}

static void PtspScrollNumeric(PTSPNUMERICSEG& seg, int delta, HWND hPtsp)
{
    int value = seg.nValue + delta;
    int clamped = std::clamp(value, 0, seg.nValueMax);
    seg.nValue = clamped;

    if (clamped == value)
    {
        NotifyValueChanged(hPtsp);
    }
}

static size_t PtspBuildDisplayText(LPPTSPSTATE lpState, LPWSTR buffer, size_t count)
{
    if (lpState)
    {
        PZPCWSTR literals = CastToP(PZPCWSTR, lpState->lpLiterals);
        LPPTSPNUMERICSEG numerics = lpState->lpNumerics;

        if (buffer)
        {
            size_t remain = count;

            for (int i = 0; i < PTSP_SEGS_COUNT; ++i)
            {
                HRESULT hr = StringCchPrintfExW(buffer, remain, &buffer, &remain, STRSAFE_DEFAULT, PTSP_NUMERIC_FORMAT, numerics[i].nValue);
                if (FAILED(hr)) break;
                hr = StringCchCopyExW(buffer, remain, literals[i], &buffer, &remain, STRSAFE_DEFAULT);
                if (FAILED(hr)) break;
            }

            return count - remain;
        }
        else
        {
            size_t size = 0;

            for (int i = 0; i < PTSP_SEGS_COUNT; ++i)
            {
                size += _scwprintf(PTSP_NUMERIC_FORMAT, numerics[i].nValue);
                size += lstrlen(literals[i]);
            }

            return size;
        }
    }

    return 0;
}

static LRESULT CALLBACK PlainTimeSpanPick_WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    LPPTSPSTATE lpState = CastToP(LPPTSPSTATE, GetWindowLongPtr(hWnd, NULL));
    
    switch (message)
    {
        case WM_NCCREATE:
        {
            LPPTSPSTATE pState = HEAPALLOC(PTSPSTATE);
            if (!pState) return FALSE;
            PtspCreateNewState(pState);
            SetWindowLongPtr(hWnd, NULL, CastToP(LONG_PTR, pState));
            break;
        }

        case WM_CREATE:
        {
            HWND hParent = CastToP(LPCREATESTRUCT, lParam)->hwndParent;
            HFONT hFont = CastToP(HFONT, SendMessage(hParent, WM_GETFONT, 0, 0));
            if (!hFont) hFont = CastToP(HFONT, GetStockObject(DEFAULT_GUI_FONT));
            if (hFont && lpState) lpState->hFont = hFont;
            return 0;
        }

        case WM_GETTEXT:
        {
            if (lpState && lParam)
            {
                return CastToS(LRESULT, PtspBuildDisplayText(lpState, CastToP(LPWSTR, lParam), CastToS(size_t, wParam)));
            }

            return 0;
        }

        case WM_GETTEXTLENGTH:
        {
            if (lpState)
            {
                return CastToS(LRESULT, PtspBuildDisplayText(lpState, nullptr, 0));
            }

            return 0;
        }

        case WM_SETTEXT:
        {
            return FALSE;
        }

        case WM_GETDLGCODE:
        {
            return DLGC_WANTARROWS | DLGC_WANTCHARS;
        }

        case WM_SETFONT:
        {
            if (lpState)
            {
                lpState->hFont = CastToP(HFONT, wParam);

                if (LOWORD(lParam))
                {
                    Invalidate();
                }
            }

            return 0;
        }

        case WM_ENABLE:
        case WM_SETFOCUS:
        case WM_KILLFOCUS:
        {
            Invalidate();
            return 0;
        }

        case WM_LBUTTONDOWN:
        {
            if (lpState)
            {
                SetFocus(hWnd);

                int idx = -1;
                int maxcx = INT_MAX;
                int x = GET_X_LPARAM(lParam);

                for (int i = 0; i < PTSP_SEGS_COUNT; ++i)
                {
                    auto& seg = GetSegmentAt(i);
                    auto& rc = seg.rcBounds;
                    int cx;

                    if (x < rc.left)
                        cx = rc.left - x;
                    else if (x > rc.right)
                        cx = x - rc.right;
                    else
                        cx = 0;

                    if (cx < maxcx)
                    {
                        maxcx = cx;
                        idx = i;
                    }
                }

                if (idx >= 0)
                {
                    set_SelectedIndex(idx);
                    ClearEditBuffer();
                    Invalidate();
                }

                return 0;
            }

            break;
        }
        
        case WM_MOUSEWHEEL:
        {
            if (lpState && get_SelectedIndex() >= 0)
            {
                int delta = GET_WHEEL_DELTA_WPARAM(wParam);
                int steps = (delta > 0) - (delta < 0);

                if (steps != 0)
                {
                    PtspScrollNumeric(get_SelectedSegment(), steps, hWnd);
                    Invalidate();
                    return 0;
                }
            }

            break;
        }

        case WM_CHAR:
        {
            if (lpState && get_SelectedIndex() >= 0)
            {
                WCHAR c = CastToS(WCHAR, wParam);

                if (c >= L'0' && c <= L'9')
                {
                    auto& seg = get_SelectedSegment();
                    int lenMax = _scwprintf(PTSP_NUMERIC_FORMAT, seg.nValueMax);
                    LPWSTR buffer = lpState->lpBufferEdit;
                    int len = lstrlen(buffer);

                    if (len < lenMax && len + 1 < PTSP_EDIT_BUFFER)
                    {
                        buffer[len] = c;
                        buffer[len + 1] = L'\0';

                        int val = _wtoi(buffer);
                        int lenTest = len + 1;

                        if (val <= seg.nValueMax)
                        {
                            seg.nValue = val;

                            if (lenTest >= lenMax)
                            {
                                PtspClearStringBuffer(buffer);
                                int i = get_SelectedIndex();
                                set_SelectedIndex((i + 1) % PTSP_SEGS_COUNT);
                            }
                        }
                        else
                        {
                            PtspClearStringBuffer(buffer);
                            seg.nValue = seg.nValueMax;
                        }
                    }

                    Invalidate();
                    NotifyValueChanged(hWnd);
                    return 0;
                }
            }

            break;
        }

        case WM_KEYDOWN:
        {
            if (lpState)
            {
                int i = get_SelectedIndex();

                if (i >= 0)
                {
                    switch (wParam)
                    {
                        case VK_RIGHT:
                        case VK_LEFT:
                        {
                            ClearEditBuffer();
                            set_SelectedIndex(wParam == VK_RIGHT
                                ? ((i + 1) % PTSP_SEGS_COUNT)
                                : ((i - 1 + PTSP_SEGS_COUNT) % PTSP_SEGS_COUNT));
                            Invalidate();
                            return 0;
                        }

                        case VK_UP:
                        case VK_DOWN:
                        {
                            ClearEditBuffer();
                            auto& seg = get_SelectedSegment();
                            int delta = (wParam == VK_UP) ? 1 : -1;
                            PtspScrollNumeric(seg, delta, hWnd);
                            Invalidate();
                            return 0;
                        }

                        default:
                        {
                            break;
                        }
                    }
                }
            }

            break;
        }

        case WM_PAINT:
        {
            if (lpState)
            {
                PAINTSTRUCT ps;
                HDC hdc = BeginPaint(hWnd, &ps);
                BOOL enabled = IsWindowEnabled(hWnd);

                RECT rc;
                GetClientRect(hWnd, &rc);
                HDC cdc = CreateCompatibleDC(hdc);
                HBITMAP bm = CreateCompatibleBitmap(hdc, rc.right, rc.bottom);
                HBITMAP bmOld = CastToP(HBITMAP, SelectObject(cdc, bm));
                LPCTRLCOLORS colors = CastToP(LPCTRLCOLORS, lpState);

                HBRUSH hBgBrush = CreateSolidBrush(colors->backText);
                FillRect(cdc, &rc, hBgBrush);
                DeleteObject(hBgBrush);
                
                COLORREF crBd = GetSysColor(enabled ? COLOR_ACTIVEBORDER : COLOR_WINDOWFRAME);
                HBRUSH hBorderBrush = CreateSolidBrush(crBd);
                FrameRect(cdc, &rc, hBorderBrush);
                DeleteObject(hBorderBrush);
                HFONT fontOld = CastToP(HFONT, SelectObject(cdc, lpState->hFont));

                TEXTMETRIC tm;
                GetTextMetrics(cdc, &tm);
                SetBkMode(cdc, TRANSPARENT);

                int x = 4;
                int y = (rc.bottom - tm.tmHeight) / 2;

                WCHAR buffer[PTSP_EDIT_BUFFER] = {};

                for (int i = 0; i < PTSP_SEGS_COUNT; ++i)
                {
                    auto& numeric = GetSegmentAt(i);
                    StringCchPrintf(buffer, PTSP_EDIT_BUFFER, PTSP_NUMERIC_FORMAT, numeric.nValue);
                    PtspDrawText(cdc, buffer, colors, &numeric.rcBounds, x, y, enabled, get_SelectedIndex() == i && GetFocus() == hWnd);

                    PZPCWSTR literals = CastToP(PZPCWSTR, lpState->lpLiterals);
                    PtspDrawText(cdc, literals[i], colors, nullptr, x, y, enabled, false);
                }

                BitBlt(hdc, 0, 0, rc.right, rc.bottom, cdc, 0, 0, SRCCOPY);

                SelectObject(cdc, fontOld);
                SelectObject(cdc, bmOld);

                DeleteObject(bm);
                DeleteDC(cdc);

                EndPaint(hWnd, &ps);
                return 0;
            }

            break;
        }

        case WM_NCDESTROY:
        {
            PtspFreeMemory(lpState);
            HEAPFREE(lpState);
            SetWindowLongPtr(hWnd, GWLP_USERDATA, NULL);
            return 0;
        }

        case PTSPM_GETVALUE:
        {
            if (lpState && lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                LPPTSPNUMERICSEG segs = lpState->lpNumerics;

                *pts = MAKETIMESPAN(
                    segs[PTSPPART_DAYS].nValue,
                    segs[PTSPPART_HOURS].nValue,
                    segs[PTSPPART_MINUTES].nValue,
                    segs[PTSPPART_SECONDS].nValue
                );
            }

            return 0;
        }

        case PTSPM_SETVALUE:
        {
            if (lpState && lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                TIMESPAN ts = std::clamp(*pts, TIMESPAN_ZERO, TIMESPAN_MAX);
                LPPTSPNUMERICSEG segs = lpState->lpNumerics;
                segs[PTSPPART_DAYS].nValue = std::clamp(GET_DAYS_TIMESPAN(ts), TIMESPAN_DAYS_ZERO, segs[PTSPPART_DAYS].nValueMax);
                segs[PTSPPART_HOURS].nValue = GET_HOURS_TIMESPAN(ts);
                segs[PTSPPART_MINUTES].nValue = GET_MINUTES_TIMESPAN(ts);
                segs[PTSPPART_SECONDS].nValue = GET_SECONDS_TIMESPAN(ts);
                Invalidate();
                NotifyValueChanged(hWnd);
            }

            return 0;
        }

        case PTSPM_GETDAYSMAX:
        {
            if (lpState && lParam)
            {
                LPINT pl = CastToP(LPINT, lParam);
                *pl = GetSegmentAt(PTSPPART_DAYS).nValueMax;
            }

            return 0;
        }

        case PTSPM_SETDAYSMAX:
        {
            if (lpState && lParam)
            {
                LPINT pl = CastToP(LPINT, lParam);
                int value = std::clamp(*pl, TIMESPAN_DAYS_ZERO, TIMESPAN_DAYS_MAX);
                GetSegmentAt(PTSPPART_DAYS).nValueMax = value;
            }

            return 0;
        }

        case PTSPM_OVERRIDECOLORS:
        {
            if (lpState)
            {
                LPCTRLCOLORS colors = CastToP(LPCTRLCOLORS, lpState);

                if (colors)
                {
                    COLORREF color = PTSPCOLOR_GET_COLOR_LPARAM(lParam);

                    switch (PTSPCOLOR_GET_PART_LPARAM(lParam))
                    {
                        case PTSPCOLOR_RESTORE:
                        {
                            RestoreCtrlColors(colors);
                            break;
                        }

                        case PTSPCOLOR_BACKTEXT:
                        {
                            colors->backText = color;
                            break;
                        }

                        case PTSPCOLOR_FORETEXT:
                        {
                            colors->foreText = color;
                            break;
                        }

                        case PTSPCOLOR_FORETEXTDISABLED:
                        {
                            colors->foreTextDisabled = color;
                            break;
                        }

                        default:
                        {
                            return 0;
                        }
                    }

                    if (wParam)
                    {
                        Invalidate();
                    }
                }
            }

            return 0;
        }

        case PTSPM_INCREASE:
        {
            if (lpState && get_SelectedIndex() >= 0 && IsWindowEnabled(hWnd))
            {
                auto& seg = get_SelectedSegment();
                int delta = CastToS(int, wParam);
                PtspScrollNumeric(seg, delta, hWnd);
                ClearEditBuffer();
                Invalidate();
            }

            return 0;
        }
    }

    return DefWindowProc(hWnd, message, wParam, lParam);
}

ATOM NATIVESAPI PlainTimeSpanPick_RegisterWC()
{
    WNDCLASSEX wcx = { sizeof(wcx) };
    wcx.style = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS | CS_GLOBALCLASS;
    wcx.lpfnWndProc = PlainTimeSpanPick_WndProc;
    wcx.cbWndExtra = sizeof(LONG_PTR);
    wcx.hInstance = GetModuleHandle(LIBRARYNAME);
    wcx.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wcx.lpszClassName = WC_PLAINTIMESPANPICK;
    return RegisterClassEx(&wcx);
}
