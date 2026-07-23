#include "pch.h"
#include "resource.h"
#include <cstdlib>
#include <string>
#include <vector>
#include <intsafe.h>
#include <windowsx.h>
#include <algorithm>
#include "../Utils.h"
#include "PlainTimeSpanPick.h"

typedef enum _PTSPSEGTYPE
{
    PTSPSEG_LITERAL,
    PTSPSEG_NUMERIC
} PTSPSEGTYPE;

typedef enum _PTSPPART
{
    PTSPPART_DAYS,
    PTSPPART_HOURS,
    PTSPPART_MINUTES,
    PTSPPART_SECONDS
} PTSPPART;

typedef struct tagPTSPSEG
{
    PTSPSEGTYPE type;
    PTSPPART part;
    RECT bounds;
    INT value;
    INT maxValue;
    std::wstring text;
} PTSPSEG, *LPPTSPSEG;

typedef struct tagPTSPSTATE
{
    CTRLCOLORS colors;
    HFONT hFont;
    INT index;
    std::vector<PTSPSEG> segments;
    std::wstring buffer;
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

static void CreateNewState(LPPTSPSTATE lpState)
{
    if (lpState)
    {
        PTSPSTATE& state = *lpState;
        state = {};
        state.index = -1;
        auto& seg = state.segments;
        seg.reserve(8);
        seg.push_back({ PTSPSEG_NUMERIC, PTSPPART_DAYS, {}, 0, 65535 });
        seg.push_back({ PTSPSEG_LITERAL, PTSPPART_DAYS, {}, 0, 0, L"天" });
        seg.push_back({ PTSPSEG_NUMERIC, PTSPPART_HOURS, {}, 0, 23 });
        seg.push_back({ PTSPSEG_LITERAL, PTSPPART_HOURS, {}, 0, 0, L"时" });
        seg.push_back({ PTSPSEG_NUMERIC, PTSPPART_MINUTES, {}, 0, 59 });
        seg.push_back({ PTSPSEG_LITERAL, PTSPPART_MINUTES, {}, 0, 0, L"分" });
        seg.push_back({ PTSPSEG_NUMERIC, PTSPPART_SECONDS, {}, 0, 59 });
        seg.push_back({ PTSPSEG_LITERAL, PTSPPART_SECONDS, {}, 0, 0, L"秒" });
        RestoreCtrlColors(CastToP(LPCTRLCOLORS, lpState));
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

static void ScrollNumeric(PTSPSEG& seg, int delta, HWND hPtsp)
{
    int value = seg.value + delta;
    int clamped = std::clamp(value, 0, seg.maxValue);
    seg.value = clamped;

    if (clamped == value)
    {
        NotifyValueChanged(hPtsp);
    }
}

static void UpdateSegmentText(std::vector<PTSPSEG>& segs)
{
    WCHAR buffer[PTSP_SEG_BUFFER];

    for (auto& seg : segs)
    {
        if (seg.type == PTSPSEG_NUMERIC)
        {
            swprintf_s(buffer, L"%d", seg.value);
            seg.text = buffer;
        }
    }
}

static LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    LPPTSPSTATE lpState = CastToP(LPPTSPSTATE, GetWindowLongPtr(hWnd, NULL));
    
    switch (message)
    {
        case WM_NCCREATE:
        {
            LPPTSPSTATE pState = CastToP(LPPTSPSTATE, HeapAllocEx(sizeof(PTSPSTATE)));
            if (!pState) return FALSE;
            CreateNewState(pState);
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
                    InvalidateRect(hWnd, nullptr, TRUE);
                }
            }

            return 0;
        }

        case WM_SETFOCUS:
        case WM_KILLFOCUS:
        {
            InvalidateRect(hWnd, nullptr, TRUE);
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

                for (int i = 0; i < lpState->segments.size(); i++)
                {
                    auto& seg = lpState->segments[i];

                    if (seg.type != PTSPSEG_NUMERIC)
                    {
                        continue;
                    }

                    int cx;

                    if (x < seg.bounds.left)
                    {
                        cx = seg.bounds.left - x;
                    }
                    else if (x > seg.bounds.right)
                    {
                        cx = x - seg.bounds.right;
                    }
                    else
                    {
                        cx = 0;
                    }

                    if (cx < maxcx)
                    {
                        maxcx = cx;
                        idx = i;
                    }
                }

                if (idx >= 0)
                {
                    lpState->index = idx;
                    lpState->buffer.clear();
                    InvalidateRect(hWnd, nullptr, TRUE);
                }

                return 0;
            }

            break;
        }
        
        case WM_MOUSEWHEEL:
        {
            if (lpState && lpState->index >= 0)
            {
                int delta = GET_WHEEL_DELTA_WPARAM(wParam);
                int steps = (delta > 0) - (delta < 0);

                if (steps != 0)
                {
                    ScrollNumeric(lpState->segments[lpState->index], steps, hWnd);
                    InvalidateRect(hWnd, nullptr, TRUE);
                    return 0;
                }
            }

            break;
        }

        case WM_CHAR:
        {
            if (lpState && lpState->index >= 0)
            {
                WCHAR c = CastToS(WCHAR, wParam);

                if (c >= L'0' && c <= L'9')
                {
                    auto& seg = lpState->segments[lpState->index];

                    if (seg.type == PTSPSEG_NUMERIC)
                    {
                        WCHAR buffer[PTSP_SEG_BUFFER];
                        swprintf_s(buffer, L"%d", seg.maxValue);
                        size_t length = wcslen(buffer);

                        auto& segBuffer = lpState->buffer;
                        std::wstring buf = segBuffer + c;
                        int val = _wtoi(buf.c_str());

                        if (buf.length() <= length && val <= seg.maxValue)
                        {
                            segBuffer = buf;
                            seg.value = val;

                            if (segBuffer.length() >= length)
                            {
                                segBuffer.clear();

                                int i = lpState->index;
                                do
                                {
                                    i = (i + 1) % lpState->segments.size();
                                }
                                while (lpState->segments[i].type != PTSPSEG_NUMERIC);

                                lpState->index = i;
                            }
                        }
                        else
                        {
                            segBuffer.clear();
                            seg.value = seg.maxValue;
                        }

                        InvalidateRect(hWnd, nullptr, TRUE);
                        NotifyValueChanged(hWnd);
                        return 0;
                    }
                }
            }

            break;
        }

        case WM_KEYDOWN:
        {
            if (lpState && lpState->index >= 0)
            {
                int i = lpState->index;
                auto& segs = lpState->segments;
                int length = CastToS(int, segs.size());

                switch (wParam)
                {
                    case VK_RIGHT:
                    case VK_LEFT:
                    {
                        lpState->buffer.clear();

                        do
                        {
                            i = wParam == VK_RIGHT ? ((i + 1) % length) : ((i - 1 + length) % length);
                        }
                        while (segs[i].type != PTSPSEG_NUMERIC);

                        lpState->index = i;
                        InvalidateRect(hWnd, nullptr, TRUE);
                        return 0;
                    }

                    case VK_UP:
                    case VK_DOWN:
                    {
                        lpState->buffer.clear();
                        auto& seg = segs[lpState->index];
                        int delta = (wParam == VK_UP) ? 1 : -1;
                        ScrollNumeric(seg, delta, hWnd);
                        InvalidateRect(hWnd, nullptr, TRUE);
                        return 0;
                    }
                    
                    default:
                    {
                        break;
                    }
                }
            }

            break;
        }

        case WM_ENABLE:
        {
            InvalidateRect(hWnd, nullptr, TRUE);
            return 0;
        }

        case WM_PAINT:
        {
            if (lpState)
            {
                PAINTSTRUCT ps;
                HDC hdc = BeginPaint(hWnd, &ps);

                RECT rc;
                GetClientRect(hWnd, &rc);
                HDC cdc = CreateCompatibleDC(hdc);
                HBITMAP bm = CreateCompatibleBitmap(hdc, rc.right, rc.bottom);
                HBITMAP bmOld = CastToP(HBITMAP, SelectObject(cdc, bm));
                LPCTRLCOLORS colors = CastToP(LPCTRLCOLORS, lpState);

                HBRUSH hBgBrush = CreateSolidBrush(colors->backText);
                FillRect(cdc, &rc, hBgBrush);

                HFONT fontOld = CastToP(HFONT, SelectObject(cdc, lpState->hFont));
                UpdateSegmentText(lpState->segments);

                TEXTMETRIC tm;
                GetTextMetrics(cdc, &tm);

                int x = 5;
                int y = (rc.bottom - tm.tmHeight) / 2;

                for (int i = 0; i < lpState->segments.size(); i++)
                {
                    auto& seg = lpState->segments[i];
                    LPCWSTR text = seg.text.c_str();

                    RECT rcText = {};
                    DrawTextW(cdc, text, -1, &rcText, DT_CALCRECT | DT_NOPREFIX | DT_SINGLELINE);

                    int cx = rcText.right - rcText.left;
                    int cy = rcText.bottom - rcText.top;
                    seg.bounds = { x, y, x + cx, y + cy };

                    if (seg.type == PTSPSEG_NUMERIC && lpState->index == i && GetFocus() == hWnd)
                    {
                        FillRect(cdc, &seg.bounds, GetSysColorBrush(COLOR_HIGHLIGHT));
                        SetTextColor(cdc, GetSysColor(COLOR_HIGHLIGHTTEXT));
                    }
                    else
                    {
                        SetTextColor(cdc, IsWindowEnabled(hWnd) ? colors->foreText : colors->foreTextDisabled);
                    }

                    SetBkMode(cdc, TRANSPARENT);
                    DrawText(cdc, text, -1, &seg.bounds, DT_NOPREFIX | DT_SINGLELINE | DT_VCENTER);
                    x += cx;
                }

                BitBlt(hdc, 0, 0, rc.right, rc.bottom, cdc, 0, 0, SRCCOPY);

                SelectObject(cdc, fontOld);
                SelectObject(cdc, bmOld);

                DeleteObject(bm);
                DeleteObject(hBgBrush);
                DeleteDC(cdc);

                EndPaint(hWnd, &ps);
                return 0;
            }

            break;
        }

        case WM_NCDESTROY:
        {
            HeapFreeEx(lpState);
            SetWindowLongPtr(hWnd, GWLP_USERDATA, NULL);
            return 0;
        }

        case PTSPM_GETVALUE:
        {
            if (lpState && lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                auto& segs = lpState->segments;
                *pts = MAKETIMESPAN(segs[0].value, segs[2].value, segs[4].value, segs[6].value);
            }

            return 0;
        }

        case PTSPM_SETVALUE:
        {
            if (lpState && lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                TIMESPAN ts = std::clamp(*pts, TIMESPAN_ZERO, TIMESPAN_MAX);
                auto& segs = lpState->segments;
                segs[0].value = std::clamp(GET_DAYS_TIMESPAN(ts), TIMESPAN_DAYS_ZERO, segs[0].maxValue);
                segs[2].value = GET_HOURS_TIMESPAN(ts);
                segs[4].value = GET_MINUTES_TIMESPAN(ts);
                segs[6].value = GET_SECONDS_TIMESPAN(ts);
                InvalidateRect(hWnd, nullptr, TRUE);
                NotifyValueChanged(hWnd);
            }

            return 0;
        }

        case PTSPM_GETDAYSMAX:
        {
            if (lpState && lParam)
            {
                LPINT pl = CastToP(LPINT, lParam);
                *pl = lpState->segments[0].maxValue;
            }

            return 0;
        }

        case PTSPM_SETDAYSMAX:
        {
            if (lpState && lParam)
            {
                LPINT pl = CastToP(LPINT, lParam);
                int value = std::clamp(*pl, TIMESPAN_DAYS_ZERO, TIMESPAN_DAYS_MAX);
                lpState->segments[0].maxValue = value;
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
                        InvalidateRect(hWnd, nullptr, TRUE);
                    }
                }
            }

            return 0;
        }

        case PTSPM_INCREASE:
        {
            if (lpState && lpState->index >= 0 && IsWindowEnabled(hWnd))
            {
                auto& seg = lpState->segments[lpState->index];

                if (seg.type == PTSPSEG_NUMERIC)
                {
                    int delta = CastToS(int, wParam);
                    ScrollNumeric(seg, delta, hWnd);
                    lpState->buffer.clear();
                    InvalidateRect(hWnd, nullptr, TRUE);
                }
            }

            return 0;
        }
        
        default:
        {
            break;
        }
    }

    return DefWindowProc(hWnd, message, wParam, lParam);
}

ATOM PlainTimeSpanPick_RegisterWC()
{
    WNDCLASSEX wcx = { sizeof(wcx) };
    wcx.style = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS | CS_GLOBALCLASS;
    wcx.lpfnWndProc = WndProc;
    wcx.cbWndExtra = sizeof(LONG_PTR);
    wcx.hInstance = GetModuleHandle(LIBRARYNAME);
    wcx.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wcx.lpszClassName = WC_PLAINTIMESPANPICK;
    return RegisterClassEx(&wcx);
}
