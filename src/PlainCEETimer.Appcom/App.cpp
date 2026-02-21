#define EXE L"PlainCEETimer.exe"

#include <windows.h>

int main()
{
    STARTUPINFO si;
    PROCESS_INFORMATION pi;
    GetStartupInfo(&si);

    if (CreateProcess(EXE, GetCommandLine(), nullptr, nullptr, TRUE, 0, nullptr, nullptr, &si, &pi))
    {
        WaitForSingleObject(pi.hProcess, INFINITE);
        DWORD exitCode = 0;
        GetExitCodeProcess(pi.hProcess, &exitCode);
        CloseHandle(pi.hProcess);
        CloseHandle(pi.hThread);
        return exitCode;
    }

    return GetLastError();
}