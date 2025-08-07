using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
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
        public const string AppVersion = "5.0.5.1";
        public const string AppBuildDate = "2025/8/7";
        public const string CopyrightInfo = "Copyright © 2023-2025 WangHaonie";
        public const string OriginalFileName = $"{AppNameEng}.exe";
        public const string NativesDll = "PlainCEETimer.Natives.dll";
        public const string User32Dll = "user32.dll";
        public const string UxThemeDll = "uxtheme.dll";
        public const string Shell32Dll = "shell32.dll";
        public const string Gdi32Dll = "gdi32.dll";
        public const string DateTimeFormat = "yyyyMMddHHmmss";
        private const string UEFilePrefix = "UnhandledException_";

        private static bool IsMainProcess;
        private static bool IsClosing;
        private static bool CanSaveConfig;
        private static string[] Args;
        private static Mutex MainMutex;
        private static readonly string PipeName = $"{AppNameEngOld}_[34c14833-98da-49f7-a2ab-369e88e73b95]";
        private static readonly string CurrentExecutableName = Path.GetFileName(CurrentExecutablePath);
        private static readonly string DotNetConfig = $"{CurrentExecutablePath}.config";
        private static readonly MessageBoxHelper MessageX = MessageBoxHelper.Instance;

        public static void StartProgram(string[] args)
        {
            ExtractConfig();
            _ = ThemeManager.Initialize;
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException((Exception)e.ExceptionObject);
            AppIcon = IconHelper.GetIcon(CurrentExecutablePath);
            Args = Array.ConvertAll(args, x => x.ToLower());
            var AllArgs = string.Join(" ", args);
            MainMutex = new Mutex(true, $"{AppNameEngOld}_MUTEX_61c0097d-3682-421c-84e6-70ca37dc31dd_[A3F8B92E6D14]", out IsMainProcess);

            if (IsMainProcess)
            {
                new Action(StartPipeServer).Start();

                if (CurrentExecutableName.Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
                {
                    if (Args.Length == 0)
                    {
                        new Action(UacHelper.CheckAdmin).Start();
                        Win32TaskScheduler.Initialize();
                        Application.Run(new MainForm());
                    }
                    else
                    {
                        switch (Args[0])
                        {
                            case "/?":
                            case "/h":
                                PopupHelp();
                                break;
                            case "/ac":
                                UacHelper.CheckAdmin();
                                MessageX.Info(
                                    $"""
                                    检测结果：

                                    {Win32User.LogonUser} 为当前登入的账户。
                                    目前高考倒计时正在以 {Win32User.ProcessOwner} 的身份运行{(Win32User.NotElevated ? "" : " (已提权)")}，{(UacHelper.IsAdmin ? "已经" : "无法")}获取到管理员权限。 
                                    """);
                                break;
                            case "/fr":
                                Application.Run(new DownloaderForm(GetNextArg()));
                                break;
                            case "/op":
                                UacHelper.CheckAdmin();
                                new OptimizationHelper(GetNextArg() == "/auto").Optimize();
                                break;
                            case "/lnk":
                                ShellLink.CreateAppShortcut(GetNextArg() == "/custom");
                                break;
                            case "/uninst":
                                Startup.DeleteAll();
                                DeleteExtraFiles();
                                break;
                            default:
                                MessageX.Error($"无法解析的命令行参数: \n{AllArgs}", autoClose: true);
                                break;
                        }
                    }
                }
                else
                {
                    MessageX.Error($"为了您的使用体验，请不要更改程序文件名! 程序将在该消息框自动关闭后尝试自动恢复到原文件名，若自动恢复失败请手动改回。\n\n当前文件名: {CurrentExecutableName}\n原始文件名: {OriginalFileName}", autoClose: true);
                    ProcessHelper.Run("cmd", $"/c ren \"{CurrentExecutablePath}\" {OriginalFileName} && start \"\" \"{CurrentExecutableDir}{OriginalFileName}\" {AllArgs}");
                }
            }
            else if (Args.Length == 0)
            {
                StartPipeClient();
            }
            else if (!(Args[0] == "/run" && Args.Length > 3 && StartPipeClient(args[1], args[2], string.Join(" ", args.Skip(3)))))
            {
                MessageX.Error("请先退出已打开的实例再使用命令行功能。", autoClose: true);
            }

            Exit();
        }

        private static void PopupHelp()
        {
            MessageX.Info(
                """
                可用的命令行参数: 

                /h
                        显示此帮助信息
                /ac
                        检测当前用户是否具有管理员权限
                /fr [<版本号>]
                        强制下载并安装指定的版本，留空则当前版本，推荐
                        在特殊情况下使用，不支持老版本
                /op
                        优化本程序，提升运行速度
                /lnk [/custom]
                        向开始菜单文件夹和桌面创建指向本程序的快捷方式
                        /custom 表示用户将自行选择保存快捷方式的文件夹
                """);
        }

        public static void Exit(bool restart = false)
        {
            Startup.Cleanup();
            IsClosing = true;

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

            ProcessHelper.Run("cmd", $"/c taskkill /f /fi \"PID eq {Process.GetCurrentProcess().Id}\" /im {CurrentExecutableName} {(restart ? $"& start \"\" \"{CurrentExecutablePath}\"" : "")}");
            Environment.Exit(0);
        }

        private static void DeleteExtraFiles()
        {
            try
            {
                foreach (var uefile in Directory.GetFiles(CurrentExecutableDir, $"{UEFilePrefix}*.txt"))
                {
                    File.Delete(uefile);
                }

                File.Delete(DotNetConfig);
            }
            catch { }
        }

        private static void StartPipeServer()
        {
            try
            {
                while (true)
                {
                    using var server = new NamedPipeServerStream(PipeName, PipeDirection.In);
                    server.WaitForConnection();
                    OnTrayMenuShowAllClicked();
                }
            }
            catch { }
        }

        private static string GetNextArg()
        {
            if (Args.Length > 1)
            {
                return Args[1];
            }

            return null;
        }

        private static bool StartPipeClient(string pipe = null, string path = null, string args = null)
        {
            var isRedirector = !string.IsNullOrEmpty(pipe);

            try
            {
                using var client = new NamedPipeClientStream(".", isRedirector ? pipe : PipeName, PipeDirection.Out);
                client.Connect(isRedirector ? 500 : 1000);

                if (isRedirector)
                {
                    ProcessHelper.RunRedirector(new StreamWriter(client) { AutoFlush = true }, path, args);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void HandleException(Exception ex)
        {
            if (!IsClosing)
            {
                var now = DateTime.Now;
                var content = $"—————————————————— {AppNameEng} v{AppVersion} - {now.Format()} ——————————————————\n{ex}";
                var exFileName = $"{UEFilePrefix}{now.ToString(DateTimeFormat)}.txt";
                var exFilePath = $"{CurrentExecutableDir}{exFileName}";
                File.AppendAllText(exFilePath, content);

                var result = MessageBox.Show($"程序出现意外错误，非常抱歉给您带来不便！\n\n个别常见错误可能收录于用户手册中，请到仓库首页访问并查询可能的解决办法。若无则建议您及时将相关内容提交到 Issues 以帮助我们定位并解决问题。\n\n错误信息：\n{ex.Message}\n\n详细错误信息已保存至：\n{exFilePath}\n\n现在您可以点击【中止】关闭应用程序，【重试】重启应用程序，【忽略】忽略本次错误。", "意外错误 - 高考倒计时", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                if (result != DialogResult.Ignore)
                {
                    Exit(result == DialogResult.Retry);
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
                File.Delete(DotNetConfig);
                File.WriteAllText(DotNetConfig, @"<?xml version=""1.0"" encoding=""utf-8"" ?><configuration><appSettings><add key=""EnableWindowsFormsHighDpiAutoResizing"" value=""true""/></appSettings></configuration>");
                ProcessHelper.Run("attrib", $"+s +h \"{DotNetConfig}\"");
            }
            catch { }
        }
    }
}