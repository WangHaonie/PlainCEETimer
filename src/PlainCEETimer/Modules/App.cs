using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Update;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules;

internal static class App
{
    public static string ExecutableDir => field ??= AppDomain.CurrentDomain.BaseDirectory;

    public static string ExecutablePath => field ??= Application.ExecutablePath;

    public static string ConfigFilePath => field ??= $"{ExecutableDir}{AppNameEng}.config";

    public static Icon AppIcon => appIcon ??= HICON.FromFile(ExecutablePath).ToIcon();

    public static AppConfig AppConfig { get; private set; }

    public static Version VersionObject => field ??= Version.Parse(AppInfo.Version);

    internal static event Action ActivateMain;

    internal static event Action AppExit;

    public const string AppName = "高考倒计时 by WangHaonie";
    public const string AppNameEng = "PlainCEETimer";
    public const string AppNameEngOld = "CEETimerCSharpWinForms";
    public const string CopyrightInfo = "Copyright © 2023-2026 WangHaonie";
    public const string OriginalFileName = $"{AppNameEng}.exe";
    public const string NativesDll = "PlainCEETimer.Natives.dll";
    public const string User32Dll = "user32.dll";
    public const string UxThemeDll = "uxtheme.dll";
    public const string Shell32Dll = "shell32.dll";
    public const string Gdi32Dll = "gdi32.dll";
    public const string Kernel32Dll = "kernel32.dll";
    public const string DateTimeFormat = "yyyyMMddHHmmss";
    private const string UEFilePrefix = "UnhandledException_";

    private static bool IsMainProcess;
    private static bool IsClosing;
    private static string AllArgs;
    private static Icon appIcon;
    private static Mutex MainMutex;
    private static readonly object _lock = new();
    private static readonly string PipeName = $"{AppNameEngOld}_[34c14833-98da-49f7-a2ab-369e88e73b95]";
    private static readonly string ExecutableName = Path.GetFileName(ExecutablePath);
    private static readonly AppMessageBox MessageX = AppMessageBox.Instance;

    [STAThread]
    private static void Main(string[] args)
    {
        Application.SetCompatibleTextRenderingDefault(false);
#if !DEBUG
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => HandleException(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException((Exception)e.ExceptionObject);
#endif
        MainMutex = new(true, $"{AppNameEngOld}_MUTEX_61c0097d-3682-421c-84e6-70ca37dc31dd_[A3F8B92E6D14]", out IsMainProcess);
        AllArgs = string.Join(" ", args);

        if (IsMainProcess)
        {
            new Action(StartPipeServer).Start();
        }

        if (!StartProgram(args.Length, CliOption.Parse(args)))
        {
            StartPipeClient();
            Exit();
        }
    }

    private static bool StartProgram(int argc, CliOption args)
    {
        InternalInit();

        if (argc != 0)
        {
            Win32.AllocConsole();
        }

        if (IsMainProcess)
        {
            if (ExecutableName.Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
            {
                if (argc == 0)
                {
                    new Action(UacHelper.CheckAdmin).Start();
                    Win32TaskScheduler.Initialize();
                    Application.Run(new MainForm());
                }
                else
                {
                    switch (args.FirstOption)
                    {
                        case "?":
                        case "h":
                            PrintHelp();
                            break;
                        case "ac":
                            UacHelper.PrintReport();
                            break;
                        case "fr":
                            new Updater().InteractiveDownload(args.GetFirst(), args.Get("src"));
                            break;
                        case "op":
                            UacHelper.CheckAdmin();
                            new OptimizationHelper(args.Defined("auto")).Optimize();
                            break;
                        case "lnk":
                            ShellLink.CreateAppShortcut(args.Defined("custom"));
                            break;
                        case "uninst":
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
                MessageX.Error($"为了您的使用体验，请不要更改程序文件名! 程序将在该消息框自动关闭后尝试自动恢复到原文件名，若自动恢复失败请手动改回。\n\n当前文件名: {ExecutableName}\n原始文件名: {OriginalFileName}", autoClose: true);
                ProcessHelper.Run("cmd", $"/c ren \"{ExecutablePath}\" {OriginalFileName} && start \"\" \"{ExecutableDir}{OriginalFileName}\" {AllArgs}");
            }

            return true;
        }
        else if (!(args.FirstOption == "run" && StartPipeClient(args.GetFirst(), args.Get("exe"), args.Get("args"))))
        {
            if (argc != 0)
            {
                ConsoleHelper.Instance.WriteLine("请先退出已打开的实例再使用命令行功能。", ConsoleColor.Red);
            }
        }

        return false;
    }

    public static void PopupAbortRetryIgnore(string message, string title)
    {
        var result = MessageBox.Show(message, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

        if (result != DialogResult.Ignore)
        {
            Exit(result == DialogResult.Retry, true);
        }
    }

    public static string WriteException(Exception ex)
    {
        lock (_lock)
        {
            var now = DateTime.Now;
            var content = $"—————————————————— {GetAppDescription()} - {now.Format()} ——————————————————\n{ex}";
            var exFileName = $"{UEFilePrefix}{now.ToString(DateTimeFormat)}.txt";
            var exFilePath = $"{ExecutableDir}{exFileName}";
            File.AppendAllText(exFilePath, content);
            return "安装目录：\n" + exFileName;
        }
    }

    internal static void OnActivateMain()
    {
        Win32UI.ActivateUnmanagedWindows();
        ActivateMain?.Invoke();
    }

    private static void PrintHelp()
    {
        ConsoleHelper.Instance
            .WriteLine(GetAppDescription(), ConsoleColor.White)
            .WriteLine(AppName, ConsoleColor.White)
            .WriteLine()
            .WriteLine("用法:")
            .WriteLine($"{AppNameEng} [options]", ConsoleColor.White)
            .WriteLine()
            .WriteLine("命令行选项:")
            .WriteLine("/?, /h", ConsoleColor.White)
                .WriteLine("\t显示此帮助信息.")
            .WriteLine("/ac", ConsoleColor.White)
                .WriteLine("\t检测当前用户是否具有管理员权限.")
            .WriteLine("/fr [<版本号>] [/src <UpdateSource>]", ConsoleColor.White)
                .WriteLine("\t强制下载并安装指定的版本, 留空则当前版本, 推荐在特殊情况下使用, 不支持老版本.")
                .WriteLine($"\t可用的更新源: [ {nameof(UpdateSource.GiteeStable)},0 | {nameof(UpdateSource.GitHubStable)},1 | {nameof(UpdateSource.GiteeCI)},2 | {nameof(UpdateSource.GitHubCI)},3 ]")
                .WriteLine($"\t默认 {nameof(UpdateSource.GiteeStable)},0")
            .WriteLine("/lnk [/custom]", ConsoleColor.White)
                .WriteLine("\t向开始菜单文件夹和桌面创建指向本程序的快捷方式. /custom 表示自行选择保存快捷方式的文件夹.")
            .WriteLine("/op", ConsoleColor.White)
                .WriteLine("\t优化当前程序集, 提升运行速度.");
    }

    private static void DeleteExtraFiles()
    {
        try
        {
            foreach (var uefile in Directory.GetFiles(ExecutableDir, $"{UEFilePrefix}*.txt"))
            {
                File.Delete(uefile);
            }
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
                OnActivateMain();
            }
        }
        catch { }
    }

    private static bool StartPipeClient(string pipe = null, string path = null, string args = null)
    {
        var isRedirector = !string.IsNullOrEmpty(pipe);

        try
        {
            using var client = new NamedPipeClientStream(".", isRedirector ? pipe : PipeName, PipeDirection.Out);
            client.Connect(isRedirector ? 500 : 1000);
            using var w = new StreamWriter(client);

            if (isRedirector)
            {
                ProcessHelper.RunRedirector(w, path, args);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void InternalInit()
    {
        AppConfig = ConfigValidator.ReadConfig();
        ThemeManager.Initialize();
        DefaultValues.InitEssentials(true);
        ConfigValidator.Validate();
        ConfigurationManager.AppSettings.Set("EnableWindowsFormsHighDpiAutoResizing", "true");
        Application.EnableVisualStyles();
    }

    public static void Exit(bool restart = false, bool useArgs = false)
    {
        IsClosing = true;
        appIcon.Destory();
        AppExit?.Invoke();
        AppExit = null;
        ActivateMain = null;

        if (MainMutex != null)
        {
            if (IsMainProcess)
            {
                MainMutex.ReleaseMutex();
            }

            MainMutex.Dispose();
            MainMutex = null;
        }

        if (restart)
        {
            ProcessHelper.Run(ExecutablePath, useArgs ? AllArgs : null);
        }

        Environment.Exit(0);
    }

    private static void HandleException(Exception ex)
    {
        if (!IsClosing)
        {
            PopupAbortRetryIgnore($"程序出现意外错误，非常抱歉给您带来不便！\n\n个别常见错误可能收录于用户手册中，请到仓库首页访问并查询可能的解决办法。若无则建议您及时将相关内容提交到 Issues 以帮助我们定位并解决问题。\n\n错误信息：\n{ex.Message}\n\n详细错误信息已保存至{WriteException(ex)}\n\n现在您可以点击【中止】关闭应用程序，【重试】重启应用程序，【忽略】忽略本次错误。", "意外错误 - 高考倒计时");
        }
    }

    private static string GetAppDescription()
    {
        return $"{AppNameEng} v{AppInfo.Version} ({AppInfo.BuildDate}, {AppInfo.CommitSHA})";
    }
}