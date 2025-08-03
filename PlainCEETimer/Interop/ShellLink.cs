using System;
using System.IO;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ShellLink
    {
        private static readonly string AppPath = App.CurrentExecutablePath;

        public static void CreateAppShortcut()
        {
            var programsdir = $"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\{App.AppName}\\";

            if (!Directory.Exists(programsdir))
            {
                Directory.CreateDirectory(programsdir);
            }

            ResetAppShortcut($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\高考倒计时.lnk");
            ResetAppShortcut(programsdir + "高考倒计时.lnk");
        }

        private static void ResetAppShortcut(string lnk)
        {
            var shlnk = new SHLNKINFO(lnk);
            Query(ref shlnk);
            var path = shlnk.pszFile;
            var args = shlnk.pszArgs;
            var needed = false;

            if (path == null || !path.Equals(App.CurrentExecutablePath, StringComparison.OrdinalIgnoreCase))
            {
                shlnk.pszFile = AppPath;
                needed = true;
            }

            if (!string.IsNullOrEmpty(args))
            {
                shlnk.pszArgs = string.Empty;
                needed = true;
            }

            if (needed)
            {
                shlnk.pszWorkDir = string.Empty;
                shlnk.wHotkey = SHKEY.NONE;
                shlnk.iShowCmd = SWCMD.NORMAL;
                shlnk.pszDescr = App.AppName;
                shlnk.pszIconPath = string.Empty;
                shlnk.iIcon = 0;
                Create(shlnk);
            }
        }

        [DllImport(App.NativesDll, EntryPoint = "#19")]
        public static extern void Initialize();

        [DllImport(App.NativesDll, EntryPoint = "#20", CharSet = CharSet.Unicode)]
        private static extern void Create(SHLNKINFO shLnkInfo);

        [DllImport(App.NativesDll, EntryPoint = "#21", CharSet = CharSet.Unicode)]
        private static extern void Query(ref SHLNKINFO shLnkInfo);

        [DllImport(App.NativesDll, EntryPoint = "#22")]
        public static extern void Release();
    }
}
