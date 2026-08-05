#include "pch.h"
#include "User.h"
#include "Utils.h"
#include <strsafe.h>
#include <TlHelp32.h>
#include <Windows.h>
#include <WtsApi32.h>

static LPCWSTR BuildUserName(BOOL hasDomain, LPWSTR bufferDomain, DWORD cbDomain, BOOL hasUser, LPWSTR bufferUser, DWORD cbUser)
{
    size_t count = 0;

    if (hasUser && cbUser > sizeof(WCHAR))
    {
        count += (cbUser / sizeof(WCHAR));

        if (hasDomain && cbDomain > sizeof(WCHAR))
        {
            count += (cbDomain / sizeof(WCHAR));
            // 多出来的 L'\0' 恰好可以用来填 L'\\'，故不 += 1
        }
        else
        {
            hasDomain = FALSE;
        }
    }

    if (count)
    {
        LPWSTR buffer = CoTaskStrAllocW(count, nullptr);

        if (buffer && hasDomain)
        {
            LPWSTR current = buffer;
            StringCchCopyEx(current, count, bufferDomain, &current, &count, STRSAFE_DEFAULT);
            StringCchCopyEx(current, count, L"\\", &current, &count, STRSAFE_DEFAULT);
            StringCchCopyEx(current, count, bufferUser, &current, &count, STRSAFE_DEFAULT);
        }

        return buffer;
    }

    return CoTaskStrDupW(L"未知用户名");
}

static LPWSTR BuildCommandLine(LPWSTR application, LPWSTR args)
{
    if (WString_IsNullOrEmpty(args))
    {
        return application;
    }

    if (WString_IsNullOrEmpty(application))
    {
        return args;
    }

    size_t count = 0;
    count += lstrlen(application);
    count += lstrlen(args);
    count += 2; // L' ' 和 L'\0'

    LPWSTR buffer = CastToP(LPWSTR, HeapAllocEx(count * sizeof(WCHAR)));
    
    if (buffer)
    {
        LPWSTR current = buffer;
        StringCchCopyEx(current, count, application, &current, &count, STRSAFE_DEFAULT);
        StringCchCopyEx(current, count, L" ", &current, &count, STRSAFE_DEFAULT);
        StringCchCopyEx(current, count, args, &current, &count, STRSAFE_DEFAULT);
    }

    return buffer;
}

LPCWSTR NATIVESAPI GetLogonUserName()
{
    DWORD cbDomain;
    DWORD cbUser;
    LPWSTR bufferDomain = nullptr;
    LPWSTR bufferUser = nullptr;

    DWORD sid = WTSGetActiveConsoleSessionId();
    BOOL hasDomain = WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, sid, WTSDomainName, &bufferDomain, &cbDomain);
    BOOL hasUser = WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, sid, WTSUserName, &bufferUser, &cbUser);

    LPCWSTR result = BuildUserName(hasDomain, bufferDomain, cbDomain, hasUser, bufferUser, cbUser);
    if (bufferDomain) WTSFreeMemory(bufferDomain);
    if (bufferUser) WTSFreeMemory(bufferUser);

    return result;
}

BOOL NATIVESAPI RunProcessAsLogonUser(LPWSTR path, LPWSTR args, LPDWORD lpExitCode)
{
    if (WString_IsNullOrEmpty(path) && WString_IsNullOrEmpty(args))
    {
        return FALSE;
    }

    BOOL result = FALSE;
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
                if (WString_StartsWith(pe32.szExeFile, L"taskh")) // 匹配 taskhost*.exe 进程
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
            }
            while (Process32Next(hSnapshot, &pe32));
        }
    }

    DWORD exitCode = -1;

    if (result)
    {
        HANDLE hTokenPrimary = nullptr;

        if (DuplicateTokenEx(hToken, MAXIMUM_ALLOWED, nullptr, SecurityIdentification, TokenPrimary, &hTokenPrimary))
        {
            STARTUPINFO si = { sizeof(si) };
            PROCESS_INFORMATION pi = {};
            LPWSTR cli = BuildCommandLine(path, args);

            if (CreateProcessWithTokenW(hTokenPrimary, 0, nullptr, cli, CREATE_NO_WINDOW, nullptr, nullptr, &si, &pi))
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

            HeapFreeEx(CastToP(LPVOID*, &cli));
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
