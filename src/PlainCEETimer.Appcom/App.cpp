#define EXE L"PlainCEETimer.exe"

#include <Windows.h>
#include <Shlwapi.h>

int main()
{
    WCHAR dir[MAX_PATH];
    GetModuleFileName(nullptr, dir, MAX_PATH);
    PathRemoveFileSpec(dir);
    WCHAR exe[MAX_PATH];
    PathCombine(exe, dir, EXE);

    STARTUPINFO si = { sizeof(si) };
    PROCESS_INFORMATION pi = {};
    GetStartupInfo(&si);

    if (CreateProcess(exe, GetCommandLine(), nullptr, nullptr, TRUE, 0, nullptr, nullptr, &si, &pi))
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