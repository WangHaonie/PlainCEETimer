using System.IO;
using System.Text;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules
{
    public static class Win32TaskScheduler
    {
        public static bool IsTaskSchd { get; private set; }

        private static readonly bool IsWin7 = App.OSBuild == WindowsBuilds.Windows7;
        private static readonly string UserName = Win32User.SessionUser;
        private static readonly string UserNameOnly = UserName.Split('\\')[1];
        private static readonly string TaskName = $"\"WangHaonie\\PlainCEETimer AutoStartup ({UserName.GetHashCode():X})\"";
        private static readonly string AppPath = App.CurrentExecutablePath;

        public static void RefreshStartUpState()
        {
            IsTaskSchd = CheckStartUpState() == 0;
        }

        public static void SetStartUpTask()
        {
            if (CheckStartUpState() is 1 or 2)
            {
                var xml = $"{AppPath}.Task.xml";

                File.WriteAllText(xml, $@"<?xml version=""1.0"" encoding=""UTF-16""?><Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task""><RegistrationInfo><Author>WangHaonie</Author><Description>用于在 {UserNameOnly} 登录时自动运行</Description></RegistrationInfo><Triggers><LogonTrigger><Enabled>true</Enabled><UserId>{UserName}</UserId></LogonTrigger></Triggers><Principals><Principal id=""Author""><UserId>{UserName}</UserId><LogonType>InteractiveToken</LogonType><RunLevel>LeastPrivilege</RunLevel></Principal></Principals><Settings><DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries><StopIfGoingOnBatteries>false</StopIfGoingOnBatteries></Settings><Actions Context=""Author""><Exec><Command>""{AppPath}""</Command></Exec></Actions></Task>", Encoding.Unicode);
                ProcessHelper.Run("schtasks", $"/create /tn {TaskName} /xml \"{xml}\" /f", getExitCode: true);
                File.Delete(xml);
                RefreshStartUpState();
            }
        }

        public static void DeleteStartUpTask()
        {
            if (CheckStartUpState() != 3)
            {
                ProcessHelper.Run("schtasks", $"/delete /tn {TaskName} /f", getExitCode: true);
                RefreshStartUpState();
            }
        }

        /// <summary>
        /// 获取本程序的自启动状态。
        /// </summary>
        /// <returns>0 - 已设置，1 - 设置异常，2 - 命令出错，3 - 环境异常</returns>
        private static int CheckStartUpState()
        {
            if (Win32User.NotElevated)
            {
                var args = $"/query /tn {TaskName} /xml";

                if (IsWin7)
                {
                    return ProcessHelper.Run("cmd", $"/c chcp 437 >nul && schtasks " + args, getExitCode: true) == 0 ? 0 : 2;
                }
                else
                {
                    var raw = ProcessHelper.GetOutput("schtasks", args);

                    if (raw != "")
                    {
                        var xml = Xml.FormString(raw);

                        if (xml.Check("true", true, "Triggers", "LogonTrigger", "Enabled") &&
                            xml.Check($"\"{AppPath}\"", false, "Actions", "Exec", "Command") &&
                            xml.Check("false", false, "Settings", "DisallowStartIfOnBatteries") &&
                            xml.Check("false", false, "Settings", "StopIfGoingOnBatteries"))
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
                }
            }

            return 3;
        }
    }
}
