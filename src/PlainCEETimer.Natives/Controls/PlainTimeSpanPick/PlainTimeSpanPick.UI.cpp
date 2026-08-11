#include "pch.h"
#include "PlainTimeSpanPick.UI.h"
#include <algorithm>
#include <windowsx.h>

void PlainTimeSpanPick::RestoreCtrlColors(LPCTRLCOLORS lpColors)
{
    if (lpColors)
    {
        lpColors->backText = GetSysColor(COLOR_WINDOW);
        lpColors->foreText = GetSysColor(COLOR_WINDOWTEXT);
        lpColors->foreTextDisabled = GetSysColor(COLOR_GRAYTEXT);
    }
}

size_t PlainTimeSpanPick::BuildDisplayText(LPWSTR buffer, size_t count) const
{
    return BuildDisplayTextInternal(m_lpSegments, m_cSegments, buffer, count);
}

size_t PlainTimeSpanPick::BuildDisplayTextInternal(LPPTSPSEGMENT lpSegments, INT cSegments, LPWSTR buffer, size_t count)
{
    if (lpSegments)
    {
        if (buffer)
        {
            size_t remain = count;

            for (int i = 0; i < cSegments; ++i)
            {
                auto& seg = lpSegments[i];

                if (IsNumericPart(seg.dwType))
                {
                    HRESULT hr = StringCchPrintfEx(buffer, remain, &buffer, &remain, STRSAFE_DEFAULT, PTSP_NUMERIC_FORMAT, seg.nValue);
                    if (FAILED(hr)) break;
                }
                else if (seg.lpText && seg.cchText > 0)
                {
                    HRESULT hr = StringCchCopyEx(buffer, remain, seg.lpText, &buffer, &remain, STRSAFE_DEFAULT);
                    if (FAILED(hr)) break;
                }
            }

            return count - remain;
        }
        else
        {
            size_t length = 0;

            for (int i = 0; i < cSegments; ++i)
            {
                auto& seg = lpSegments[i];

                if (IsNumericPart(seg.dwType))
                {
                    length += _scwprintf(PTSP_NUMERIC_FORMAT, seg.nValue);
                }
                else
                {
                    length += seg.cchText;
                }
            }

            return length;
        }
    }

    return 0;
}

void PlainTimeSpanPick::ScrollNumeric(PTSPSEGMENT& seg, int delta)
{
    LONGLONG value = seg.nValue + delta;
    LONGLONG clamped = std::clamp(value, 0LL, seg.nValueMax);
    seg.nValue = clamped;

    if (clamped == value)
    {
        NotifyValueChanged();
    }
}

void PlainTimeSpanPick::NotifyValueChanged() const
{
    HWND hParent = GetParent(m_hWnd);
    if (!hParent) hParent = m_hWnd;

    SendMessage(hParent, WM_COMMAND,
        MAKEWPARAM(CastToS(UINT, GetWindowLongPtr(m_hWnd, GWLP_ID)), PTSPN_VALUECHANGE),
        CastToP(LPARAM, m_hWnd));
}

INT PlainTimeSpanPick::FindNextNumericPart(INT start, int step) const
{
    if (m_lpSegments)
    {
        INT total = m_cSegments;

        if (total > 0)
        {
            INT i = start;

            for (INT count = 0; count < total; ++count)
            {
                i = (i + step + total) % total;

                if (IsNumericPart(GetSegmentAt(i).dwType))
                {
                    return i;
                }
            }
        }
    }

    return -1;
}

void PlainTimeSpanPick::DrawMainText(HDC hdc, LPCWSTR text, int cch, LPCTRLCOLORS colors, LPRECT prc, int& x, int& y, bool isEnabled, bool isSelected)
{
    if (text && colors)
    {
        RECT rcText = {};
        DrawText(hdc, text, cch, &rcText, DT_CALCRECT | DT_NOPREFIX | DT_SINGLELINE);
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

        DrawText(hdc, text, cch, &rcBounds, DT_NOPREFIX | DT_SINGLELINE | DT_VCENTER);
        x += cx;
    }
}

LRESULT PlainTimeSpanPick::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_CREATE:
        {
            RestoreCtrlColors(&m_crCtrlColors);
            HWND hParent = CastToP(LPCREATESTRUCT, lParam)->hwndParent;
            HFONT hFont = CastToP(HFONT, SendMessage(hParent, WM_GETFONT, 0, 0));
            if (!hFont) hFont = CastToP(HFONT, GetStockObject(DEFAULT_GUI_FONT));
            m_hFont = hFont;

            if (!SetFormat(nullptr))
            {
                return -1;
            }

            return 0;
        }

        case WM_GETTEXT:
        {
            if (lParam)
            {
                return CastToS(LRESULT, BuildDisplayText(CastToP(LPWSTR, lParam), CastToS(size_t, wParam)));
            }

            return 0;
        }

        case WM_GETTEXTLENGTH:
        {
            return CastToS(LRESULT, BuildDisplayText(nullptr, 0));
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
            m_hFont = CastToP(HFONT, wParam);

            if (LOWORD(lParam))
            {
                Invalidate();
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
            if (m_lpSegments)
            {
                SetFocus(hWnd);

                int idx = -1;
                int maxcx = INT_MAX;
                int x = GET_X_LPARAM(lParam);

                for (int i = 0; i < m_cSegments; ++i)
                {
                    auto& seg = GetSegmentAt(i);

                    if (IsNumericPart(seg.dwType))
                    {
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
            if (get_SelectedIndex() >= 0)
            {
                auto& seg = get_SelectedSegment();

                if (IsNumericPart(seg.dwType))
                {
                    int delta = GET_WHEEL_DELTA_WPARAM(wParam);
                    int steps = (delta > 0) - (delta < 0);

                    if (steps != 0)
                    {
                        ScrollNumeric(seg, steps);
                        UpdateSegmentMaxValue();
                        UpdateValue();
                        Invalidate();
                        return 0;
                    }
                }
            }

            break;
        }

        case WM_CHAR:
        {
            INT i = get_SelectedIndex();

            if (i >= 0)
            {
                auto& seg = get_SelectedSegment();

                if (IsNumericPart(seg.dwType))
                {
                    WCHAR c = CastToS(WCHAR, wParam);

                    if (c >= L'0' && c <= L'9')
                    {
                        int lenMax = _scwprintf(PTSP_NUMERIC_FORMAT, seg.nValueMax);
                        LPWSTR buffer = m_lpBufferEdit;
                        int len = lstrlen(buffer);
                        int lenLast = len + 1;

                        if (len < lenMax && lenLast < PTSP_EDIT_BUFFER)
                        {
                            buffer[len] = c;
                            buffer[lenLast] = L'\0';
                            LONGLONG val = _wtoll(buffer);

                            if (val <= seg.nValueMax)
                            {
                                seg.nValue = val;

                                if (lenLast >= lenMax)
                                {
                                    ClearStringBuffer(buffer);
                                    MoveToNumericSegment(i, 1);
                                }
                            }
                            else
                            {
                                ClearStringBuffer(buffer);
                                seg.nValue = seg.nValueMax;
                            }

                            UpdateSegmentMaxValue();
                            UpdateValue();
                        }

                        Invalidate();
                        NotifyValueChanged();
                        return 0;
                    }
                }
            }

            break;
        }

        case WM_KEYDOWN:
        {
            INT i = get_SelectedIndex();

            if (i >= 0)
            {
                auto& seg = GetSegmentAt(i);

                if (IsNumericPart(seg.dwType))
                {
                    switch (wParam)
                    {
                        case VK_RIGHT:
                        case VK_LEFT:
                        {
                            ClearEditBuffer();
                            MoveToNumericSegment(i, wParam == VK_RIGHT ? 1 : -1);
                            Invalidate();
                            return 0;
                        }

                        case VK_UP:
                        case VK_DOWN:
                        {
                            ClearEditBuffer();
                            int delta = (wParam == VK_UP) ? 1 : -1;
                            ScrollNumeric(seg, delta);
                            UpdateSegmentMaxValue();
                            UpdateValue();
                            Invalidate();
                            return 0;
                        }
                    }
                }
            }

            break;
        }

        case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);
            BOOL enabled = IsWindowEnabled(hWnd);

            RECT rc;
            GetClientRect(hWnd, &rc);
            HDC cdc = CreateCompatibleDC(hdc);
            HBITMAP bm = CreateCompatibleBitmap(hdc, rc.right, rc.bottom);
            HBITMAP bmOld = CastToP(HBITMAP, SelectObject(cdc, bm));
            LPCTRLCOLORS colors = &m_crCtrlColors;

            HBRUSH hBgBrush = CreateSolidBrush(colors->backText);
            FillRect(cdc, &rc, hBgBrush);
            DeleteObject(hBgBrush);

            COLORREF crBd = GetSysColor(enabled ? COLOR_ACTIVEBORDER : COLOR_WINDOWFRAME);
            HBRUSH hBorderBrush = CreateSolidBrush(crBd);
            FrameRect(cdc, &rc, hBorderBrush);
            DeleteObject(hBorderBrush);
            HFONT fontOld = CastToP(HFONT, SelectObject(cdc, m_hFont));

            TEXTMETRIC tm;
            GetTextMetrics(cdc, &tm);
            SetBkMode(cdc, TRANSPARENT);

            int x = 4;
            int y = (rc.bottom - tm.tmHeight) / 2;
            WCHAR buffer[PTSP_EDIT_BUFFER] = {};

            for (INT i = 0; i < m_cSegments; ++i)
            {
                auto& seg = GetSegmentAt(i);

                if (IsNumericPart(seg.dwType))
                {
                    StringCchPrintf(buffer, PTSP_EDIT_BUFFER, PTSP_NUMERIC_FORMAT, seg.nValue);
                    DrawMainText(cdc, buffer, -1, colors, &seg.rcBounds, x, y, enabled, get_SelectedIndex() == i && GetFocus() == hWnd);
                }
                else
                {
                    DrawMainText(cdc, seg.lpText, seg.cchText, colors, &seg.rcBounds, x, y, enabled, false);
                }
            }

            BitBlt(hdc, 0, 0, rc.right, rc.bottom, cdc, 0, 0, SRCCOPY);

            SelectObject(cdc, fontOld);
            SelectObject(cdc, bmOld);

            DeleteObject(bm);
            DeleteDC(cdc);

            EndPaint(hWnd, &ps);
            return 0;
        }

        case WM_NCDESTROY:
        {
            FreeCore();
            HEAPFREE(m_lpBufferEdit);
            SetWindowLongPtr(hWnd, NULL, NULL);
            delete this;
            return 0;
        }

        case PTSPM_SETFORMAT:
        {
            if (lParam && SetFormat(CastToP(LPCWSTR, lParam)))
            {
                Invalidate();
                return TRUE;
            }

            return FALSE;
        }

        case PTSPM_GETVALUE:
        {
            if (lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                *pts = m_tsValue;
            }

            return 0;
        }

        case PTSPM_SETVALUE:
        {
            if (lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                TIMESPAN ts = std::clamp(*pts, TIMESPAN_ZERO, m_tsValueMax);
                UpdateSegmentValue(ts);
                Invalidate();
                NotifyValueChanged();
            }

            return 0;
        }

        case PTSPM_GETMAXVALUE:
        {
            if (lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                *pts = m_tsValueMax;
            }

            return 0;
        }

        case PTSPM_SETMAXVALUE:
        {
            if (lParam)
            {
                LPTIMESPAN pts = CastToP(LPTIMESPAN, lParam);
                UpdateMaxValue(*pts);
                Invalidate();
            }

            return 0;
        }

        case PTSPM_OVERRIDECOLORS:
        {
            LPCTRLCOLORS colors = &m_crCtrlColors;

            if (colors)
            {
                COLORREF color = PTSPCOLOR_GET_COLOR_LPARAM(lParam);

                switch (PTSPCOLOR_GET_PART_LPARAM(lParam))
                {
                    case PTSPCOLOR_RESTORE:
                        RestoreCtrlColors(colors);
                        break;
                    case PTSPCOLOR_BACKTEXT:
                        colors->backText = color;
                        break;
                    case PTSPCOLOR_FORETEXT:
                        colors->foreText = color;
                        break;
                    case PTSPCOLOR_FORETEXTDISABLED:
                        colors->foreTextDisabled = color;
                        break;
                    default:
                        return 0;
                }

                if (wParam)
                {
                    Invalidate();
                }
            }

            return 0;
        }

        case PTSPM_INCREASE:
        {
            if (get_SelectedIndex() >= 0 && IsWindowEnabled(hWnd))
            {
                auto& seg = get_SelectedSegment();

                if (IsNumericPart(seg.dwType))
                {
                    int delta = CastToS(int, wParam);
                    ScrollNumeric(seg, delta);
                    UpdateSegmentMaxValue();
                    UpdateValue();
                    ClearEditBuffer();
                    Invalidate();
                }
            }

            return 0;
        }
    }

    return DefWindowProc(hWnd, message, wParam, lParam);
}
