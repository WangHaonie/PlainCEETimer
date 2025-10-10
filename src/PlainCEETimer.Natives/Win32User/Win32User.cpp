#include "pch.h"
#include "Win32User.h"
#include <string>
#include <TlHelp32.h>

LPCWSTR GetLogonUserName()
{
    std::wstring tmp = L"<未知用户名>";
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

    return CoTaskStrAllocW(tmp.c_str());
}

BOOL RunProcessAsLogonUser(LPCWSTR path, LPCWSTR args, LPDWORD lpExitCode)
{
    BOOL result = FALSE;

    if (IsStringNullOrEmptyW(path) && IsStringNullOrEmptyW(args))
    {
        return result;
    }

    DWORD activeSid = WTSGetActiveConsoleSessionId();
    HANDLE hToken = nullptr;

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
                if (!StringStartsWithW(pe32.szExeFile, L"taskh")) // 匹配 taskhost*.exe 进程
                {
                    continue;
                }

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

    DWORD exitCode = 0;

    if (result)
    {
        HANDLE hTokenPrimary = nullptr;

        if (DuplicateTokenEx(hToken, MAXIMUM_ALLOWED, nullptr, SecurityIdentification, TokenPrimary, &hTokenPrimary))
        {
            STARTUPINFO si = { sizeof(si) };
            PROCESS_INFORMATION pi = {};
            std::wstring cmd = path;
            cmd += L" ";
            cmd += args;

            if (CreateProcessWithTokenW(hTokenPrimary, 0, nullptr, &cmd[0], CREATE_NO_WINDOW, nullptr, nullptr, &si, &pi))
            {
                if (lpExitCode)
                {
                    WaitForSingleObject(pi.hProcess, INFINITE);

                    if (GetExitCodeProcess(pi.hProcess, &exitCode))
                    {
                        result = TRUE;
                    }
                }
                else
                {
                    result = TRUE;
                }
            }

            CloseHandle(hTokenPrimary);
        }

        CloseHandle(hToken);
    }

    if (lpExitCode)
    {
        *lpExitCode = exitCode;
    }

    return result;
}
