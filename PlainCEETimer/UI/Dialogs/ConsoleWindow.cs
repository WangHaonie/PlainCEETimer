using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class ConsoleWindow() : AppDialog(AppFormParam.AllControl)
    {
        private int ConsoleTimerTick;
        private bool ElevateNeeded;
        private bool IsRunning;
        private string Key;
        private string ExePath;
        private string ExeArgs;
        private ConsoleParam Param;
        private Action<ConsoleWindow> Complete;
        private Label LabelMessage;
        private MenuItem ContextCopy;
        private Process ElevatedProc;
        private RichTextBox ConsoleBox;
        private const int WM_VSCROLL = 0x0115;
        private const int SB_BOTTOM = 7;
        private readonly Timer ConsoleTimer = new();

        protected override void OnInitializing()
        {
            Text = "Console Window - 高考倒计时";
            BackColor = Color.Black;
            base.OnInitializing();

            this.AddControls(b =>
            [
                ConsoleBox = b.New<RichTextBox>(600, 200, null).With(c =>
                {
                    c.Font = new("Consolas", 9F);
                    c.ForeColor = Color.FromArgb(204, 204, 204);
                    c.BackColor = Color.Black;
                    c.BorderStyle = BorderStyle.None;
                    c.ReadOnly = true;
                    c.HideSelection = false;
                    c.WordWrap = true;
                }),

                LabelMessage = b.Label("请稍候...").With(c => c.ForeColor = Color.White)
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

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeCommonButtonsR(ButtonA, ButtonB, ConsoleBox, 1, 3);
            CenterControlY(LabelMessage, ButtonA);
        }

        protected override void OnShown()
        {
            TaskbarProgress.Initialize(this);

            if (UACHelper.EnsureUAC(MessageX))
            {
                TaskbarProgress.SetState(TaskbarProgressState.Indeterminate);
                ConsoleTimer_Tick(null, null);
                ConsoleTimer.Start();

                if (UACHelper.IsAdmin)
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
                    ElevateNeeded = true;

                    new Action(() =>
                    {
                        using var server = new NamedPipeServerStream(Key, PipeDirection.In, 1, PipeTransmissionMode.Message);
                        using var reader = new StreamReader(server);
                        server.WaitForConnection();
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Equals("```3", StringComparison.Ordinal))
                            {
                                Final();
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
                            ElevatedProc = ProcessHelper.RunElevated(App.CurrentExecutablePath, "/run " + Key + " " + ExePath + " " + ExeArgs);
                        }
                        catch (Exception ex)
                        {
                            SafeWrite(ProcessHelper.GetExceptionMessage(ex));
                            Final();
                        }
                    }).Start();
                }
            }
            else
            {
                Final();
                Close();
            }
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsRunning && !ElevatedProc.HasExited;
        }

        protected override void OnClosed()
        {
            TaskbarProgress.Release();
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
                TaskbarProgress.SetState(TaskbarProgressState.Normal);
                TaskbarProgress.SetValue(1UL, 1UL);
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
                Natives.SendMessage(ConsoleBox.Handle, WM_VSCROLL, new(SB_BOTTOM), IntPtr.Zero);
            }
        }
    }
}
