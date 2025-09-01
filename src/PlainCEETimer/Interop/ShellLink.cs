using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Interop;

public static class ShellLink
{
    private static readonly string LnkName = "高考倒计时.lnk";
    private static readonly string AppPath = App.CurrentExecutablePath;
    private static readonly AppMessageBox MessageX = AppMessageBox.Instance;
    private static SaveFileDialog Dialog;

    public static void CreateAppShortcut(bool allowCustom = false)
    {
        Initialize();

        if (Create(allowCustom))
        {
            MessageX.Info("操作已完成。", autoClose: true);
        }

        Release();
    }

    private static bool Create(bool allowCustom)
    {
        if (allowCustom)
        {
            Dialog ??= new()
            {
                Title = "保存快捷方式 - 高考倒计时",
                Filter = FileDialogWrapper.CreateFilters(FileFilter.Shortcut),
                FileName = LnkName
            };

            if (FileDialogWrapper.ShowDialog(Dialog) == DialogResult.OK)
            {
                ResetAppShortcut(Dialog.FileName);
                return true;
            }
        }
        else
        {
            if (Win32User.NotElevated)
            {
                if (MessageX.Warn("确认检查并重设 开始菜单 和 桌面 快捷方式？", MessageButtons.YesNo) == DialogResult.Yes)
                {
                    var programsdir = $"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\{App.AppName}\\";

                    if (!Directory.Exists(programsdir))
                    {
                        Directory.CreateDirectory(programsdir);
                    }

                    ResetAppShortcut($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{LnkName}");
                    ResetAppShortcut(programsdir + LnkName);

                    return true;
                }
            }
            else
            {
                MessageX.Error("系统环境异常！请考虑加上 /custom 参数来创建快捷方式");
            }
        }

        return false;
    }

    private static void ResetAppShortcut(string lnkPath)
    {
        var lnk = new Win32Lnk(lnkPath);
        Query(ref lnk);
        var path = lnk.Target;
        var args = lnk.Args;
        var needed = false;

        if (path == null || !path.Equals(App.CurrentExecutablePath, StringComparison.OrdinalIgnoreCase))
        {
            lnk.Target = AppPath;
            needed = true;
        }

        if (!string.IsNullOrEmpty(args))
        {
            lnk.Args = string.Empty;
            needed = true;
        }

        if (needed)
        {
            lnk.WorkingDir = string.Empty;
            lnk.Hotkey = LnkHotkey.None;
            lnk.ShowCmd = ShowWindowCommand.Normal;
            lnk.Description = App.AppName;
            lnk.IconPath = string.Empty;
            lnk.IconIndex = 0;
            Create(lnk);
        }
    }

    [DllImport(App.NativesDll, EntryPoint = "#21")]
    private static extern void Initialize();

    [DllImport(App.NativesDll, EntryPoint = "#22", CharSet = CharSet.Unicode)]
    private static extern void Create(Win32Lnk shLnkInfo);

    [DllImport(App.NativesDll, EntryPoint = "#23", CharSet = CharSet.Unicode)]
    private static extern void Query(ref Win32Lnk lpshLnkInfo);

    [DllImport(App.NativesDll, EntryPoint = "#24")]
    private static extern void Release();
}