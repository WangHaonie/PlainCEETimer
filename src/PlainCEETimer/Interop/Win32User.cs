using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

[SuppressUnmanagedCodeSecurity]
internal static class Win32User
{
    /*

    获取当前系统会话用户名 参考：

    How do I get the current username in .NET using C#? - Stack Overflow
    https://stackoverflow.com/a/60952084

    获取当前进程所有者 参考：

    How do I get the current username in .NET using C#? - Stack Overflow
    https://stackoverflow.com/a/1240379

    */

    public static string ProcessOwner { get; }

    public static string LogonUser { get; }

    public static bool NotImpersonal { get; }

    static Win32User()
    {
        var po = WindowsIdentity.GetCurrent().Name;
        var lu = PnGetLogonUserName();
        ProcessOwner = po;
        LogonUser = lu;
        NotImpersonal = po.Equals(lu, StringComparison.OrdinalIgnoreCase);
    }

    [DllImport(App.NativesDll, EntryPoint = "#52", CharSet = CharSet.Unicode)]
    private static extern string PnGetLogonUserName();

    [DllImport(App.NativesDll, EntryPoint = "#53", CharSet = CharSet.Unicode)]
    public static extern bool PnRunProcessAsLogonUser(string path, string args, out int lpExitCode);
}