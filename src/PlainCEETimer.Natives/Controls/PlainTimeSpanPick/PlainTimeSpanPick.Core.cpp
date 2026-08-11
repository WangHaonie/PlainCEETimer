#include "pch.h"
#include "PlainTimeSpanPick.Core.h"
#include "PlainTimeSpanPick.h"
#include "PlainTimeSpanPick.UI.h"
#include <algorithm>

typedef struct tagPTSPFORMAT
{
    LPWSTR lpLiterals;
    LPPTSPSEGMENT lpSegments;
    INT cchLiterals;
    INT cSegments;
} PTSPFORMAT, *LPPTSPFORMAT;

void PlainTimeSpanPick::CreateCNZWStr(CNZWSTR& snz, LPCWSTR psz)
{
    snz.cchString = lstrlen(psz);
    snz.lpString = psz;
}

BOOL PlainTimeSpanPick::SetFormat(LPCWSTR pszFormat)
{
    CNZWSTR str = {};
    LoadFormat(str, pszFormat);
    LPCNZWSTR lpFormat = &str;
    PTSPFORMAT_PARSE_RESULT result;

    if (ParseFormat(lpFormat, &result) && result.cSegments > 0)
    {
        size_t cchBufferLiteral = result.cchLiterals + result.cLiterals;

        if (cchBufferLiteral)
        {
            INT cSegments = result.cSegments;
            LPWSTR literals = HEAPALLOC_M(WCHAR, cchBufferLiteral);
            LPPTSPSEGMENT segments = HEAPALLOC_M(PTSPSEGMENT, cSegments);

            if (literals && segments
                && CreateSegments(lpFormat, literals, cchBufferLiteral, segments, cSegments))
            {
                FreeCore();
                m_lpLiterals = literals;
                m_lpSegments = segments;
                m_cSegments = cSegments;
                m_iSelected = -1;
                UpdateSegmentValue(m_tsValue);
                return TRUE;
            }

            HEAPFREE(segments);
            HEAPFREE(literals);
        }
    }

    return FALSE;
}

void PlainTimeSpanPick::LoadFormat(CNZWSTR& snzFormat, LPCWSTR pszFormat)
{
    if (WString_IsNullOrEmpty(pszFormat))
    {
        static LPWSTR res = nullptr;
        static int cch = 0;

        if (!res || !cch)
        {
            cch = LoadStringExW(GetModuleHandleW(LIBRARYNAME), IDS_CTRL_PTSP_FORMAT, &res);
        }

        LPWSTR pBuffer = res;

        if (cch > 0 && pBuffer)
        {
            snzFormat.cchString = cch;
            snzFormat.lpString = pBuffer;
        }
        else
        {
            CreateCNZWStr(snzFormat, PTSP_DEFAULT_FORMAT);
        }
    }
    else
    {
        CreateCNZWStr(snzFormat, pszFormat);
    }
}

BOOL PlainTimeSpanPick::ParseFormat(LPCNZWSTR lpFormat, PTSPFORMAT_PARSE_RESULT* pResult)
{
    if (lpFormat)
    {
        LPCWSTR s = lpFormat->lpString;

        if (s)
        {
            bool segsDefined[PTSP_NUM_SEGS_COUNT] = {};
            bool inQuote = false;
            bool isLiteral = false;
            BOOL definedAny = FALSE;

            PTSPFORMAT_PARSE_RESULT result = {};

            auto _ReadLiteral = [&]()
            {
                if (!isLiteral)
                {
                    ++result.cSegments;
                    ++result.cLiterals;
                    isLiteral = true;
                }
            };

            LPCWSTR e = s + lpFormat->cchString;

            while (s < e)
            {
                if (*s == QUOTE)
                {
                    if (s[1] == QUOTE)
                    {
                        _ReadLiteral();
                        ++result.cchLiterals;
                        s += 2;
                        continue;
                    }

                    inQuote = !inQuote;
                    ++s;
                    continue;
                }

                if (!inQuote)
                {
                    DWORD part;

                    if (IsPartValid(*s, part) && !segsDefined[part - PTSPSEG_NUM_MIN])
                    {
                        segsDefined[part - PTSPSEG_NUM_MIN] = true;
                        definedAny = true;
                        if (isLiteral) isLiteral = false;
                        ++result.cSegments;
                        ++s;
                        continue;
                    }
                }

                _ReadLiteral();
                ++result.cchLiterals;
                ++s;
            }

            if (pResult) *pResult = result;
            return definedAny;
        }
    }

    return FALSE;
}

BOOL PlainTimeSpanPick::CreateSegments(LPCNZWSTR lpFormat, LPWSTR lpBufferLiterals, size_t cchBufferLiterals, LPPTSPSEGMENT lpSegments, INT cSegments)
{
    if (lpFormat)
    {
        LPCWSTR s = lpFormat->lpString;

        if (s && lpBufferLiterals && cchBufferLiterals && lpSegments && cSegments > 0)
        {
            bool segsDefined[PTSP_NUM_SEGS_COUNT] = {};
            bool inQuote = false;
            bool isLiteral = false;

            int i = 0;
            LPWSTR p = lpBufferLiterals;
            size_t remain = cchBufferLiterals;
            LPPTSPSEGMENT lit = nullptr;

            auto _ReadLiteral = [&]() -> bool
            {
                if (!isLiteral)
                {
                    if (i < cSegments)
                    {
                        lit = &lpSegments[i];
                        CLEARMEM(lit);
                        lit->dwType = PTSPSEG_LITERAL;
                        lit->lpText = p;
                        isLiteral = true;
                        ++i;
                        return true;
                    }

                    return false;
                }

                return true;
            };

            LPCWSTR e = s + lpFormat->cchString;

            while (s < e)
            {
                if (*s == QUOTE)
                {
                    if (s[1] == QUOTE)
                    {
                        if (!_ReadLiteral())
                        {
                            return FALSE;
                        }

                        if (p && remain > 0)
                        {
                            *(p++) = QUOTE;
                            --remain;
                        }

                        if (lit)
                        {
                            ++lit->cchText;
                        }

                        s += 2;
                        continue;
                    }

                    inQuote = !inQuote;
                    ++s;
                    continue;
                }

                if (!inQuote)
                {
                    DWORD part;

                    if (IsPartValid(*s, part) && !segsDefined[part - PTSPSEG_NUM_MIN])
                    {
                        segsDefined[part - PTSPSEG_NUM_MIN] = true;

                        if (isLiteral && lit)
                        {
                            if (p && remain > 0)
                            {
                                *(p++) = L'\0';
                                --remain;
                            }

                            lit = nullptr;
                            isLiteral = false;
                        }

                        if (i >= cSegments)
                        {
                            return FALSE;
                        }

                        LPPTSPSEGMENT num = &lpSegments[i];
                        CLEARMEM(num);
                        num->dwType = part;
                        ++i;
                        ++s;
                        continue;
                    }
                }

                if (!_ReadLiteral())
                {
                    return FALSE;
                }

                if (p && remain > 0)
                {
                    *(p++) = *s;
                    --remain;
                }

                if (lit)
                {
                    ++lit->cchText;
                }

                ++s;
            }

            if (isLiteral && lit)
            {
                if (p && remain > 0)
                {
                    *(p++) = L'\0';
                    --remain;
                }
            }

            return i == cSegments;
        }
    }

    return FALSE;
}

bool PlainTimeSpanPick::IsPartValid(WCHAR c, DWORD& part)
{
    switch (c)
    {
        case L'd':
            part = PTSPSEG_DAYS;
            return true;
        case L'h':
            part = PTSPSEG_HOURS;
            return true;
        case L'm':
            part = PTSPSEG_MINUTES;
            return true;
        case L's':
            part = PTSPSEG_SECONDS;
            return true;
    }

    return false;
}

LONGLONG PlainTimeSpanPick::GetTicksByPart(DWORD part)
{
    switch (part)
    {
        CASE(PTSPSEG_DAYS, TICKS_PER_DAY);
        CASE(PTSPSEG_HOURS, TICKS_PER_HOUR);
        CASE(PTSPSEG_MINUTES, TICKS_PER_MINUTE);
        CASE(PTSPSEG_SECONDS, TICKS_PER_SECOND);
    }

    return 0LL;
}

void PlainTimeSpanPick::UpdateSegmentValue(TIMESPAN tsValue)
{
    TIMESPAN ts = std::clamp(tsValue, TIMESPAN_ZERO, m_tsValueMax);
    m_tsValue = ts;

    if (m_lpSegments)
    {
        ApplySegmentValue(m_lpSegments, m_cSegments, ts);
        UpdateSegmentMaxValue();
        UpdateValue();
    }
}

void PlainTimeSpanPick::UpdateSegmentMaxValue()
{
    if (m_lpSegments)
    {
        LONGLONG remain = m_tsValueMax;
        if (remain < 0) remain = 0;

        for (DWORD part = PTSPSEG_NUM_MIN; part <= PTSPSEG_NUM_MAX; ++part)
        {
            LPPTSPSEGMENT seg = FindSegmentByPart(part);
            LONGLONG ticks = GetTicksByPart(part);
            
            if (ticks > 0)
            {
                LONGLONG nMax = GetNaturalMax(part, m_tsValueMax);
                LONGLONG eMax = remain / ticks;
                seg->nValueMax = std::clamp(eMax, 0LL, nMax);
                seg->nValue = std::clamp(seg->nValue, 0LL, seg->nValueMax);
                remain -= seg->nValue * ticks;
                if (remain < 0) remain = 0;
            }
        }
    }
}

LONGLONG PlainTimeSpanPick::GetNaturalMax(DWORD part, TIMESPAN tsMax)
{
    switch (part)
    {
        CASE(PTSPSEG_DAYS, tsMax / TICKS_PER_DAY);
        CASE(PTSPSEG_HOURS, 23);
        CASE(PTSPSEG_MINUTES, 59);
        CASE(PTSPSEG_SECONDS, 59);
    }

    return 0LL;
}

void PlainTimeSpanPick::UpdateValue()
{
    if (m_lpSegments)
    {
        TIMESPAN value = 0LL;

        for (DWORD part = PTSPSEG_NUM_MIN; part <= PTSPSEG_NUM_MAX; ++part)
        {
            LPPTSPSEGMENT seg = FindSegmentByPart(part);

            if (seg)
            {
                value += seg->nValue * GetTicksByPart(part);
            }
        }

        m_tsValue = value;
    }
}

void PlainTimeSpanPick::UpdateMaxValue(TIMESPAN tsMax)
{
    m_tsValueMax = std::clamp(tsMax, TIMESPAN_ZERO, TIMESPAN_MAX);
    UpdateSegmentValue(m_tsValue);
}

void PlainTimeSpanPick::ApplySegmentValue(LPPTSPSEGMENT lpSegments, INT cSegments, TIMESPAN tsValue)
{
    if (lpSegments)
    {
        LONGLONG remain = tsValue;

        for (DWORD part = PTSPSEG_NUM_MIN; part <= PTSPSEG_NUM_MAX; ++part)
        {
            LPPTSPSEGMENT seg = FindSegmentByPartInternal(lpSegments, cSegments, part);

            if (seg)
            {
                LONGLONG ticks = GetTicksByPart(part);
                seg->nValue = remain / ticks;
                remain %= ticks;
            }
        }
    }
}

LPPTSPSEGMENT PlainTimeSpanPick::FindSegmentByPart(DWORD part) const
{
    return FindSegmentByPartInternal(m_lpSegments, m_cSegments, part);
}

LPPTSPSEGMENT PlainTimeSpanPick::FindSegmentByPartInternal(LPPTSPSEGMENT lpSegments, INT cSegments, DWORD part)
{
    if (lpSegments)
    {
        for (int i = 0; i < cSegments; ++i)
        {
            LPPTSPSEGMENT current = &lpSegments[i];

            if (current->dwType == part)
            {
                return current;
            }
        }
    }

    return nullptr;
}

HANDLE PlainTimeSpanPick::CreateFormat(LPCWSTR pszFormat)
{
    CNZWSTR format = {};
    LoadFormat(format, pszFormat);
    PTSPFORMAT_PARSE_RESULT result;

    if (ParseFormat(&format, &result) && result.cSegments > 0)
    {
        size_t cchBufferLiterals = result.cchLiterals + result.cLiterals;

        if (cchBufferLiterals)
        {
            INT cSegments = result.cSegments;
            LPWSTR literals = HEAPALLOC_M(WCHAR, cchBufferLiterals);
            LPPTSPSEGMENT segments = HEAPALLOC_M(PTSPSEGMENT, cSegments);

            if (literals && segments
                && CreateSegments(&format, literals, cchBufferLiterals, segments, cSegments))
            {
                LPPTSPFORMAT lpFormat = HEAPALLOC(PTSPFORMAT);

                if (lpFormat)
                {
                    lpFormat->lpLiterals = literals;
                    lpFormat->lpSegments = segments;
                    lpFormat->cchLiterals = cchBufferLiterals;
                    lpFormat->cSegments = cSegments;
                    return CastToP(HANDLE, lpFormat);
                }
            }

            HEAPFREE(literals);
            HEAPFREE(segments);
        }
    }

    return nullptr;
}

BOOL PlainTimeSpanPick::Format(HANDLE hFormat, LPTIMESPAN lptsValue, LPWSTR lpBuffer, LPINT lpcchBuffer)
{
    LPPTSPFORMAT lpFormat = CastToP(LPPTSPFORMAT, hFormat);

    if (lpFormat)
    {
        TIMESPAN ts = std::clamp(*lptsValue, TIMESPAN_ZERO, TIMESPAN_MAX);
        ApplySegmentValue(lpFormat->lpSegments, lpFormat->cSegments, ts);

        if (lpcchBuffer)
        {
            size_t count = BuildDisplayTextInternal(lpFormat->lpSegments, lpFormat->cSegments, lpBuffer, *lpcchBuffer);

            if (!lpBuffer)
            {
                *lpcchBuffer = CastToS(INT, count);
            }

            return TRUE;
        }
    }

    return FALSE;
}

BOOL PlainTimeSpanPick::FreeFormatMemory(HANDLE hFormat)
{
    LPPTSPFORMAT lpFormat = CastToP(LPPTSPFORMAT, hFormat);

    if (lpFormat
        && HEAPFREE(lpFormat->lpLiterals) && HEAPFREE(lpFormat->lpSegments) && HEAPFREE(lpFormat))
    {
        return TRUE;
    }

    return FALSE;
}
