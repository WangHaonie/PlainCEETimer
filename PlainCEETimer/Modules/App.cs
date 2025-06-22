using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules
{
    public static class App
    {
        public static int OSBuild => field == 0 ? field = Environment.OSVersion.Version.Build : field;
        public static string CurrentExecutableDir => field ??= AppDomain.CurrentDomain.BaseDirectory;
        public static string CurrentExecutablePath => field ??= Application.ExecutablePath;
        public static string ConfigFilePath => field ??= $"{CurrentExecutableDir}{AppNameEng}.config";
        public static Icon AppIcon { get; private set; }
        public static Version AppVersionObject => field ??= Version.Parse(AppVersion);
        public static ConfigObject AppConfig
        {
            get => field ??= ConfigHandler.Read();
            set
            {
                field = value;
                CanSaveConfig = true;
            }
        }

        public static event Action TrayMenuShowAllClicked;
        public static void OnTrayMenuShowAllClicked() => TrayMenuShowAllClicked?.Invoke();

        public const string AppName = "高考倒计时 by WangHaonie";
        public const string AppNameEng = "PlainCEETimer";
        public const string AppNameEngOld = "CEETimerCSharpWinForms";
        public const string NativesDll = "PlainCEETimer.Natives.dll";
        public const string User32Dll = "user32.dll";
        public const string UxThemeDll = "uxtheme.dll";
        public const string Shell32Dll = "shell32.dll";
        public const string Gdi32Dll = "gdi32.dll";
        public const string AppVersion = "5.0.3";
        public const string AppBuildDate = "2025/6/22";
        public const string CopyrightInfo = "Copyright © 2023-2025 WangHaonie";
        public const string DateTimeFormat = "yyyyMMddHHmmss";
        public const string OriginalFileName = $"{AppNameEng}.exe";
        public const string InfoMsg = "提示 - 高考倒计时";
        public const string WarnMsg = "警告 - 高考倒计时";
        public const string ErrMsg = "错误 - 高考倒计时";

        private static bool IsMainProcess;
        private static bool IsClosing;
        private static bool CanSaveConfig;
        private static Mutex MainMutex;
        private static readonly string PipeName = $"{AppNameEngOld}_[34c14833-98da-49f7-a2ab-369e88e73b95]";
        private static readonly string CurrentExecutableName = Path.GetFileName(CurrentExecutablePath);
        private static readonly MessageBoxHelper MessageX = MessageBoxHelper.Instance;

        public static void StartProgram(string[] args)
        {
            ExtractConfig();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException((Exception)e.ExceptionObject);
            AppIcon = IconHelper.GetIcon(CurrentExecutablePath);
            _ = ThemeManager.Initialize;
            var Args = Array.ConvertAll(args, x => x.ToLower());
            var AllArgs = string.Join(" ", args);
            MainMutex = new Mutex(true, $"{AppNameEngOld}_MUTEX_61c0097d-3682-421c-84e6-70ca37dc31dd_[A3F8B92E6D14]", out IsMainProcess);

            if (IsMainProcess)
            {
                new Action(StartPipeServer).Start();

                if (CurrentExecutableName.Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
                {
                    if (Args.Length == 0)
                    {
                        new Action(UACHelper.CheckAdmin).Start();
                        Application.Run(new MainForm());
                    }
                    else
                    {
                        switch (Args[0])
                        {
                            case "/?":
                            case "/h":
                                MessageX.Info(
                                    """
                                    可用的命令行参数: 
                                    
                                    /h    显示此帮助信息
                                    /ac  检测当前用户是否具有管理员权限
                                    /fr <版本号>
                                            强制下载并安装指定的版本，留空则当前版本，
                                            推荐在特殊情况下使用，不支持老版本
                                    /op  优化本程序，提升运行速度
                                    """);
                                break;
                            case "/ac":
                                UACHelper.CheckAdmin();
                                MessageX.Info(
                                    $"""
                                    检测结果：

                                    {Win32User.SessionUser} 为当前登入的账户。
                                    目前高考倒计时正在以 {Win32User.ProcessOwner} 的身份运行{(Win32User.NotElevated ? "" : " (已提权)")}，{(UACHelper.IsAdmin ? "已经" : "无法")}获取到管理员权限。 
                                    """);
                                break;
                            case "/fr":
                                Application.Run(new DownloaderForm(Args.Length > 1 ? Args[1] : null));
                                break;
                            case "/op":
                                UACHelper.CheckAdmin();
                                new OptimizationHelper(Args.Length > 1 && Args[1] == "/auto").Optimize();
                                break;
                            default:
                                MessageX.Error($"无法解析的命令行参数: \n{AllArgs}", autoClose: true);
                                break;
                        }
                    }

                    Exit(ExitReason.Normal);
                }
                else
                {
                    MessageX.Error($"为了您的使用体验，请不要更改程序文件名! 程序将在该消息框自动关闭后尝试自动恢复到原文件名，若自动恢复失败请手动改回。\n\n当前文件名: {CurrentExecutableName}\n原始文件名: {OriginalFileName}", autoClose: true);
                    ProcessHelper.Run("cmd", $"/c ren \"{CurrentExecutablePath}\" {OriginalFileName} && start \"\" \"{CurrentExecutableDir}{OriginalFileName}\" {AllArgs}");
                    Exit(ExitReason.InvalidExeName);
                }
            }
            else
            {
                if (Args.Length != 0)
                {
                    MessageX.Error("请先关闭已打开的实例再使用命令行功能。", autoClose: true);
                }

                StartPipeClient();
                Exit(ExitReason.MultipleInstances);
            }
        }

        public static void Exit(ExitReason reason)
        {
            IsClosing = true;
            var Restart = reason == ExitReason.UserRestart;

            if (CanSaveConfig)
            {
                ConfigHandler.Save();
            }

            if (IsMainProcess && MainMutex != null)
            {
                MainMutex.ReleaseMutex();
                MainMutex.Dispose();
                MainMutex = null;
            }

            ProcessHelper.Run("cmd.exe", $"/c taskkill /f /fi \"PID eq {Process.GetCurrentProcess().Id}\" /im {CurrentExecutableName} {(Restart ? $"& start \"\" \"{CurrentExecutablePath}\"" : "")}");
            Environment.Exit((int)reason);
        }

        private static void StartPipeServer()
        {
            try
            {
                while (true)
                {
                    using var PipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
                    PipeServer.WaitForConnection();
                    OnTrayMenuShowAllClicked();
                }
            }
            catch { }
        }

        private static void StartPipeClient()
        {
            try
            {
                using var PipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                PipeClient.Connect(1000);
            }
            catch { }
        }

        private static void HandleException(Exception ex)
        {
            if (!IsClosing)
            {
                var Now = DateTime.Now;
                var ExOutput = $"—————————————————— {AppNameEng} v{AppVersion} - {Now.Format()} ——————————————————\n{ex}";
                var ExFileName = $"UnhandledException_{Now.ToString(DateTimeFormat)}.txt";
                var ExFilePath = $"{CurrentExecutableDir}{ExFileName}";
                File.AppendAllText(ExFilePath, ExOutput);

                var Result = MessageBox.Show($"程序出现意外错误，非常抱歉给您带来不便！详细错误信息已写入安装目录中的 {ExFileName} 文件，建议您将其发送给开发者以帮助我们定位并解决问题。\n\n现在您可以点击【中止】关闭应用程序，【重试】重启应用程序，【忽略】忽略本次错误。\n\n错误信息：\n{ex.Message}", "意外错误 - 高考倒计时", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                if (Result != DialogResult.Ignore)
                {
                    Exit(Result == DialogResult.Retry ? ExitReason.UserRestart : ExitReason.UserShutdown);
                }
            }
        }

        private static void ExtractConfig()
        {
            /*
            
            使 NumericUpDown 的 UpDown 按钮在高 DPI 下自动缩放 参考：

            NumericUpDown 类 (System.Windows.Forms) | Microsoft Learn
            https://learn.microsoft.com/zh-cn/dotnet/api/system.windows.forms.numericupdown

            */

            try
            {
                var appconfig = $"{CurrentExecutablePath}.config";
                File.Delete(appconfig);
                File.WriteAllText(appconfig, @"<?xml version=""1.0"" encoding=""utf-8"" ?><configuration><appSettings><add key=""EnableWindowsFormsHighDpiAutoResizing"" value=""true""/></appSettings></configuration>");
                ProcessHelper.Run("cmd", $"/c attrib +s +h \"{appconfig}\"");
            }
            catch { }
        }
    }
}