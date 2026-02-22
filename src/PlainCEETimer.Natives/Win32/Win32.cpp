#include "pch.h"
#include "Win32.h"
#include <wincon.h>

HWND AllocConsoleForApp(BOOL fRefresh, PHANDLE phStdIn, PHANDLE phStdOut, PHANDLE phStdErr)
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

    if (fRefresh)
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
