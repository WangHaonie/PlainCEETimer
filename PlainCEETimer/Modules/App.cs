using PlainCEETimer.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public static class App
    {
        public static bool IsAdmin { get; private set; }
        public static bool AllowClosing { get; private set; } = false;
        public static bool IsWindows7Above => Environment.OSVersion.Version >= new Version(6, 1, 7600);
        public static bool IsWindows11 => Environment.OSVersion.Version >= new Version(10, 0, 22000);
        public static event EventHandler TrayMenuShowAllClicked;
        public static event EventHandler UniTopMostStateChanged;
        public static event EventHandler AppConfigChanged;
        public static Icon AppIcon { get; private set; }
        public static void OnTrayMenuShowAllClicked() => TrayMenuShowAllClicked?.Invoke(null, EventArgs.Empty);
        public static void OnUniTopMostStateChanged() => UniTopMostStateChanged?.Invoke(null, EventArgs.Empty);
        public static void OnAppConfigChanged() => AppConfigChanged?.Invoke(null, EventArgs.Empty);

        public static string CurrentExecutableDir
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = AppDomain.CurrentDomain.BaseDirectory;
                }

                return field;
            }
        }

        public static string CurrentExecutablePath
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = Application.ExecutablePath;
                }

                return field;
            }
        }

        public static string ConfigFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = $"{CurrentExecutableDir}{AppNameEng}.config";
                }

                return field;
            }
        }

        public const string AppName = "高考倒计时 by WangHaonie";
        public const string AppNameEng = "PlainCEETimer";
        public const string AppNameEngBak = "CEETimerCSharpWinForms";
        public const string AppVersion = "3.0.9";
        public const string AppBuildDate = "2025/02/25";
        public const string CopyrightInfo = "Copyright © 2023-2025 WangHaonie";
        public const string OriginalFileName = $"{AppNameEng}.exe";
        public const string InfoMsg = "提示 - 高考倒计时";
        public const string WarnMsg = "警告 - 高考倒计时";
        public const string ErrMsg = "错误 - 高考倒计时";
        public const string DateTimeFormat = "yyyy-MM-dd dddd HH:mm:ss";
        public const string UpdateAPI = "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/api/github.json";
        public const string UpdateURL = "https://gitee.com/WangHaonie/CEETimerCSharpWinForms/raw/main/download/CEETimerCSharpWinForms_{0}_x64_Setup.exe";
        public const string RequestUA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36";

        private static bool MainInstance;
        private static readonly string PipeName = $"{AppNameEngBak}_[34c14833-98da-49f7-a2ab-369e88e73b95]";
        private static readonly string CurrentExecutableName = Path.GetFileName(CurrentExecutablePath);
        private static readonly MessageBoxHelper MessageX = new();
        private static Mutex MainMutex;

        public static void SetDpiAwareness()
        {
            bool IsWindows81Above = Environment.OSVersion.Version >= new Version(6, 3, 9600);
            bool IsWindows10Build1703Above = Environment.OSVersion.Version >= new Version(10, 0, 15063);
            DpiAwareness.SetProcessDpiAwarenessEx(IsWindows10Build1703Above ? 2 : (IsWindows81Above ? 1 : 0));
        }

        public static void StartProgram(string[] args)
        {
            AppIcon = Icon.ExtractAssociatedIcon(CurrentExecutablePath);
            _ = DefaultValues.Initialize;
            var Args = Array.ConvertAll(args, x => x.ToLower());
            var AllArgs = string.Join(" ", args);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => HandleException((Exception)e.ExceptionObject);
            MainMutex = new Mutex(true, $"{AppNameEngBak}_MUTEX_61c0097d-3682-421c-84e6-70ca37dc31dd_[A3F8B92E6D14]", out bool IsNewProcess);

            if (IsNewProcess)
            {
                MainInstance = true;
                new Thread(StartPipeServer).Start();

                if (!CurrentExecutableName.Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageX.Error($"为了您的使用体验，请不要更改程序文件名! 程序将在该消息框自动关闭后尝试自动恢复到原文件名，若自动恢复失败请手动改回。\n\n当前文件名: {CurrentExecutableName}\n原始文件名: {OriginalFileName}", AutoClose: true);
                    ProcessHelper.RunProcess("cmd.exe", $"/c ren \"{CurrentExecutablePath}\" {OriginalFileName} & start \"\" \"{CurrentExecutableDir}{OriginalFileName}\" {AllArgs}");
                    Exit(ExitReason.InvalidExeName);
                }
                else
                {
                    if (Args.Length == 0)
                    {
                        new Thread(() => CheckAdmin(out _)).Start();
                        Application.Run(new MainForm());
                    }
                    else
                    {
                        switch (Args[0])
                        {
                            case "/?":
                            case "/h":
                                MessageX.Info("可用的命令行参数: \n\n/h    显示此帮助信息；\n/ac  检测当前用户是否具有管理员权限；\n/fr <版本号>\n        强制下载并安装指定的版本，留空则当前版本，\n        推荐在特殊情况下使用，不支持老版本。");
                                break;
                            case "/ac":
                                CheckAdmin(out string UserName, true);
                                MessageX.Info($"当前用户 {UserName} {(IsAdmin ? "" : "不")}具有管理员权限。");
                                break;
                            case "/fr":
                                var version = Args.Length > 1 ? Args[1] : null;
                                Application.Run(new DownloaderForm(version));
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
            AllowClosing = true;

            if (Restart)
            {
                ClearMutex();
                ProcessHelper.RunProcess(CurrentExecutablePath, null);
                Exit(ExitReason.UserRestart);
            }
            else
            {
                Exit(ExitReason.UserShutdown);
            }

            AllowClosing = false;
        }

        public static void CheckAdmin(out string UserName, bool QueryUserName = false)
        {
            var AdminChecker = ProcessHelper.RunProcess("cmd.exe", "/c net session");
            AdminChecker.WaitForExit();
            IsAdmin = AdminChecker.ExitCode == 0;
            UserName = "";

            if (QueryUserName)
            {
                UserName = ProcessHelper.GetProcessOutput(ProcessHelper.RunProcess("cmd.exe", "/c whoami", RedirectOutput: true));
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
            var ExOutput = $"\n\n================== v{AppVersion} - {DateTime.Now.ToString(DateTimeFormat)} =================={ex.ToMessage()}";
            var ExFileName = "UnhandledException.txt";
            var ExFilePath = $"{CurrentExecutableDir}{ExFileName}";
            File.AppendAllText(ExFilePath, ExOutput);

            var _DialogResult = MessageX.Error(string.Format("程序出现意外错误，非常抱歉给您带来不便！相关错误信息已写入到安装文件夹中的 {0} 文件，建议您将相关信息发送给开发者以帮助我们定位并解决问题。\n现在您可以点击右上角【关闭】来忽略本次错误,【是】重启应用程序,【否】关闭应用程序。{1}", ExFileName, ex.ToMessage()), Buttons: MessageBoxExButtons.YesNo);

            if (_DialogResult != DialogResult.None)
            {
                Shutdown(Restart: _DialogResult == DialogResult.Yes);
            }
        }

        private static void ClearMutex()
        {
            if (MainInstance)
            {
                MainMutex?.ReleaseMutex();
            }

            MainMutex?.Dispose();
            MainMutex = null;
        }
    }
}