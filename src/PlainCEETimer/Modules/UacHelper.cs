using Microsoft.Win32;
using PlainCEETimer.Interop;
using PlainCEETimer.UI;

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

    public static void PopupReport()
    {
        CheckAdmin();

        AppMessageBox.Instance.Info(
            $"""
            检测结果:
                                
            当前系统
                UAC 状态: {Level} ({(int)Level}/4)
                用户名: {Win32User.LogonUser}
                管理员权限: {GetUserAdmin(true)}

            当前进程
                所有者: {Win32User.ProcessOwner}
                管理员权限: {GetUserAdmin(false)}
            """);
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

    private static string GetUserAdmin(bool logon)
    {
        if (!logon)
        {
            return IsAdmin ? "有" : "无";
        }

        if (ProcessHelper.RunAsLogonUser(AcExe, AcArg, out var code))
        {
            return code == 0 ? "有" : "无";
        }

        return "未知";
    }
}
