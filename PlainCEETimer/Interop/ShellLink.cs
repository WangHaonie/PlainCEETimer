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

            var shlnkdesktop = new SHLNKINFO($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\高考倒计时.lnk");
            var shlnkstartmenu = new SHLNKINFO(programsdir + "高考倒计时.lnk");
            ResetAppShortcut(ref shlnkdesktop);
            ResetAppShortcut(ref shlnkstartmenu);
        }

        private static void ResetAppShortcut(ref SHLNKINFO shlnk)
        {
            Export(ref shlnk);
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
                shlnk.iIcon = 0;
                shlnk.pszIconPath = string.Empty;
                shlnk.pszDescr = App.AppName;
                Create(shlnk);
            }
        }

        [DllImport(App.NativesDll, EntryPoint = "#19")]
        public static extern void Initialize();

        [DllImport(App.NativesDll, EntryPoint = "#20", CharSet = CharSet.Unicode)]
        private static extern void Create(SHLNKINFO shLnkInfo);

        [DllImport(App.NativesDll, EntryPoint = "#21", CharSet = CharSet.Unicode)]
        private static extern void Export(ref SHLNKINFO shLnkInfo);

        [DllImport(App.NativesDll, EntryPoint = "#22")]
        public static extern void Release();
    }
}
