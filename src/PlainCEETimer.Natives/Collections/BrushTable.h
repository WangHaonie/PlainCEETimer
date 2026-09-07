#pragma once

#include <atlcoll.h>
#include <Windows.h>

class BrushTable
{
public:
	BrushTable() = default;

	HBRUSH GetBrush(COLORREF color);
	void Clear();

	BrushTable(const BrushTable&) = delete;
	BrushTable& operator=(const BrushTable&) = delete;

	~BrushTable();

private:
	CAtlMap<COLORREF, HBRUSH> m_map;
};
