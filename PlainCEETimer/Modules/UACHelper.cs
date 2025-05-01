using Microsoft.Win32;

namespace PlainCEETimer.Modules
{
    /*
    
    获取 UAC 通知等级 参考：

    How to change UAC settings in Windows 10
    https://winaero.com/how-to-change-uac-settings-in-windows-10/

    UAC Levels & Windows User Account Control Settings - CyberFOX
    https://support.cyberfox.com/settings/360048585292-UAC-Levels-Windows-User-Account-Control-Settings

    */

    public static class UACHelper
    {
        public static bool IsUACDisabled { get; }
        public static bool IsAdmin { get; private set; }
        public static string UserName { get; private set; }

        private static readonly UACNotifyLevel Level;

        static UACHelper()
        {
            Level = GetUACNotifyLevel();
            IsUACDisabled = Level >= UACNotifyLevel.Never;
        }

        public static bool EnsureUAC(MessageBoxHelper mx)
        {
            if (!IsAdmin)
            {
                mx.Warn("检测到当前用户不具有管理员权限，将尝试自动提权。\n\n若弹出 UAC 对话框，请点击 是。");

                if (IsUACDisabled)
                {
                    mx.Error("提权失败! 当前系统已禁用 UAC。");
                    return false;
                }
            }

            return true;
        }

        public static void CheckAdmin(bool QueryUserName = false)
        {
            IsAdmin = (int)ProcessHelper.Run("net", "session", 2) == 0;

            if (QueryUserName)
            {
                UserName = (string)ProcessHelper.Run("whoami", Return: 1);
            }
        }

        private static UACNotifyLevel GetUACNotifyLevel()
        {
            using var reg = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", rootKey: RegistryHive.LocalMachine);

            return ((int)reg.GetValue("EnableLUA", 0), (int)reg.GetValue("ConsentPromptBehaviorAdmin", 0), (int)reg.GetValue("PromptOnSecureDesktop", 0)) switch
            {
                (1, 2, 1) => UACNotifyLevel.AllDimming,
                (1, 5, 1) => UACNotifyLevel.AppsOnlyDimming,
                (1, 5, 0) => UACNotifyLevel.AppsOnlyNoDimming,
                (1 or 0, 0, 0) => UACNotifyLevel.Never,
                (0, _, _) => UACNotifyLevel.Disabled,
                _ => UACNotifyLevel.Unknown
            };
        }
    }
}
