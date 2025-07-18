﻿using Microsoft.Win32;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules
{
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

        static UacHelper()
        {
            Level = GetNotifyLevel();
            IsUACDisabled = Level >= UacNotifyLevel.Never;
        }

        public static bool EnsureUAC(MessageBoxHelper mx)
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

        public static void CheckAdmin()
        {
            IsAdmin = ProcessHelper.Run("net", "session", getExitCode: true) == 0;
        }

        private static UacNotifyLevel GetNotifyLevel()
        {
            using var reg = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", rootKey: RegistryHive.LocalMachine);

            return (reg.Get("EnableLUA", 0), reg.Get("ConsentPromptBehaviorAdmin", 0), reg.Get("PromptOnSecureDesktop", 0)) switch
            {
                (1, 2, 1) => UacNotifyLevel.AllDimming,
                (1, 5, 1) => UacNotifyLevel.AppsOnlyDimming,
                (1, 5, 0) => UacNotifyLevel.AppsOnlyNoDimming,
                (1 or 0, 0, 0) => UacNotifyLevel.Never,
                (0, _, _) => UacNotifyLevel.Disabled,
                _ => UacNotifyLevel.Unknown
            };
        }
    }
}
