#include "pch.h"
#include "PlainTimeSpanPick.h"
#include <Windows.h>

BOOL PlainTimeSpanPick::ValidateFormat(LPCWSTR pszFormat)
{
    CNZWSTR format = {};
    CreateCNZWStr(format, pszFormat);
    PTSPFORMAT_PARSE_RESULT result;

    if (ParseFormat(&format, &result))
    {
        return result.cSegments > 0;
    }

    return FALSE;
}

LRESULT CALLBACK PlainTimeSpanPick::s_WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    PlainTimeSpanPick* ptr = CastToP(PlainTimeSpanPick*, GetWindowLongPtr(hWnd, NULL));

    if (message == WM_NCCREATE)
    {
        ptr = new PlainTimeSpanPick(hWnd);
        ptr->m_lpBufferEdit = HEAPALLOC_M(WCHAR, PTSP_EDIT_BUFFER);
        SetWindowLongPtr(hWnd, NULL, CastToP(LONG_PTR, ptr));
    }

    if (ptr)
    {
        return ptr->WndProc(hWnd, message, wParam, lParam);
    }

    return DefWindowProc(hWnd, message, wParam, lParam);
}

ATOM NATIVESAPI PlainTimeSpanPick_RegisterWC()
{
    WNDCLASSEX wcx = { sizeof(wcx) };
    wcx.style = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS | CS_GLOBALCLASS;
    wcx.lpfnWndProc = PlainTimeSpanPick::s_WndProc;
    wcx.cbWndExtra = sizeof(LONG_PTR);
    wcx.hInstance = GetModuleHandle(LIBRARYNAME);
    wcx.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wcx.lpszClassName = WC_PLAINTIMESPANPICK;
    return RegisterClassEx(&wcx);
}

BOOL NATIVESAPI PlainTimeSpanPick_ValidateFormat(LPCWSTR pszFormat)
{
    return PlainTimeSpanPick::ValidateFormat(pszFormat);
}
