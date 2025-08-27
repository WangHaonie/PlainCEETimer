using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32User
{
    /*

    获取当前系统会话用户名 参考：

    How do I get the current username in .NET using C#? - Stack Overflow
    https://stackoverflow.com/a/60952084

    获取当前进程所有者 参考：

    How do I get the current username in .NET using C#? - Stack Overflow
    https://stackoverflow.com/a/1240379

    */

    public static string ProcessOwner => ProcOwner;
    public static string LogonUser => LogonUserName;
    public static bool NotElevated { get; }

    private const int WTSInfoClass_WTSUserName = 5;
    private const int WTSInfoClass_WTSDomainName = 7;

    private static readonly string ProcOwner;
    private static readonly string LogonUserName;

    static Win32User()
    {
        ProcOwner = WindowsIdentity.GetCurrent().Name;
        LogonUserName = GetCurrentLogonUserName();
        NotElevated = ProcOwner.Equals(LogonUserName, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetCurrentLogonUserName()
    {
        var username = "";
        var sid = WTSGetActiveConsoleSessionId();

        if (WTSQuerySessionInformation(IntPtr.Zero, sid, WTSInfoClass_WTSDomainName, out IntPtr buffer, out int strLen) && strLen > 1)
        {
            username = Marshal.PtrToStringAnsi(buffer);
            WTSFreeMemory(buffer);

            if (WTSQuerySessionInformation(IntPtr.Zero, sid, WTSInfoClass_WTSUserName, out buffer, out strLen) && strLen > 1)
            {
                username += @"\" + Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
            }
        }

        return username;
    }

    [DllImport(App.Kernel32Dll)]
    private static extern int WTSGetActiveConsoleSessionId();

    [DllImport("wtsapi32.dll")]
    private static extern void WTSFreeMemory(IntPtr pMemory);

    [DllImport("wtsapi32.dll")]
    private static extern BOOL WTSQuerySessionInformation(IntPtr hServer, int SessionId, int WTSInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
}