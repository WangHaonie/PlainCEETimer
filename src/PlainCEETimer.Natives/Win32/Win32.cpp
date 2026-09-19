#include "pch.h"
#include "..\Utils.h"
#include "Win32.h"
#include <TlHelp32.h>
#include <Windows.h>
#include <float.h>

static void PnKillProcessTreeCore(DWORD dwProcessId)
{
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    PROCESSENTRY32 pe32 = { sizeof(pe32) };

    if (Process32First(hSnapshot, &pe32))
    {
        do
        {
            if (pe32.th32ParentProcessID == dwProcessId)
            {
                PnKillProcessTreeCore(pe32.th32ProcessID);
            }
        }
        while (Process32Next(hSnapshot, &pe32));
    }

    HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, FALSE, dwProcessId);

    if (hProcess)
    {
        TerminateProcess(hProcess, 0);
        CloseHandle(hProcess);
    }
}

static double GetColorDistance(COLORREF color1, COLORREF color2)
{
    long r1 = GetRValue(color1), g1 = GetGValue(color1), b1 = GetBValue(color1);
    long r2 = GetRValue(color2), g2 = GetGValue(color2), b2 = GetBValue(color2);

    long rDiff = r1 - r2;
    long gDiff = g1 - g2;
    long bDiff = b1 - b2;
    long rMean = (r1 + r2) / 2;

    if (rMean < 128)
        return (((512 + rMean) * rDiff * rDiff) >> 8) + 4 * gDiff * gDiff + (((767 - rMean) * bDiff * bDiff) >> 8);
    else
        return (((767 - rMean) * rDiff * rDiff) >> 8) + 4 * gDiff * gDiff + (((512 + rMean) * bDiff * bDiff) >> 8);
}

static int FindNearestConsoleColor(COLORREF color, LPCOLORREF lpColors)
{
    if (lpColors)
    {
        double dmin = DBL_MAX;
        int result = 0;

        for (int i = 0; i < 16; ++i)
        {
            double d = GetColorDistance(color, lpColors[i]);

            if (d < dmin)
            {
                dmin = d;
                result = i;
            }
        }

        return result;
    }

    return 0;
}

HWND NATIVESAPI PnAllocConsole(BOOL bRefresh, PHANDLE phStdIn, PHANDLE phStdOut, PHANDLE phStdErr)
{
    BOOL attached = AttachConsole(ATTACH_PARENT_PROCESS);
    if (!attached) AllocConsole();

    HANDLE hStdIn = GetStdHandle(STD_INPUT_HANDLE);
    HANDLE hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
    HANDLE hStdErr = GetStdHandle(STD_ERROR_HANDLE);

    SetStdHandle(STD_INPUT_HANDLE, hStdIn);
    SetStdHandle(STD_OUTPUT_HANDLE, hStdOut);
    SetStdHandle(STD_ERROR_HANDLE, hStdErr);

    if (phStdIn) *phStdIn = hStdIn;
    if (phStdOut) *phStdOut = hStdOut;
    if (phStdErr) *phStdErr = hStdErr;

    if (bRefresh)
    {
        INPUT_RECORD irs[2] = {};

        auto& ir0 = irs[0];
        auto& ke0 = ir0.Event.KeyEvent;
        ir0.EventType = KEY_EVENT;
        ke0.bKeyDown = 1;
        ke0.wRepeatCount = 1;
        ke0.wVirtualKeyCode = VK_RETURN;
        ke0.uChar.UnicodeChar = L'\r';

        auto& ir1 = irs[1];
        auto& ke1 = ir1.Event.KeyEvent;
        ir1.EventType = KEY_EVENT;
        ke1.bKeyDown = 0;
        ke1.wRepeatCount = 1;
        ke1.wVirtualKeyCode = VK_RETURN;
        ke1.uChar.UnicodeChar = L'\r';

        DWORD dw = 0;
        WriteConsoleInput(hStdIn, irs, 2, &dw);
    }

    return GetConsoleWindow();
}

void NATIVESAPI PnKillProcessTree(DWORD dwProcessId)
{
    PnKillProcessTreeCore(dwProcessId);
}

int NATIVESAPI PnLoadStringInternal(UINT uID, LPWSTR* ppBuffer)
{
    static HMODULE hModule = GetModuleHandle(LIBRARYNAME);
    return LoadStringExW(hModule, uID, ppBuffer);
}

BOOL NATIVESAPI PnGetConsoleColor(HANDLE hConsoleHandle, COLORREF color, LPDWORD lpdwConsoleColor)
{
    if (lpdwConsoleColor)
    {
        HANDLE hOut = hConsoleHandle;

        if (IS_HANDLE_VALID(hOut))
        {
            static HANDLE last = nullptr;
            static LPCOLORREF colors = nullptr;

            if (hOut != last)
            {
                static CONSOLE_SCREEN_BUFFER_INFOEX csbix = { sizeof(csbix) };
                colors = GetConsoleScreenBufferInfoEx(hOut, &csbix) ? csbix.ColorTable : nullptr;
                last = hOut;
            }

            if (colors)
            {
                *lpdwConsoleColor = FindNearestConsoleColor(color, colors);
                return TRUE;
            }
        }
    }

    return FALSE;
}

BOOL NATIVESAPI PnSetConsoleMode(HANDLE hConsoleHandle, DWORD dwFlags)
{
    HANDLE hConsole = hConsoleHandle
        ? hConsoleHandle
        : GetStdHandle((dwFlags & 0xF00000) ? STD_OUTPUT_HANDLE : STD_INPUT_HANDLE);

    if (IS_HANDLE_VALID(hConsole))
    {
        DWORD current;
        DWORD dwMode = dwFlags & 0x00FFFF;

        if (GetConsoleMode(hConsole, &current))
        {
            if ((dwFlags & 0x0F0000))
                current |= dwMode;
            else
                current &= ~dwMode;

            return SetConsoleMode(hConsole, current);
        }
    }

    return FALSE;
}

BOOL NATIVESAPI PnBuildAnsiColorString(LPWSTR lpBuffer, DWORD dwCchBuffer, LPDWORD lpData)
{
    if (lpBuffer && dwCchBuffer && lpData)
    {
        COLORREF fore = lpData[1];
        COLORREF back = lpData[2];

        size_t remain = 0;
        HRESULT hr = StringCchPrintfEx(lpBuffer, dwCchBuffer, nullptr, &remain, STRSAFE_DEFAULT, L"\x1b[38;2;%d;%d;%d;48;2;%d;%d;%dm",
            GetRValue(fore), GetGValue(fore), GetBValue(fore), GetRValue(back), GetGValue(back), GetBValue(back));

        if (SUCCEEDED(hr))
        {
            *lpData = dwCchBuffer - CastS(DWORD, remain);
            return TRUE;
        }
    }

    return FALSE;
}

COORD NATIVESAPI PnConsoleGetCursor(HANDLE hConsoleHandle)
{
    if (IS_HANDLE_VALID(hConsoleHandle))
    {
        CONSOLE_SCREEN_BUFFER_INFO csbi;

        if (GetConsoleScreenBufferInfo(hConsoleHandle, &csbi))
        {
            return csbi.dwCursorPosition;
        }
    }

    return COORD{};
}

BOOL NATIVESAPI PnConsoleClear(HANDLE hConsoleHandle, COORD coFrom, COORD coTo)
{
    CONSOLE_SCREEN_BUFFER_INFO csbi;

    if (GetConsoleScreenBufferInfo(hConsoleHandle, &csbi))
    {
        int cx = csbi.dwSize.X;
        int start = coFrom.Y * cx + coFrom.X;
        int end = coTo.Y * cx + coTo.X;

        DWORD length = CastS(DWORD, end - start);

        if (length)
        {
            DWORD count;
            FillConsoleOutputCharacter(hConsoleHandle, L' ', length, coFrom, &count);
            FillConsoleOutputAttribute(hConsoleHandle, csbi.wAttributes, length, coFrom, &count);
            return SetConsoleCursorPosition(hConsoleHandle, coFrom);
        }
    }

    return FALSE;
}
