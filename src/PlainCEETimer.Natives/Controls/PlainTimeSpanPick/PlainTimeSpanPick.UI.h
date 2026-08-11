#pragma once

#include "PlainTimeSpanPick.h"
#include "PlainTimeSpanPick.Core.h"

#define get_SelectedIndex()         m_iSelected
#define set_SelectedIndex(value)    get_SelectedIndex() = value

#define MoveToNumericSegment(current, step) \
            INT next = FindNextNumericPart(current, step); \
            if (next >= 0) set_SelectedIndex(next);

#define GetSegmentAt(index)         m_lpSegments[index]
#define get_SelectedSegment()       GetSegmentAt(get_SelectedIndex())
#define ClearStringBuffer(b)        if (b) *b = L'\0';
#define ClearEditBuffer()           ClearStringBuffer(m_lpBufferEdit)
#define Invalidate()                InvalidateRect(hWnd, nullptr, FALSE)
#define IsNumericPart(p)            (p >= PTSPSEG_NUM_MIN && p <= PTSPSEG_NUM_MAX)
