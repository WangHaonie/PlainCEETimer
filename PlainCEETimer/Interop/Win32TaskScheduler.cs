using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class Win32TaskScheduler
    {
        public static bool IsTaskSchd { get; private set; }

        private static readonly string UserName = Win32User.SessionUser;
        private static readonly string UserNameOnly = UserName.Split('\\')[1];
        private static readonly string TaskName = $"WangHaonie\\PlainCEETimer AutoStartup ({UserName.GetHashCode():X})";
        private static readonly string AppPath = App.CurrentExecutablePath;

        static Win32TaskScheduler()
        {
            Initialize();
        }

        public static void RefreshStartUpState()
        {
            IsTaskSchd = CheckStartUpState() == 0;
        }

        public static void SetStartUpTask()
        {
            if (CheckStartUpState() is 1 or 2)
            {
                ImportTask(TaskName, $@"<?xml version=""1.0"" encoding=""UTF-16""?><Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task""><RegistrationInfo><Author>WangHaonie</Author><Description>用于在 {UserNameOnly} 登录时自动运行</Description></RegistrationInfo><Triggers><LogonTrigger><Enabled>true</Enabled><UserId>{UserName}</UserId></LogonTrigger></Triggers><Principals><Principal id=""Author""><UserId>{UserName}</UserId><LogonType>InteractiveToken</LogonType><RunLevel>LeastPrivilege</RunLevel></Principal></Principals><Settings><DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries><StopIfGoingOnBatteries>false</StopIfGoingOnBatteries></Settings><Actions Context=""Author""><Exec><Command>""{AppPath}""</Command></Exec></Actions></Task>");
                RefreshStartUpState();
            }
        }

        public static void DeleteStartUpTask()
        {
            if (CheckStartUpState() != 3)
            {
                DeleteTask(TaskName);
                RefreshStartUpState();
            }
        }

        /// <summary>
        /// 获取本程序的自启动状态。
        /// </summary>
        /// <returns>0 - 已设置，1 - 设置异常，2 - 未设置，3 - 环境异常</returns>
        private static int CheckStartUpState()
        {
            if (Win32User.NotElevated)
            {
                ExportTask(TaskName, out IntPtr pBstrXml);
                var raw = Marshal.PtrToStringUni(pBstrXml);
                SysFreeString(pBstrXml);

                if (!string.IsNullOrEmpty(raw))
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

            return 3;
        }

        [DllImport(App.NativesDll, EntryPoint = "#16")]
        private static extern void Initialize();

        [DllImport(App.NativesDll, EntryPoint = "#17", CharSet = CharSet.Unicode)]
        private static extern void ImportTask(string taskName, string xml);

        [DllImport(App.NativesDll, EntryPoint = "#18", CharSet = CharSet.Unicode)]
        private static extern void ExportTask(string taskName, out IntPtr pBstrXml);

        [DllImport(App.NativesDll, EntryPoint = "#19", CharSet = CharSet.Unicode)]
        private static extern void DeleteTask(string taskName);

        [DllImport(App.NativesDll, EntryPoint = "#20")]
        public static extern void Release();

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode)]
        private static extern void SysFreeString(IntPtr bstrString);
    }
}
