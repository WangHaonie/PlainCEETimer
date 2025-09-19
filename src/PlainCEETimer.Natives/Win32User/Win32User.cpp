#include "pch.h"
#include "Win32User.h"
#include <string>
#include <TlHelp32.h>

using namespace std;

LPCWSTR GetLogonUserName()
{
    wstring tmp = L"<未知用户名>";
    LPWSTR buffer = nullptr;
    DWORD length = 0;
    DWORD sid = WTSGetActiveConsoleSessionId();

    if (WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, sid, WTSDomainName, &buffer, &length) && length > 1)
    {
        tmp = buffer;
        WTSFreeMemory(buffer);
        buffer = nullptr;

        if (WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, sid, WTSUserName, &buffer, &length) && length > 1)
        {
            tmp += L"\\";
            tmp += buffer;
            WTSFreeMemory(buffer);
            buffer = nullptr;
        }
    }

    return _wcsdup(tmp.c_str());
}

BOOL RunProcessAsLogonUser(LPCWSTR path, LPCWSTR args, LPDWORD lpExitCode)
{
    DWORD activeSid = WTSGetActiveConsoleSessionId();
    HANDLE hToken = nullptr;
    DWORD exitCode = 0;
    BOOL result = FALSE;

    if (WTSQueryUserToken(activeSid, &hToken))
    {
        result = TRUE;
    }

    if (!result)
    {
        HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
        PROCESSENTRY32 pe32 = { sizeof(pe32) };

        if (Process32First(hSnapshot, &pe32))
        {
            do
            {
                DWORD sid;

                if (ProcessIdToSessionId(pe32.th32ProcessID, &sid) && sid == activeSid)
                {
                    HANDLE hProc = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, FALSE, pe32.th32ProcessID);

                    if (hProc)
                    {
                        if (OpenProcessToken(hProc, TOKEN_QUERY | TOKEN_DUPLICATE, &hToken))
                        {
                            result = TRUE;
                        }

                        CloseHandle(hProc);
                    }

                    break;
                }
            }
            while (Process32Next(hSnapshot, &pe32));
        }
    }

    if (result)
    {
        HANDLE hTokenPrimary = nullptr;

        if (DuplicateTokenEx(hToken, MAXIMUM_ALLOWED, nullptr, SecurityIdentification, TokenPrimary, &hTokenPrimary))
        {
            STARTUPINFO si = { sizeof(si) };
            PROCESS_INFORMATION pi = {};
            wstring cmd = path;
            cmd += L" ";
            cmd += args;

            if (CreateProcessWithTokenW(hTokenPrimary, 0, nullptr, &cmd[0], CREATE_NO_WINDOW, nullptr, nullptr, &si, &pi))
            {
                WaitForSingleObject(pi.hProcess, INFINITE);

                if (GetExitCodeProcess(pi.hProcess, &exitCode))
                {
                    result = TRUE;
                }
            }

            CloseHandle(hTokenPrimary);
        }

        CloseHandle(hToken);
    }

    *lpExitCode = exitCode;
    return result;
}
