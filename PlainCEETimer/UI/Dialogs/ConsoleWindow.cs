using System;
using System.ComponentModel;
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
        public event Action Exit;
        public event Action Error;
        public event Action Complete;
        public bool AutoClose { get; set; }
        public bool EnableLeftButton { get; set; }

        private RichTextBox ConsoleBox;
        private NamedPipeServerStream Server;
        private MenuItem ContextCopy;
        private Label LabelMessage;
        private readonly Timer ConsoleTimer = new();
        private int ConsoleTimerTick;
        private string LineFromClient;
        private bool IsSafeThread;
        private bool IsRunning;
        private string[] Command;
        private string Key;
        private bool ElevateNeeded;
        private Process ElevatedProc;

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
                ContextCopy = b.Item("复制(&C)", (_, _) =>
                {
                    Clipboard.SetText(ConsoleBox.SelectedText);
                })
            ]);

            menu.Popup += (_, _) => ContextCopy.Enabled = !string.IsNullOrWhiteSpace(ConsoleBox.SelectedText);
            ConsoleBox.ContextMenu = menu;
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
                            var proc = Process.Start(new ProcessStartInfo()
                            {
                                FileName = name,
                                Arguments = args,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            });

                            proc.EnableRaisingEvents = true;

                            proc.Exited += (_, _) =>
                            {
                                SafeWrite("===================================");
                                SafeWrite($"命令执行完成，返回值为 0x{proc.ExitCode:X}。可以关闭此窗口。");
                                SafeWrite("===================================");
                                Exit?.Invoke();
                            };

                            proc.OutputDataReceived += (_, e) => SafeWrite(e.Data);
                            proc.BeginOutputReadLine();
                            proc.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            SafeWrite($"{ex}\n\n出现未知错误，请及时向我们反馈相关信息。");
                            Error?.Invoke();
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
                            ElevatedProc = new Process()
                            {
                                StartInfo = new ProcessStartInfo()
                                {
                                    FileName = App.CurrentExecutablePath,
                                    Arguments = "/run " + Key + " " + name + " " + args,
                                    Verb = "runas",
                                    UseShellExecute = true,
                                    CreateNoWindow = true
                                },

                                EnableRaisingEvents = true
                            };

                            ElevatedProc.Start();
                        }
                        catch (Win32Exception ex) when (ex.NativeErrorCode == Constants.ERROR_CANCELLED)
                        {
                            /*
                            
                            检测用户是否点击了 UAC 提示框的 "否" 参考:

                            c# - Run process as administrator from a non-admin application - Stack Overflow
                            https://stackoverflow.com/a/20872219/21094697

                            */

                            SafeWrite($"{ex}\n\n授权失败，请在 UAC 对话框弹出时点击 \"是\"。");
                            Error?.Invoke();
                            Final();
                        }
                        catch (Exception ex)
                        {
                            SafeWrite($"{ex}\n\n出现未知错误，请及时向我们反馈相关信息。");
                            Error?.Invoke();
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
                                Exit?.Invoke();
                            }
                            else if (LineFromClient.Has("```2"))
                            {
                                Error?.Invoke();
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

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeCommonButtonsR(ButtonA, ButtonB, ConsoleBox, 1, 3);
            CenterControlY(LabelMessage, ButtonA);
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
            Invoke(() => LabelMessage.Text += text);
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

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsRunning && !ElevatedProc.HasExited;
        }

        protected override void OnClosed()
        {
            TaskbarProgress.Release();
        }
    }
}
