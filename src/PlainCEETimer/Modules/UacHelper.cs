using System;
using Microsoft.Win32;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

/*

获取 UAC 通知等级 参考：

How to change UAC settings in Windows 10
https://winaero.com/how-to-change-uac-settings-in-windows-10/

UAC Levels & Windows User Account Control Settings - CyberFOX
https://support.cyberfox.com/settings/360048585292-UAC-Levels-Windows-User-Account-Control-Settings

*/

public static class UacHelper
{
    public static bool IsAdmin { get; private set; }
    public static bool IsUacDisabled => Level <= UacNotifyLevel.Disabled;

    private static readonly UacNotifyLevel Level;
    private const string AcExe = "net";
    private const string AcArg = "session";

    static UacHelper()
    {
        Level = GetNotifyLevel();
    }

    public static void PrintReport()
    {
        CheckAdmin();

        var ard = GetUserAdminDesc(false, out var ar);
        var ardL = GetUserAdminDesc(true, out var arL);

        ConsoleHelper.Instance
            .WriteLine("\t======== 检测结果 ========")
            .WriteLine()
            .WriteLine("当前系统", ConsoleColor.Cyan)
                .Write("\tUAC 状态\t").Write(Level, Level.ToConsoleColor()).Write(" (").Write((int)Level).WriteLine("/4)")
                .Write("\t用户名\t\t").WriteLine(Win32User.LogonUser, ConsoleColor.White)
                .Write("\t管理员权限\t").WriteLine(ardL, arL.ToConsoleColor())
            .WriteLine()
            .WriteLine("当前进程", ConsoleColor.Cyan)
                .Write("\t所有者\t\t").WriteLine(Win32User.ProcessOwner, ConsoleColor.White)
                .Write("\t管理员权限\t").Write(ard, ar.ToConsoleColor())
            .WriteLine();
    }

    public static void CheckAdmin()
    {
        IsAdmin = ProcessHelper.Run(AcExe, AcArg, getExitCode: true) == 0;
    }

    private static UacNotifyLevel GetNotifyLevel()
    {
        using var reg = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", rootKey: RegistryHive.LocalMachine);

        return (reg.Get("EnableLUA", 0), reg.Get("ConsentPromptBehaviorAdmin", 0), reg.Get("PromptOnSecureDesktop", 0)) switch
        {
            (1, 2, 1) => UacNotifyLevel.AlwaysDimmed,
            (1, 5, 1) => UacNotifyLevel.AppsOnlyDimmed,
            (1, 5, 0) => UacNotifyLevel.AppsOnlyNoDimmed,
            (1, 0, 0) => UacNotifyLevel.NeverNotify,
            (0, _, _) => UacNotifyLevel.Disabled,
            _ => UacNotifyLevel.Unknown
        };
    }

    private static string GetUserAdminDesc(bool logon, out AdminRights rights)
    {
        rights = GetUserAdmin(logon);

        return rights switch
        {
            AdminRights.Yes => "有",
            AdminRights.No => "无",
            _ => "未知"
        };
    }

    private static AdminRights GetUserAdmin(bool logon)
    {
        if (!logon)
        {
            return IsAdmin ? AdminRights.Yes : AdminRights.No;
        }

        if (ProcessHelper.RunAsLogonUser(AcExe, AcArg, out var code))
        {
            return code == 0 ? AdminRights.Yes : AdminRights.No;
        }

        return AdminRights.Unknown;
    }
}
