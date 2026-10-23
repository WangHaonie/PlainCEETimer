﻿#pragma once
#include <Windows.h>

#define IMAGE_ARROWUP_COUNT             7

inline constexpr BYTE IMAGE_ARROWUP_96[] =
{
    0b00100000, 0b00000000,
    0b01110000, 0b00000000,
    0b11111000, 0b00000000,
};

inline constexpr BYTE IMAGE_ARROWUP_120[] =
{
    0b00010000, 0b00000000,
    0b00111000, 0b00000000,
    0b01111100, 0b00000000,
    0b11111110, 0b00000000,
};

inline constexpr BYTE IMAGE_ARROWUP_144[] =
{
    0b00001000, 0b00000000,
    0b00011100, 0b00000000,
    0b00111110, 0b00000000,
    0b01111111, 0b00000000,
    0b11111111, 0b10000000,
};

inline constexpr BYTE IMAGE_ARROWUP_192[] =
{
    0b00001100, 0b00000000,
    0b00011110, 0b00000000,
    0b00111111, 0b00000000,
    0b01111111, 0b10000000,
    0b11111111, 0b11000000,
};

inline constexpr BYTE IMAGE_ARROWUP_240[] =
{
    0b00000110, 0b00000000,
    0b00001111, 0b00000000,
    0b00011111, 0b10000000,
    0b00111111, 0b11000000,
    0b01111111, 0b11100000,
    0b11111111, 0b11110000,
};

inline constexpr BYTE IMAGE_ARROWUP_288[] =
{
    0b00000001, 0b00000000,
    0b00000011, 0b10000000,
    0b00000111, 0b11000000,
    0b00001111, 0b11100000,
    0b00011111, 0b11110000,
    0b00111111, 0b11111000,
    0b01111111, 0b11111100,
    0b11111111, 0b11111110,
    0b11111111, 0b11111110,
};

inline constexpr BYTE IMAGE_ARROWUP_384[] =
{
    0b00000000, 0b01100000, 0b00000000, 0b00000000,
    0b00000000, 0b11110000, 0b00000000, 0b00000000,
    0b00000001, 0b11111000, 0b00000000, 0b00000000,
    0b00000011, 0b11111100, 0b00000000, 0b00000000,
    0b00000111, 0b11111110, 0b00000000, 0b00000000,
    0b00001111, 0b11111111, 0b00000000, 0b00000000,
    0b00011111, 0b11111111, 0b10000000, 0b00000000,
    0b00111111, 0b11111111, 0b11000000, 0b00000000,
    0b01111111, 0b11111111, 0b11100000, 0b00000000,
    0b11111111, 0b11111111, 0b11110000, 0b00000000,
    0b11111111, 0b11111111, 0b11110000, 0b00000000,
};

struct ARROWUP_INFO
{
    DWORD dpi;
    const BYTE* pData;
    
    union
    {
        SIZE size;
        struct
        {
            LONG cx;
            LONG cy;
        };
    };
};

inline constexpr ARROWUP_INFO IMAGE_ARROWUP_TABLE[IMAGE_ARROWUP_COUNT] =
{
    { 96, IMAGE_ARROWUP_96, 5, 3 },
    { 120, IMAGE_ARROWUP_120, 7, 4 },
    { 144, IMAGE_ARROWUP_144, 9, 5 },
    { 192, IMAGE_ARROWUP_192, 10, 5 },
    { 240, IMAGE_ARROWUP_240, 12, 6 },
    { 288, IMAGE_ARROWUP_288, 15, 9 },
    { 384, IMAGE_ARROWUP_384, 20, 11 },
};

inline HBITMAP IMAGE_ARROWUP_CACHE[IMAGE_ARROWUP_COUNT] = {};
inline HBITMAP IMAGE_ARROWRIGHT_CACHE[IMAGE_ARROWUP_COUNT] = {};

inline bool GetArrowUpImage(DWORD dpi, HBITMAP* phbm, SIZE* psz)
{
    if (!phbm || !psz)
    {
        return false;
    }

    int index = IMAGE_ARROWUP_COUNT - 1;

    for (int i = 0; i < IMAGE_ARROWUP_COUNT; i++)
    {
        if (dpi <= IMAGE_ARROWUP_TABLE[i].dpi)
        {
            index = i;
            break;
        }
    }

    auto& info = IMAGE_ARROWUP_TABLE[index];
    auto& result = IMAGE_ARROWUP_CACHE[index];

    if (!result)
    {
        result = CreateBitmap(info.cx, info.cy, 1, 1, info.pData);
    }

    *phbm = result;
    *psz = info.size;
    return result != nullptr;
}

inline bool GetArrowRightImage(DWORD dpi, HBITMAP* phbm, SIZE* psz)
{
    if (!phbm || !psz)
    {
        return false;
    }

    int index = IMAGE_ARROWUP_COUNT - 1;

    for (int i = 0; i < IMAGE_ARROWUP_COUNT; i++)
    {
        if (dpi <= IMAGE_ARROWUP_TABLE[i].dpi)
        {
            index = i;
            break;
        }
    }

    auto& info = IMAGE_ARROWUP_TABLE[index];
    auto& result = IMAGE_ARROWRIGHT_CACHE[index];

    if (!result)
    {
        LONG cx = info.cx;
        LONG cy = info.cy;
        LONG dcx = cy;
        LONG dcy = cx;
        int cbcx = ((cx + 15) / 16) * 2;
        int cbdcx = ((dcx + 15) / 16) * 2;
        int len = cbdcx * dcy;

        BYTE* buf = new BYTE[len];
        ZeroMemory(buf, len);

        for (LONG dy = 0; dy < dcy; dy++)
        {
            for (LONG dx = 0; dx < dcx; dx++)
            {
                LONG sx = dy;
                LONG sy = cy - 1 - dx;

                if ((info.pData[sy * cbcx + (sx >> 3)] >> (7 - (sx & 7))) & 1)
                {
                    buf[dy * cbdcx + (dx >> 3)] |= (BYTE)(0x80 >> (dx & 7));
                }
            }
        }

        result = CreateBitmap(dcx, dcy, 1, 1, buf);
    }

    *phbm = result;
    psz->cx = info.cy;
    psz->cy = info.cx;
    return result != nullptr;
}
