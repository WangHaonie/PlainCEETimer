#include "pch.h"
#include "Win32.h"
#include <wincon.h>

HWND AllocConsoleForApp(PHANDLE phStdIn, PHANDLE phStdOut, PHANDLE phStdErr)
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

    return GetConsoleWindow();
}
