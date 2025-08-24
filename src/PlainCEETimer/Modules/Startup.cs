using System;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

/// <summary>
/// 管理本程序的开机自启状态。
/// </summary>
internal static class Startup
{
    public static bool IsTaskSchd { get; private set; }

    private static readonly bool NotElevated = Win32User.NotElevated;
    private static readonly string UserName = Win32User.LogonUser;
    private static readonly string UserNameOnly = UserName.Split('\\')[1];
    private static readonly string TaskName = $"WangHaonie\\PlainCEETimer AutoStartup ({UserName.GetHashCode():X})";
    private static readonly string AppPath = $"\"{App.CurrentExecutablePath}\"";
    private static readonly string StartupKey = App.AppNameEngOld;
    private static readonly RegistryHelper Registry = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);

    public static bool GetRegistryState()
    {
        return NotElevated && Registry.Check(StartupKey, AppPath, "");
    }

    public static void RefreshTaskState()
    {
        IsTaskSchd = CheckStartUpState() == 0;
    }

    public static void SetAll(bool option, bool isTask)
    {
        new Action(() =>
        {
            if (option && isTask)
            {
                SetTask();
                DeleteRegistry();
            }
            else if (option)
            {
                SetRegistry();
                DeleteTask();
            }
            else
            {
                DeleteAll();
            }
        }).Start();
    }

    public static void DeleteAll()
    {
        DeleteRegistry();
        DeleteTask();
    }

    public static void Cleanup()
    {
        EnableTask();
        Win32TaskScheduler.Release();
        Registry.Dispose();
    }

    private static void SetRegistry()
    {
        if (!GetRegistryState())
        {
            Registry.Set(StartupKey, AppPath);
        }
    }

    private static void SetTask()
    {
        if (CheckStartUpState() is 1 or 2)
        {
            Win32TaskScheduler.Import(TaskName, $@"<?xml version=""1.0"" encoding=""UTF-16""?><Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task""><RegistrationInfo><Author>WangHaonie</Author><Description>用于在 {UserNameOnly} 登录时自动运行</Description></RegistrationInfo><Triggers><LogonTrigger><Enabled>true</Enabled><UserId>{UserName}</UserId></LogonTrigger></Triggers><Principals><Principal id=""Author""><UserId>{UserName}</UserId><LogonType>InteractiveToken</LogonType><RunLevel>LeastPrivilege</RunLevel></Principal></Principals><Settings><DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries><StopIfGoingOnBatteries>false</StopIfGoingOnBatteries></Settings><Actions Context=""Author""><Exec><Command>{AppPath}</Command></Exec></Actions></Task>");
            RefreshTaskState();
        }
    }

    private static void EnableTask()
    {
        Win32TaskScheduler.Enable(TaskName);
    }

    private static void DeleteRegistry()
    {
        if (GetRegistryState())
        {
            Registry.Delete(StartupKey);
        }
    }

    private static void DeleteTask()
    {
        if (CheckStartUpState() is 0 or 1)
        {
            Win32TaskScheduler.Delete(TaskName);
            RefreshTaskState();
        }
    }

    /// <summary>
    /// 获取本程序在任务计划程序中的自启动状态。
    /// </summary>
    /// <returns>0 - 已设置，1 - 设置异常，2 - 未设置，3 - 环境异常</returns>
    private static int CheckStartUpState()
    {
        if (NotElevated)
        {
            Win32TaskScheduler.Export(TaskName, out string raw);

            if (!string.IsNullOrEmpty(raw))
            {
                var xml = Xml.FormString(raw);
                EnableTask();

                if (xml.Check("true", true, "Triggers", "LogonTrigger", "Enabled") &&
                    xml.Check(AppPath, false, "Actions", "Exec", "Command") &&
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
}
