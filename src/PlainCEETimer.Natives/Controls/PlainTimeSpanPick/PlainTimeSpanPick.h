#pragma once

#include "../../Utils.h"
#include "resource.h"

#define WC_PLAINTIMESPANPICK        L"PlainTimeSpanPick"

#define PTSPM_SETFORMAT             (WM_USER + 0x111)
#define PTSPM_GETVALUE              (WM_USER + 0x112)
#define PTSPM_SETVALUE              (WM_USER + 0x113)
#define PTSPM_GETMAXVALUE           (WM_USER + 0x114)
#define PTSPM_SETMAXVALUE           (WM_USER + 0x115)
#define PTSPM_OVERRIDECOLORS        (WM_USER + 0x116)
#define PTSPM_INCREASE              (WM_USER + 0x117)

#define PTSPN_VALUECHANGE           1

#define PTSPCOLOR_BACKTEXT          0
#define PTSPCOLOR_FORETEXT          1
#define PTSPCOLOR_FORETEXTDISABLED  2
#define PTSPCOLOR_RESTORE           0xFF

#define PTSPCOLOR_GET_PART_LPARAM(lp)   ((BYTE)((((DWORD_PTR)(lp)) >> 24) & 0xFF))
#define PTSPCOLOR_GET_COLOR_LPARAM(lp)  ((COLORREF)(((DWORD_PTR)(lp)) & 0xFFFFFF))

typedef LONGLONG TIMESPAN, *LPTIMESPAN;

typedef struct tagCTRLCOLORS
{
    COLORREF backText;
    COLORREF foreText;
    COLORREF foreTextDisabled;
} CTRLCOLORS, *LPCTRLCOLORS;

typedef struct tagCNZWSTR
{
    size_t cchString;
    LPCWSTR lpString;
} CNZWSTR, *LPCNZWSTR;

typedef struct tagPTSPSEGMENT
{
    DWORD dwType;
    INT cchText;
    LPWSTR lpText;
    LONGLONG nValue;
    LONGLONG nValueMax;
    RECT rcBounds;
} PTSPSEGMENT, *LPPTSPSEGMENT;

typedef struct PTSPFORMAT_PARSE_RESULT
{
    INT cSegments;
    INT cLiterals;
    size_t cchLiterals;
} PTSPFORMAT_PARSE_RESULT;

#define TICKS_PER_SECOND            10000000LL
#define TICKS_PER_MINUTE            (TICKS_PER_SECOND * 60LL)
#define TICKS_PER_HOUR              (TICKS_PER_MINUTE * 60LL)
#define TICKS_PER_DAY               (TICKS_PER_HOUR * 24LL)

#define MAKETIMESPAN(d, h, m, s) \
    ((TIMESPAN)(((LONGLONG)(d) * TICKS_PER_DAY) + \
     ((LONGLONG)(h) * TICKS_PER_HOUR) + \
     ((LONGLONG)(m) * TICKS_PER_MINUTE) + \
     ((LONGLONG)(s) * TICKS_PER_SECOND)))

#define GET_DAYS_TIMESPAN(ts)       (INT)((LONGLONG)(ts) / TICKS_PER_DAY)
#define GET_HOURS_TIMESPAN(ts)      (INT)(((LONGLONG)(ts) / TICKS_PER_HOUR) % 24LL)
#define GET_MINUTES_TIMESPAN(ts)    (INT)(((LONGLONG)(ts) / TICKS_PER_MINUTE) % 60LL)
#define GET_SECONDS_TIMESPAN(ts)    (INT)(((LONGLONG)(ts) / TICKS_PER_SECOND) % 60LL)

#define TIMESPAN_MIN                ((TIMESPAN)MINLONGLONG)
#define TIMESPAN_ZERO               ((TIMESPAN)0LL)
#define TIMESPAN_MAX                ((TIMESPAN)MAXLONGLONG)

#define PTSP_DEFAULT_FORMAT         L"d天h时m分s秒"

#define PTSP_EDIT_BUFFER            14
#define PTSP_NUM_SEGS_COUNT         4

class PlainTimeSpanPick
{
public:

    static BOOL ValidateFormat(LPCWSTR pszFormat);
    static HANDLE CreateFormat(LPCWSTR pszFormat);
    static BOOL Format(HANDLE hFormat, LPTIMESPAN lptsValue, LPWSTR lpBuffer, LPINT lpcchBuffer);
    static BOOL FreeFormatMemory(HANDLE hFormat);
    static LRESULT CALLBACK s_WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

private:

    explicit PlainTimeSpanPick(HWND hPtsp) : m_hWnd(hPtsp) {};

    HWND m_hWnd;
    CTRLCOLORS m_crCtrlColors = {};
    INT m_iSelected = -1;
    LPWSTR m_lpBufferEdit = nullptr;
    HFONT m_hFont = nullptr;
    LPWSTR m_lpLiterals = nullptr;
    LPPTSPSEGMENT m_lpSegments = nullptr;
    INT m_cSegments = 0;
    TIMESPAN m_tsValue = TIMESPAN_ZERO;
    TIMESPAN m_tsValueMax = MAKETIMESPAN(65535, 23, 59, 59);

    BOOL SetFormat(LPCWSTR pszFormat);
    void UpdateValue();
    void UpdateMaxValue(TIMESPAN tsMax);
    void UpdateSegmentValue(TIMESPAN tsValue);
    void UpdateSegmentMaxValue();
    void ScrollNumeric(PTSPSEGMENT& seg, int delta);
    void NotifyValueChanged() const;
    INT FindNextNumericPart(INT start, int step) const;
    LPPTSPSEGMENT FindSegmentByPart(DWORD part) const;
    size_t BuildDisplayText(LPWSTR buffer, size_t count) const;
    LRESULT WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

    static void CreateCNZWStr(CNZWSTR& snz, LPCWSTR psz);
    static BOOL CreateSegments(LPCNZWSTR lpFormat, LPWSTR lpBufferLiterals, size_t cchBufferLiterals, LPPTSPSEGMENT lpSegments, INT cSegments);
    static void DrawMainText(HDC hdc, LPCWSTR text, int cch, LPCTRLCOLORS colors, LPRECT prc, int& x, int& y, bool isEnabled, bool isSelected);
    static LONGLONG GetTicksByPart(DWORD part);
    static bool IsPartValid(WCHAR c, DWORD& part);
    static void LoadFormat(CNZWSTR& snzFormat, LPCWSTR pszFormat);
    static BOOL ParseFormat(LPCNZWSTR lpFormat, PTSPFORMAT_PARSE_RESULT* pResult);
    static BOOL TryParseFormat(LPCWSTR pszFormat, CNZWSTR& snzFormat, PTSPFORMAT_PARSE_RESULT* pResult);
    static void RestoreCtrlColors(LPCTRLCOLORS lpColors);
    static LONGLONG GetNaturalMax(DWORD part, TIMESPAN tsMax);
    static LPPTSPSEGMENT FindSegmentByPartInternal(LPPTSPSEGMENT lpSegments, INT cSegments, DWORD part);
    static void ApplySegmentValue(LPPTSPSEGMENT lpSegments, INT cSegments, TIMESPAN tsValue);
    static size_t BuildDisplayTextInternal(LPPTSPSEGMENT lpSegments, INT cSegments, LPWSTR buffer, size_t count);
};

NATIVES_EXPORT ATOM NATIVESAPI PlainTimeSpanPick_RegisterWC();
NATIVES_EXPORT BOOL NATIVESAPI PlainTimeSpanPick_ValidateFormat(LPCWSTR pszFormat);
NATIVES_EXPORT HANDLE NATIVESAPI PlainTimeSpanPick_ParseFormat(LPCWSTR pszFormat);
NATIVES_EXPORT BOOL NATIVESAPI PlainTimeSpanPick_Format(HANDLE hFormat, LPTIMESPAN lptsValue, LPWSTR lpBuffer, LPINT lpcchBuffer);
NATIVES_EXPORT BOOL NATIVESAPI PlainTimeSpanPick_FreeMemory(HANDLE hFormat);
