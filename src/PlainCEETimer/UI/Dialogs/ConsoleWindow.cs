using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ConsoleWindow : AppDialog
{
    protected override AppFormParam Params => AppFormParam.AllControl;

    private int ConsoleTimerTick;
    private bool ElevateNeeded;
    private bool IsRunning;
    private string Key;
    private string ExePath;
    private string ExeArgs;
    private ConsoleParam Param;
    private Action<ConsoleWindow> Complete;
    private PlainLabel LabelMessage;
    private MenuItem ContextCopy;
    private Process ElevatedProc;
    private PlainTextBox ConsoleBox;
    private TaskbarProgress tbp;
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
        ButtonA.Text = "确认(&O)";
        ButtonA.Visible = false;
        ButtonB.Text = "关闭(&C)";
        ButtonB.Enabled = false;

        if ((Param & ConsoleParam.NoMenu) != ConsoleParam.NoMenu)
        {
            ConsoleBox.ContextMenu = ContextMenuBuilder.Build(b =>
            [
                ContextCopy = b.Item("复制(&C)", (_, _) => Clipboard.SetText(ConsoleBox.SelectedText)),
                b.Separator(),
                b.Item("全选(&A)", (_, _) => ConsoleBox.SelectAll())
            ], (_, _) => ContextCopy.Enabled = !string.IsNullOrWhiteSpace(ConsoleBox.SelectedText));
        }
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeCommonButtonsR(ButtonA, ButtonB, ConsoleBox, 1, 3);
        CenterControlY(LabelMessage, ButtonA);
    }

    protected override void OnShown()
    {
        tbp = new(Handle);

        tbp.SetState(ProgressStyle.Indeterminate);
        ConsoleTimer_Tick(null, null);
        ConsoleTimer.Start();

        if (UacHelper.IsAdmin)
        {
            new Action(() =>
            {
                try
                {
                    ProcessHelper.Run(ExePath, ExeArgs, (proc, _) =>
                    {
                        SafeWrite(ProcessHelper.GetExitMessage(proc));
                    }, (_, e) => SafeWrite(e.Data));
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
            ElevateNeeded = true;

            new Action(() =>
            {
                using var server = new NamedPipeServerStream(Key, PipeDirection.In, 1, PipeTransmissionMode.Message);
                using var reader = new StreamReader(server);
                server.WaitForConnection();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "```3")
                    {
                        Final();
                        break;
                    }
                    else
                    {
                        SafeWrite(line);
                    }
                }
            }).Start();

            new Action(() =>
            {
                try
                {
                    ElevatedProc = ProcessHelper.RunElevated(App.ExecutablePath, "/run " + Key + " " + ExePath + " " + ExeArgs);
                }
                catch (Exception ex)
                {
                    SafeWrite(ProcessHelper.GetExceptionMessage(ex));
                    Final();
                }
            }).Start();
        }

        Final();
    }

    protected override bool OnClosing(CloseReason closeReason)
    {
        return IsRunning || (ElevatedProc != null && !ElevatedProc.HasExited);
    }

    public void UpdateState(string text)
    {
        SafeExecute(() => LabelMessage.Text += text);
    }

    public static DialogResult Run(string path, string args, Action<ConsoleWindow> onComplete, ConsoleParam param = ConsoleParam.None)
    {
        return new ConsoleWindow
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
            if (ElevateNeeded && IsRunning && ElevatedProc.HasExited)
            {
                Final();
            }
        }
        catch { }
    }

    private void Final()
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

            if ((Param & ConsoleParam.ShowLeftButton) == ConsoleParam.ShowLeftButton)
            {
                ButtonA.Visible = true;
                ButtonA.Enabled = true;
            }

            if ((Param & ConsoleParam.AutoClose) == ConsoleParam.AutoClose)
            {
                Close();
            }
        });
    }

    private void SafeWrite(string line)
    {
        SafeExecute(() => Write(line));
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
}
