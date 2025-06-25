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
        public bool AutoClose { get; set; }
        public bool EnableLeftButton { get; set; }

        public event Action Exit;
        public event Action Error;
        public event Action Complete;

        private bool ElevateNeeded;
        private bool IsRunning;
        private bool IsSafeThread;
        private int ConsoleTimerTick;
        private string Key;
        private string LineFromClient;
        private string[] Command;
        private Label LabelMessage;
        private MenuItem ContextCopy;
        private NamedPipeServerStream Server;
        private Process ElevatedProc;
        private RichTextBox ConsoleBox;
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
            ButtonB.Text = "关闭(&C)";
            ButtonB.Enabled = false;

            var menu = ContextMenuBuilder.Build(b =>
            [
                ContextCopy = b.Item("复制(&C)", (_, _) => Clipboard.SetText(ConsoleBox.SelectedText)),
                b.Separator(),
                b.Item("全选(&A)", (_, _) => ConsoleBox.SelectAll())
            ]);

            menu.Popup += (_, _) => ContextCopy.Enabled = !string.IsNullOrWhiteSpace(ConsoleBox.SelectedText);
            ConsoleBox.ContextMenu = menu;
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
                var name = Command[0];
                var args = Command[1];
                TaskbarProgress.SetState(TaskbarProgressState.Indeterminate);
                ConsoleTimer_Tick(null, null);
                ConsoleTimer.Start();

                if (UACHelper.IsAdmin)
                {
                    new Action(() =>
                    {
                        try
                        {
                            ProcessHelper.Run(name, args, (proc, _) =>
                            {
                                SafeWrite(ProcessHelper.GetExitMessage(proc));
                                OnExit();
                            }, (_, e) => SafeWrite(e.Data));
                        }
                        catch (Exception ex)
                        {
                            SafeWrite(ProcessHelper.GetExceptionMessage(ex));
                            OnError();
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
                        try
                        {
                            ElevatedProc = ProcessHelper.RunElevated(App.CurrentExecutablePath, "/run " + Key + " " + name + " " + args);
                        }
                        catch (Exception ex)
                        {
                            SafeWrite(ProcessHelper.GetExceptionMessage(ex));
                            OnError();
                            Final();
                        }
                    }).Start();

                    new Action(() =>
                    {
                        Server = new(Key, PipeDirection.In, 1, PipeTransmissionMode.Message);
                        using var reader = new StreamReader(Server);
                        Server.WaitForConnection();

                        while ((LineFromClient = reader.ReadLine()) != null)
                        {
                            if (LineFromClient.Has("```1"))
                            {
                                OnExit();
                            }
                            else if (LineFromClient.Has("```2"))
                            {
                                OnError();
                            }
                            else if (LineFromClient.Has("```3"))
                            {
                                Final();
                            }
                            else
                            {
                                SafeWrite(LineFromClient);
                            }
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

        public void Run(params string[] command)
        {
            IsSafeThread = InvokeRequired;
            IsRunning = true;
            Command = command;
            Key = $"PlainCEETimer.CommandOutputRedirector_{Guid.NewGuid()}";
            ShowDialog();
        }

        public void SafeWrite(string line)
        {
            if (IsSafeThread)
            {
                Write(line);
            }
            else
            {
                Invoke(() => Write(line));
            }
        }

        public void UpdateState(string text)
        {
            SafeExecute(() => LabelMessage.Text += text);
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

        private void OnExit()
        {
            Exit?.Invoke();
        }

        private void OnError()
        {
            Error?.Invoke();
        }

        private void Final()
        {
            IsRunning = false;
            ConsoleTimer.Stop();
            LabelMessage.Text = $"命令已完成 ({ConsoleTimerTick} s)。";
            TaskbarProgress.SetState(TaskbarProgressState.Normal);
            TaskbarProgress.SetValue(1UL, 1UL);
            Complete?.Invoke();

            SafeExecute(() =>
            {
                ButtonB.Enabled = true;

                if (EnableLeftButton)
                {
                    ButtonA.Enabled = true;
                }

                if (AutoClose)
                {
                    Close();
                }
            });
        }

        private void SafeExecute(Action action)
        {
            Invoke(action);
        }

        private void Write(string line)
        {
            if (line != null)
            {
                ConsoleBox.AppendText(line);
                ConsoleBox.AppendText("\r\n");
                CommonDialogHelper.SendMessageW(ConsoleBox.Handle, 0x115, (IntPtr)7, IntPtr.Zero);
            }
        }
    }
}
