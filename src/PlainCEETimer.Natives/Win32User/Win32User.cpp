#include "pch.h"
#include "Win32User.h"
#include <string>

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
