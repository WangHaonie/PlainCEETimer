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

    public static string ProcessOwner => processOwner;
    public static string LogonUser => logonUser;
    public static bool NotImpersonalOrElevated { get; }

    private static readonly string processOwner;
    private static readonly string logonUser;

    static Win32User()
    {
        processOwner = WindowsIdentity.GetCurrent().Name;
        logonUser = GetLogonUserName();
        NotImpersonalOrElevated = processOwner.Equals(logonUser, StringComparison.OrdinalIgnoreCase);
    }

    [DllImport(App.NativesDll, EntryPoint = "#29", CharSet = CharSet.Unicode)]
    public static extern BOOL RunProcessAsLogonUser(string path, string args, out int lpExitCode);

    [DllImport(App.NativesDll, EntryPoint = "#27", CharSet = CharSet.Unicode)]
    private static extern string GetLogonUserName();
}