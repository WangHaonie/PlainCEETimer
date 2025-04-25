using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules
{
    public static class App
    {
        public static int OSBuild => field == 0 ? field = Environment.OSVersion.Version.Build : field;
        public static bool CanSaveConfig { get; set; }
        public static bool AllowUIClosing { get; private set; }
        public static bool AllowShutdown { get; set; } = true;
        public static bool IsAdmin { get; private set; }
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
        public const string AppVersion = "3.0.8";
        public const string AppBuildDate = "2025/04/25";
        public const string CopyrightInfo = "Copyright © 2023-2025 WangHaonie";
        public const string OriginalFileName = $"{AppNameEng}.exe";
        public const string InfoMsg = "提示 - 高考倒计时";
        public const string WarnMsg = "警告 - 高考倒计时";
        public const string ErrMsg = "错误 - 高考倒计时";
        private const string ExFileName = "UnhandledException.txt";

        private static Mutex MainMutex;
        private static readonly string PipeName = $"{AppNameEngOld}_[34c14833-98da-49f7-a2ab-369e88e73b95]";
        private static readonly string CurrentExecutableName = Path.GetFileName(CurrentExecutablePath);
        private static readonly MessageBoxHelper MessageX = MessageBoxHelper.Instance;

        public static void StartProgram(string[] args)
        {
            AppIcon = IconHelper.GetIcon(CurrentExecutablePath);
            _ = ThemeManager.Initialize;
            var Args = Array.ConvertAll(args, x => x.ToLower());
            var AllArgs = string.Join(" ", args);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException((Exception)e.ExceptionObject);
            MainMutex = new Mutex(true, $"{AppNameEngOld}_MUTEX_61c0097d-3682-421c-84e6-70ca37dc31dd_[A3F8B92E6D14]", out bool IsNewProcess);

            if (IsNewProcess)
            {
                new Thread(StartPipeServer).Start();

                if (!CurrentExecutableName.Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageX.Error($"为了您的使用体验，请不要更改程序文件名! 程序将在该消息框自动关闭后尝试自动恢复到原文件名，若自动恢复失败请手动改回。\n\n当前文件名: {CurrentExecutableName}\n原始文件名: {OriginalFileName}", AutoClose: true);
                    ProcessHelper.Run("cmd", $"/c ren \"{CurrentExecutablePath}\" {OriginalFileName} & start \"\" \"{CurrentExecutableDir}{OriginalFileName}\" {AllArgs}");
                    Exit(ExitReason.InvalidExeName);
                }
                else
                {
                    if (Args.Length == 0)
                    {
                        new Thread(() => CheckAdmin()).Start();
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
                                MessageX.Info($"当前用户 {CheckAdmin(true)} {(IsAdmin ? "" : "不")}具有管理员权限。");
                                break;
                            case "/fr":
                                Application.Run(new DownloaderForm(Args.Length > 1 ? Args[1] : null));
                                break;
                            case "/op":
                                new OptimizationHelper().Optimize();
                                break;
                            default:
                                MessageX.Error($"无法解析的命令行参数: \n{AllArgs}", AutoClose: true);
                                break;
                        }
                    }

                    Exit(ExitReason.NormalExit);
                }
            }
            else
            {
                if (Args.Length != 0)
                {
                    MessageX.Error("请先关闭已打开的实例再使用命令行功能。", AutoClose: true);
                }

                StartPipeClient();
                Exit(ExitReason.AnotherInstanceIsRunning);
            }
        }

        public static void Shutdown(bool Restart = false)
        {
            if (AllowShutdown)
            {
                AllowUIClosing = true;

                if (CanSaveConfig)
                {
                    ConfigHandler.Save();
                }

                Application.Exit();
                Application.ExitThread();

                if (Restart)
                {
                    ClearMutex();
                    ProcessHelper.Run(CurrentExecutablePath);
                    Exit(ExitReason.UserRestart);
                }
                else
                {
                    Exit(ExitReason.UserShutdown);
                }

                AllowUIClosing = false;
            }
        }

        public static void OpenInstallDir()
        {
            Process.Start(CurrentExecutableDir);
        }

        public static void Exit(ExitReason Reason)
        {
            ClearMutex();
            Environment.Exit((int)Reason);
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
            if (!AllowUIClosing)
            {
                var ExOutput = $"\n\n================== v{AppVersion} - {DateTime.Now.ToFormatted()} ==================\n{ex}";
                var ExFilePath = $"{CurrentExecutableDir}{ExFileName}";
                File.AppendAllText(ExFilePath, ExOutput);

                var Result = MessageX.Error($"程序出现意外错误，非常抱歉给您带来不便！相关错误信息已写入到安装文件夹中的 {ExFileName} 文件，建议您将相关信息发送给开发者以帮助我们定位并解决问题。\n现在您可以点击右上角【关闭】来忽略本次错误,【是】重启应用程序,【否】关闭应用程序。", ex, Buttons: MessageButtons.YesNo);

                if (Result != DialogResult.None)
                {
                    Shutdown(Restart: Result == DialogResult.Yes);
                }
            }
        }

        private static void ClearMutex()
        {
            MainMutex?.Dispose();
            MainMutex = null;
        }

        private static string CheckAdmin(bool QueryUserName = false)
        {
            IsAdmin = (int)ProcessHelper.Run("net", "session", 2) == 0;

            if (QueryUserName)
            {
                return (string)ProcessHelper.Run("whoami", Return: 1);
            }

            return null;
        }
    }
}