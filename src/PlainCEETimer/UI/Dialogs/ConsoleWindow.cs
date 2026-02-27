using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;
using Timer = System.Windows.Forms.Timer;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ConsoleWindow : AppDialog
{
    protected override AppFormParam Params => AppFormParam.AllControl;

    private int finalCount;
    private int closeClickCount;
    private int ConsoleTimerTick;
    private bool IsRunning;
    private string Key;
    private string ExePath;
    private string ExeArgs;
    private ConsoleParam Param;
    private DateTime taskStartTime;
    private DateTime lastClickTime;
    private Action<ConsoleWindow> Complete;
    private PlainLabel LabelMessage;
    private MenuItem MenuItemCopy;
    private PlainTextBox ConsoleBox;
    private TaskbarProgress tbp;
    private PlainToolTip ToolTipCloseInfo;
    private Process ExternalProc;
    private readonly Timer ConsoleTimer = new();

    protected override void OnInitializing()
    {
        Text = "命令输出 - 高考倒计时";
        base.OnInitializing();

        this.AddControls(b =>
        [
            ConsoleBox = b.TextArea(600, 200, null).With(x =>
            {
                x.ReadOnly = true;
                x.BorderStyle = BorderStyle.None;
                x.Font = new("Consolas", 9F);

                if (ThemeManager.ShouldUseDarkMode)
                {
                    x.ForeColor = Colors.DarkForeConsole;
                    x.BackColor = Colors.DarkBackConsole;
                }
            }),

            LabelMessage = b.Label("请稍候...")
        ]);

        ConsoleTimer.Interval = 1000;
        ConsoleTimer.Tick += ConsoleTimer_Tick;
        ButtonB.Text = "关闭(&C)";
        ButtonB.Enabled = false;

        ToolTipCloseInfo = new()
        {
            ToolTipIcon = ToolTipIcon.Info,
            ToolTipTitle = "提示"
        };

        if ((Param & ConsoleParam.NoMenu) != ConsoleParam.NoMenu)
        {
            ConsoleBox.AttachContextMenu(b =>
            [
                MenuItemCopy = b.Item("复制(&C)", (_, _) => Clipboard.SetText(ConsoleBox.SelectedText)),
                b.Separator(),
                b.Item("全选(&A)", (_, _) => ConsoleBox.SelectAll())
            ], (_, _) => MenuItemCopy.Enabled = !string.IsNullOrWhiteSpace(ConsoleBox.SelectedText), out _);
        }
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeCommonButtonsR(ButtonA, ButtonB, ConsoleBox, 1, 3);
        CenterControlY(LabelMessage, ButtonA);
        InitWindowSize(ButtonB, 3, 3);
        ButtonA.Delete();
    }

    protected override void OnShown()
    {
        tbp = new(Handle);
        tbp.SetState(ProgressStyle.Indeterminate);
        ConsoleTimer_Tick(null, null);
        ConsoleTimer.Start();
        taskStartTime = DateTime.Now;
        ToolTipCloseInfo.InitStyle();

        if (UacHelper.IsAdmin)
        {
            new Action(() =>
            {
                try
                {
                    WaitForExit(ProcessHelper.Run(ExePath, ExeArgs, (proc, _) =>
                    {
                        SafeWrite(ProcessHelper.GetExitMessage(proc));
                    }, (_, e) => SafeWrite(e.Data)));
                }
                catch (Exception ex)
                {
                    SafeWrite(ProcessHelper.GetExceptionMessage(ex));
                }
                finally
                {
                    Final();
                }
            }).Start();
        }
        else
        {
            MessageX.Warn("检测到当前用户不具有管理员权限，将尝试自动提权。\n\n若弹出 UAC 对话框，请点击 是。" + (UacHelper.IsUacDisabled ? "\n(当前系统貌似已禁用 UAC，可能会导致提权失败)" : ""));

            new Action(() =>
            {
                using var server = new NamedPipeServerStream(Key, PipeDirection.In, 1, PipeTransmissionMode.Message);
                using var reader = new StreamReader(server);
                server.WaitForConnection();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    SafeWrite(line);
                }
            }).Start();

            new Action(() =>
            {
                try
                {
                    WaitForExit(ProcessHelper.RunElevated(App.ExecutablePath, new CliOption()
                        .Add("/run").Add(Key)
                        .Add("/exe").Add(ExePath)
                        .Add("/args").Add(ExeArgs)
                        .ToArgs()));
                }
                catch (Exception ex)
                {
                    SafeWrite(ProcessHelper.GetExceptionMessage(ex));
                }
                finally
                {
                    Final();
                }
            }).Start();
        }
    }

    protected override bool OnClosing(CloseReason closeReason)
    {
        var flag = IsRunning || (ExternalProc?.HasExited == false);

        if (!flag)
        {
            return false;
        }

        var now = DateTime.Now;
        var uptime = now - taskStartTime;

        if (uptime.TotalSeconds <= 10D)
        {
            return true;
        }

        var cancel = true;

        if ((now - lastClickTime).TotalSeconds > 3D)
        {
            closeClickCount = 1;
        }
        else
        {
            closeClickCount++;
        }

        lastClickTime = now;

        if (closeClickCount >= 10)
        {
            cancel = false;
            Win32.KillProcessTree(ExternalProc.Id);
            ToolTipCloseInfo.Hide(this);
            Final();
        }
        else
        {
            var fh = FontHeight * 2;
            ToolTipCloseInfo.Show($"再点击 {10 - closeClickCount} 次强制关闭本窗口", this, new Point(ClientSize.Width - fh, fh), 2000);
        }

        return cancel;
    }

    public static void Run(string path, string args, Action<ConsoleWindow> onComplete, ConsoleParam param = ConsoleParam.None)
    {
        new ConsoleWindow
        {
            Param = param,
            Complete = onComplete,
            IsRunning = true,
            ExePath = path,
            ExeArgs = args,
            Key = $"PlainCEETimer.ShExecCommandOutputRedirector_{Guid.NewGuid()}"
        }.ShowDialog();
    }

    private void ConsoleTimer_Tick(object sender, EventArgs e)
    {
        LabelMessage.Text = $"正在运行中... ({ConsoleTimerTick} s)";
        ConsoleTimerTick++;

        try
        {
            if (ExternalProc != null && ExternalProc.HasExited)
            {
                Final();
            }
        }
        catch { }
    }

    private void SafeWrite(string line)
    {
        SafeExecute(() => Write(line));
    }

    private void WaitForExit(Process process)
    {
        ExternalProc = process;
        process.WaitForExit();
    }

    private void SafeExecute(Action action)
    {
        if (InvokeRequired)
        {
            BeginInvoke(action);
        }
        else
        {
            action();
        }
    }

    private void Write(string line)
    {
        if (line != null)
        {
            ConsoleBox.AppendText(line);
            ConsoleBox.AppendText("\r\n");
        }
    }

    private void Final()
    {
        if (Interlocked.Exchange(ref finalCount, 1) == 0)
        {
            SafeExecute(() =>
            {
                IsRunning = false;
                ConsoleTimer.Stop();
                LabelMessage.Text = $"命令已完成 ({ConsoleTimerTick} s)。";
                tbp.SetState(ProgressStyle.Normal);
                tbp.SetValue(1, 1);
                Complete?.Invoke(this);
                ButtonB.Enabled = true;

                if ((Param & ConsoleParam.AutoClose) == ConsoleParam.AutoClose)
                {
                    Close();
                }
            });
        }
    }
}
