#include "pch.h"
#include "BrushTable.h"

HBRUSH BrushTable::GetBrush(COLORREF color)
{
    HBRUSH hbr = nullptr;
    bool bFound = m_map.Lookup(color, hbr);
    if (bFound) return hbr;
    hbr = CreateSolidBrush(color);
    if (hbr) m_map.SetAt(color, hbr);
    return hbr;
}

void BrushTable::Clear()
{
    POSITION p = m_map.GetStartPosition();

    while (p)
    {
        HBRUSH hbr = m_map.GetNextValue(p);
        if (hbr) DeleteObject(hbr);
    }

    m_map.RemoveAll();
}

BrushTable::~BrushTable()
{
    Clear();
}
