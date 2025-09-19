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

    private static readonly UacNotifyLevel Level;
    private static readonly bool IsUACDisabled;
    private const string AcExe = "net";
    private const string AcArg = "session";

    static UacHelper()
    {
        Level = GetNotifyLevel();
        IsUACDisabled = Level <= UacNotifyLevel.Never;
    }

    public static bool EnsureUAC(AppMessageBox mx)
    {
        if (!IsAdmin)
        {
            mx.Warn("检测到当前用户不具有管理员权限，将尝试自动提权。\n\n若弹出 UAC 对话框，请点击 是。");

            if (IsUACDisabled)
            {
                mx.Error("授权失败! 当前系统已禁用 UAC。");
                return false;
            }
        }

        return true;
    }

    public static void PopupReport()
    {
        CheckAdmin();

        AppMessageBox.Instance.Info(
            $"""
            检测结果:
                                
            当前系统
                UAC 状态: {GetUacDescription()}
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
            (1, 2, 1) => UacNotifyLevel.AlwaysDimming,
            (1, 5, 1) => UacNotifyLevel.AppsOnlyDimming,
            (1, 5, 0) => UacNotifyLevel.AppsOnlyNoDimming,
            (1 or 0, 0, 0) => UacNotifyLevel.Never,
            (0, _, _) => UacNotifyLevel.Disabled,
            _ => UacNotifyLevel.Unknown
        };
    }

    private static string GetUacDescription()
    {
        return $"{Level} ({(int)Level}) ({(IsUACDisabled ? "异常" : "正常")})";
    }

    private static string GetUserAdmin(bool logon)
    {
        if (!logon || Win32User.NotImpersonalOrElevated)
        {
            return IsAdmin ? "有" : "无";
        }

        if (ProcessHelper.RunAsLogon(AcExe, AcArg, out int code))
        {
            return code == 0 ? "有" : "无";
        }

        return "未知";
    }
}
