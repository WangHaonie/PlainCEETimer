using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules;

internal static class App
{
    public static string ExecutableDir => field ??= AppDomain.CurrentDomain.BaseDirectory;
    public static string ExecutablePath => field ??= Application.ExecutablePath;
    public static string ConfigFilePath => field ??= $"{ExecutableDir}{AppNameEng}.config";
    public static AppConfig AppConfig { get; private set; }
    public static Icon AppIcon { get; private set; }
    public static Version AppVersionObject => field ??= Version.Parse(AppVersion);

    internal static event Action ActivateMain;
    internal static event Action AppExit;

    public const string AppName = "高考倒计时 by WangHaonie";
    public const string AppNameEng = "PlainCEETimer";
    public const string AppNameEngOld = "CEETimerCSharpWinForms";
    public const string AppVersion = "5.0.8";
    public const string AppBuildDate = "2025/12/29";
    public const string CopyrightInfo = "Copyright © 2023-2025 WangHaonie";
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
    private static string[] Args;
    private static int ArgsLength;
    private static Mutex MainMutex;
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

        if (IsMainProcess)
        {
            new Action(StartPipeServer).Start();
        }

        if (!StartProgram(args))
        {
            StartPipeClient();
            Exit();
        }
    }

    private static bool StartProgram(string[] args)
    {
        AppIcon = HICON.FromFile(ExecutablePath).ToIcon();
        Args = args.ArraySelect(x => x.ToLower());
        ArgsLength = Args.Length;
        var AllArgs = string.Join(" ", args);
        InitConfig();

        if (IsMainProcess)
        {
            if (ExecutableName.Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
            {
                if (ArgsLength == 0)
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
                            UacHelper.PopupReport();
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
                MessageX.Error($"为了您的使用体验，请不要更改程序文件名! 程序将在该消息框自动关闭后尝试自动恢复到原文件名，若自动恢复失败请手动改回。\n\n当前文件名: {ExecutableName}\n原始文件名: {OriginalFileName}", autoClose: true);
                ProcessHelper.Run("cmd", $"/c ren \"{ExecutablePath}\" {OriginalFileName} && start \"\" \"{ExecutableDir}{OriginalFileName}\" {AllArgs}");
            }

            return true;
        }
        else if (!(ArgsLength > 3 && Args[0] == "/run" && StartPipeClient(args[1], args[2], string.Join(" ", args.Skip(3)))))
        {
            if (ArgsLength != 0)
            {
                MessageX.Error("请先退出已打开的实例再使用命令行功能。", autoClose: true);
            }
        }

        return false;
    }

    public static void PopupAbortRetryIgnore(string message, string title)
    {
        var result = MessageBox.Show(message, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

        if (result != DialogResult.Ignore)
        {
            Exit(result == DialogResult.Retry);
        }
    }

    public static void Exit(bool restart = false)
    {
        IsClosing = true;
        AppIcon.Destory();
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
            ProcessHelper.Run(ExecutablePath, null);
        }

        Environment.Exit(0);
    }

    public static string WriteException(Exception ex)
    {
        var now = DateTime.Now;
        var content = $"—————————————————— {AppNameEng} v{AppVersion} - {now.Format()} ——————————————————\n{ex}";
        var exFileName = $"{UEFilePrefix}{now.ToString(DateTimeFormat)}.txt";
        var exFilePath = $"{ExecutableDir}{exFileName}";
        File.AppendAllText(exFilePath, content);
        return "安装目录：\n" + exFileName;
    }

    internal static void OnActivateMain()
    {
        Win32UI.ActivateUnmanagedWindows();
        ActivateMain?.Invoke();
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

    private static string GetNextArg()
    {
        if (ArgsLength > 1)
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

    private static void InitConfig()
    {
        AppConfig = Validator.ReadConfig();
        ThemeManager.Initialize();
        CountdownRule[] rules = AppConfig.GlobalRules;

        if (rules == null || rules.Length < 3)
        {
            var f = Validator.ValidateNeeded;
            Validator.ValidateNeeded = false;
            var r = DefaultValues.GlobalDefaultRules.Copy();
            r.PopulateWith(rules);
            AppConfig.GlobalRules = r;
            Validator.ValidateNeeded = f;
            Validator.DemandConfig();
        }
    }

    private static void HandleException(Exception ex)
    {
        if (!IsClosing)
        {
            PopupAbortRetryIgnore($"程序出现意外错误，非常抱歉给您带来不便！\n\n个别常见错误可能收录于用户手册中，请到仓库首页访问并查询可能的解决办法。若无则建议您及时将相关内容提交到 Issues 以帮助我们定位并解决问题。\n\n错误信息：\n{ex.Message}\n\n详细错误信息已保存至{WriteException(ex)}\n\n现在您可以点击【中止】关闭应用程序，【重试】重启应用程序，【忽略】忽略本次错误。", "意外错误 - 高考倒计时");
        }
    }
}