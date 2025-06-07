using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Dialogs;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.WinForms;

namespace PlainCEETimer.Forms
{
    public sealed partial class SettingsForm : AppForm
    {
        public bool RefreshNeeded { get; private set; }

        private bool IsColorLabelsDragging;
        private bool IsSyncingTime;
        private bool UserChanged;
        private bool InvokeChangeRequired;
        private bool IsFunnyClick;
        private bool IsSetStartUp;
        private int SelectedTheme;
        private string[] EditedCustomTexts;
        private ContextMenu ContextMenuDefaultColor;
        private CustomRuleObject[] EditedCustomRules;
        private ExamInfoObject[] EditedExamInfo;
        private Font SelectedFont;
        private Label[] ColorLabels;
        private Label[] ColorPreviewLabels;
        private NavigationBar NavBar;
        private ColorSetObject[] SelectedColors;
        private readonly ConfigObject AppConfig = App.AppConfig;

        public SettingsForm() : base(AppFormParam.CompositedStyle | AppFormParam.CenterScreen)
        {
            InitializeComponent();

            this.AddControls(b =>
            [
                b.Container<Panel>(57, 0, 332, 259,
                [
                    b.Page(
                    [
                        b.Container<PlainGroupBox>(5, 3, 323, 67, "考试信息",
                        [
                            b.Label(6, 19, "在此添加考试信息以启动倒计时。"),

                            b.Button(8, 37, "管理(&G)", (_, _) =>
                            {
                                ExamInfoManager ExamMan = new()
                                {
                                    Data = EditedExamInfo
                                };

                                if (ExamMan.ShowDialog() == DialogResult.OK)
                                {
                                    EditedExamInfo = ExamMan.Data;
                                    SettingsChanged();
                                }
                            }),

                            b.CheckBox(125, 40, "启用自动切换", (_, _) =>
                            {
                                ComboBoxAutoSwitchIntervel.Enabled = CheckBoxAutoSwitch.Checked;
                                SettingsChanged();
                            }),

                            b.ComboBox(231, 37, 86, false, SettingsChanged,
                            [
                                new("10 秒", 0),
                                new("15 秒", 1),
                                new("30 秒", 2),
                                new("45 秒", 3),
                                new("1 分钟", 4),
                                new("2 分钟", 5),
                                new("3 分钟", 6),
                                new("5 分钟", 7),
                                new("10 分钟", 8),
                                new("15 分钟", 9),
                                new("30 分钟", 10),
                                new("45 分钟", 11),
                                new("1 小时", 12),
                            ])
                        ]),

                        b.Container<PlainGroupBox>(5, 76, 323, 148, "其他",
                        [
                            b.CheckBox(9, 22, "系统运行时自动启动倒计时(&B)", SettingsChanged),
                            b.CheckBox(9, 44, "自动清理倒计时占用的运行内存(&M)", SettingsChanged),

                            b.CheckBox(9, 69, "顶置倒计时窗口(&T)", (_, _) =>
                            {
                                CheckBoxUniTopMost.Enabled = CheckBoxTopMost.Checked;

                                if (CheckBoxUniTopMost.Checked && !CheckBoxTopMost.Checked)
                                {
                                    CheckBoxUniTopMost.Checked = false;
                                    CheckBoxUniTopMost.Enabled = false;
                                }

                                ChangePptsvcStyle(null, null);
                            }, true, true),

                            b.CheckBox(157, 69, "顶置其他窗口(&U)", SettingsChanged),

                            b.CheckBox(9, 94, "在托盘区域显示通知图标(&I)", (_, _) =>
                            {
                                CheckBoxTrayText.Enabled = CheckBoxTrayIcon.Checked;
                                CheckBoxTrayText.Checked = CheckBoxTrayIcon.Checked && AppConfig.General.TrayText;
                                SettingsChanged();
                            }),

                            b.CheckBox(9, 119, "鼠标悬停在通知图标上时显示倒计时内容(&N)", SettingsChanged),
                        ])
                    ]),

                    b.Page(
                    [
                        b.Container<PlainGroupBox>(5, 3, 323, 98, "倒计时内容",
                        [
                            b.Label(7, 19, "当考试开始后, 显示"),

                            b.ComboBox(126, 15, 190, SettingsChanged,
                            [
                                new("<程序欢迎信息>", 0),
                                new("考试还有多久结束", 1),
                                new("考试还有多久结束 和 已过去了多久", 2)
                            ]),

                            b.CheckBox(9, 46, "只显示", (sender, _) =>
                            {
                                CheckBoxCeiling.Enabled = ComboBoxShowXOnly.Enabled = CheckBoxShowXOnly.Checked;
                                ComboBoxShowXOnly.SelectedIndex = CheckBoxShowXOnly.Checked ? AppConfig.Display.X : 0;
                                ChangeCustomTextStyle(sender);

                                if (CheckBoxCeiling.Checked && !CheckBoxShowXOnly.Checked)
                                {
                                    CheckBoxCeiling.Checked = false;
                                    CheckBoxCeiling.Enabled = false;
                                }

                                SettingsChanged();
                            }),

                            b.ComboBox(72, 43, 38, false, (_, _) =>
                            {
                                var Index = ComboBoxShowXOnly.SelectedIndex;
                                CheckBoxCeiling.Visible = Index == 0;
                                CheckBoxCeiling.Checked = Index == 0 && AppConfig.Display.Ceiling;
                                SettingsChanged();
                            },
                            [
                                new("天", 0),
                                new("时", 1),
                                new("分", 2),
                                new("秒", 3)
                            ]),

                            b.CheckBox(9, 72, "自定义不同时刻的颜色和内容:", (sender, _) =>
                            {
                                ChangeCustomTextStyle(sender);
                                SettingsChanged();
                            }),

                            b.Button(219, 68, "规则管理器(&R)", false, (_, _) =>
                            {
                                RulesManager Manager = new()
                                {
                                    Data = EditedCustomRules,
                                    ColorPresets = SelectedColors,
                                    CustomTextPreset = EditedCustomTexts
                                };

                                if (Manager.ShowDialog() == DialogResult.OK)
                                {
                                    EditedCustomRules = Manager.Data;
                                    EditedCustomTexts = Manager.CustomTextPreset;
                                    SettingsChanged(null, null);
                                }
                            }).With(x =>
                            {
                                x.AutoSize = true;
                                x.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                                x.Enabled = false;
                            })
                        ])
                    ]),

                    b.Page(
                    [

                    ])
                ]),









                //b.New<Panel>(57, 0, 332, 259, null, c => c.AddControls(b =>
                //[
                //    b.New<NavigationPage>(null, p => p.AddControls(b =>
                //    [
                //        b.New<GroupBox>(5, 3, 323, 67, "考试信息", x => x.AddControls(b =>
                //        [
                //            
                //        ])),

                //        b.New<GroupBox>()
                //    ]))
                //])),

                //b.New<Panel>(3, 0, 332, 259, null, c =>
                //{
                //    c.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                //    c.Controls.Add(new NavigationBar(["基本", "显示", "外观", "工具"], [PageGeneral, PageDisplay, PageAppearance, PageTools], PageNavPages)
                //    {
                //        Indent = ScaleToDpi(5),
                //        ItemHeight = ScaleToDpi(25)
                //    });
                //}),

                b.Button(233, 262, "保存(&S)", false, (_, _) => Save()),
                b.Button(314, 262, "取消(&C)")
            ]);
        }

        protected override void OnLoad()
        {
            RefreshNeeded = false;
            InitializeExtra();
            RefreshSettings();
            UpdateSettingsArea(SettingsArea.LastColor);
            UpdateSettingsArea(SettingsArea.Funny, false);
        }

        protected override void AdjustUI()
        {
            AlignControlsR(ButtonSave, ButtonCancel, PageNavPages);
            SetLabelAutoWrap(LabelPptsvc, GBoxPptsvc);
            SetLabelAutoWrap(LabelSyncTime, GBoxSyncTime);
            SetLabelAutoWrap(LabelColor, GBoxColors);
            SetLabelAutoWrap(LabelRestart, GBoxRestart);
            CompactControlsX(ComboBoxShowXOnly, CheckBoxShowXOnly);
            CompactControlsX(CheckBoxCeiling, ComboBoxShowXOnly, 10);
            CompactControlsX(ComboBoxScreens, LabelScreens);
            CompactControlsX(LabelPosition, ComboBoxScreens);
            CompactControlsX(ComboBoxPosition, LabelPosition);
            CompactControlsX(ComboBoxCountdownEnd, LabelCountdownEnd);
            CompactControlsX(ButtonSyncTime, ComboBoxNtpServers, 3);
            CompactControlsX(ComboBoxAutoSwitchIntervel, CheckBoxAutoSwitch);
            CompactControlsY(ButtonSyncTime, LabelSyncTime, 3);
            CompactControlsY(ButtonRestart, LabelRestart, 3);

            WhenHighDpi(() =>
            {
                AlignControlsX(ComboBoxAutoSwitchIntervel, CheckBoxAutoSwitch);
                AlignControlsX(ComboBoxShowXOnly, CheckBoxShowXOnly, -1);
                AlignControlsX(ComboBoxScreens, LabelScreens);
                AlignControlsX(ComboBoxPosition, LabelPosition);
                AlignControlsX(ComboBoxCountdownEnd, LabelCountdownEnd);
                AlignControlsX(ButtonRulesMan, CheckBoxRulesMan);
                AlignControlsX(ComboBoxNtpServers, ButtonSyncTime);
            });
        }

        protected override void OnShown()
        {
            NavBar.Focus();
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsSyncingTime || (UserChanged && ShowUnsavedWarning("检测到当前设置未保存，是否立即进行保存？", Save, ref UserChanged));
        }

        protected override void OnClosed()
        {
            if (InvokeChangeRequired)
            {
                SaveSettings();
            }

            UpdateSettingsArea(SettingsArea.Funny, false);
        }

        private void SettingsChanged()
        {
            SettingsChanged(null, null);
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            WhenLoaded(() =>
            {
                UserChanged = true;
                ButtonSave.Enabled = true;
            });
        }

        private void ComboBoxScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxPosition.SelectedIndex = ComboBoxPosition.Enabled ? (int)AppConfig.Display.Position : 3;
            SettingsChanged(null, null);
        }

        private void CheckBoxDraggable_CheckedChanged(object sender, EventArgs e)
        {
            ChangePptsvcStyle(null, null);
            ComboBoxScreens.SelectedIndex = CheckBoxDraggable.Checked ? 0 : AppConfig.Display.ScreenIndex;
            ComboBoxPosition.SelectedIndex = CheckBoxDraggable.Checked ? 3 : (int)AppConfig.Display.Position;

            var flag = !CheckBoxDraggable.Checked;
            ComboBoxScreens.Enabled = flag;
            ComboBoxPosition.Enabled = flag;
        }

        private void ButtonFont_Click(object sender, EventArgs e)
        {
            FontDialogEx Dialog = new(SelectedFont);

            if (Dialog.ShowDialog(this) == DialogResult.OK)
            {
                SettingsChanged(sender, e);
                UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: Dialog.Font);
            }
        }

        private void ButtonDefaultFont_Click(object sender, EventArgs e)
        {
            UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: DefaultValues.CountdownDefaultFont);
            SettingsChanged(sender, e);
        }

        private void ColorLabels_Click(object sender, EventArgs e)
        {
            var LabelSender = (Label)sender;
            var Dialog = new ColorDialogEx();

            if (Dialog.ShowDialog(LabelSender.BackColor, this) == DialogResult.OK)
            {
                LabelSender.BackColor = Dialog.Color;
                UpdateSettingsArea(SettingsArea.SelectedColor);
                SettingsChanged(sender, e);
            }
        }

        private void ColorLabels_MouseDown(object sender, MouseEventArgs e)
        {
            IsColorLabelsDragging = e.Button == MouseButtons.Left;
        }

        private void ColorLabels_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsColorLabelsDragging)
            {
                Cursor = Cursors.Cross;
            }
        }

        private void ColorLabels_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsColorLabelsDragging)
            {
                IsColorLabelsDragging = false;
                Cursor = Cursors.Default;

                var LabelSender = (Label)sender;
                var ParentContainer = LabelSender.Parent;
                var TargetControl = ParentContainer.GetChildAtPoint(ParentContainer.PointToClient(Cursor.Position));

                if (TargetControl != null && TargetControl is Label TagetLabel && ColorLabels.Contains(TagetLabel) && LabelSender != TagetLabel)
                {
                    TagetLabel.BackColor = LabelSender.BackColor;
                    UpdateSettingsArea(SettingsArea.SelectedColor);
                    SettingsChanged(sender, e);
                }
            }
        }

        private void ButtonDefaultColor_Click(object sender, EventArgs e)
        {
            ContextMenuDefaultColor.Show(ButtonDefaultColor, new(0, ButtonDefaultColor.Height));
        }

        private void ButtonSyncTime_Click(object sender, EventArgs e)
        {
            if (UACHelper.EnsureUAC(MessageX))
            {
                var server = ((ComboData)ComboBoxNtpServers.SelectedItem).Display;
                UpdateSettingsArea(SettingsArea.SyncTime);
                new Action(() => StartSyncTime(server)).Start();
            }
        }

        private void ButtonRestart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UpdateSettingsArea(SettingsArea.Funny);
                IsFunnyClick = true;
            }
            else if (!IsFunnyClick && e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (MessageX.Info("是否重启到命令行模式？", Buttons: MessageButtons.YesNo) == DialogResult.Yes)
                {
                    ProcessHelper.Run("cmd", $"/k title PlainCEETimer && \"{App.CurrentExecutablePath}\" /? & echo PlainCEETimer 命令行选项 & echo. & echo 请在此处输入命令行 & echo 或者输入 PlainCEETimer /h 获取帮助 && cd /d {App.CurrentExecutableDir}", ShowWindow: true);
                    App.Exit(ExitReason.Normal);
                }
            }

        }

        private void ButtonRestart_Click(object sender, EventArgs e)
        {
            App.Exit(IsFunnyClick ? ExitReason.UserShutdown : ExitReason.UserRestart);
        }

        private void RadioButtonTheme_CheckedChanged(object sender, EventArgs e)
        {
            SelectedTheme = (int)((RadioButton)sender).Tag;
            SettingsChanged(null, null);
        }

        private void InitializeExtra()
        {
            PanelNav.Controls.Add(NavBar = );

            if (!ThemeManager.IsDarkModeSupported)
            {
                GBoxTheme.Enabled = false;
                GBoxTheme.Visible = false;
            }

            ContextMenuDefaultColor = ContextMenuBuilder.Build(b =>
            [
                b.Item("白底(&L)", (_, _) =>
                {
                    SetLabelColors(DefaultValues.CountdownDefaultColorsLight);
                    SettingsChanged(null, null);
                }),

                b.Item("黑底(&D)", (_, _) =>
                {
                    SetLabelColors(DefaultValues.CountdownDefaultColorsDark);
                    SettingsChanged(null, null);
                })
            ]);

            LabelPreviewColor1.Text = $"距离...{Constants.PH_START}...";
            LabelPreviewColor2.Text = $"距离...{Constants.PH_LEFT}...";
            LabelPreviewColor3.Text = $"距离...{Constants.PH_PAST}...";

            ComboBoxCountdownEnd.BindData(
            [
            ]);

            ComboBoxPosition.BindData(
            [
                new("左上角", 0),
                new("左侧居中", 1),
                new("左下角", 2),
                new("顶部居中", 3),
                new("屏幕中心", 4),
                new("底部居中", 5),
                new("右上角", 6),
                new("右侧居中", 7),
                new("右下角", 8)
            ]);

            ComboBoxNtpServers.BindData(
            [
                new("time.windows.com", 0),
                new("ntp.aliyun.com", 1),
                new("ntp.tencent.com", 2),
                new("time.cloudflare.com", 3)
            ]);

            var CurrentScreens = Screen.AllScreens;
            var Length = CurrentScreens.Length;
            var Monitors = new ComboData[Length];
            for (int i = 0; i < Length; i++)
            {
                var CurrentScreen = CurrentScreens[i];
                Monitors[i] = new(string.Format("{0} {1} ({2}x{3})", i + 1, CurrentScreen.DeviceName, CurrentScreen.Bounds.Width, CurrentScreen.Bounds.Height), i);
            }
            ComboBoxScreens.BindData(Monitors);

            ColorPreviewLabels = [LabelPreviewColor1, LabelPreviewColor2, LabelPreviewColor3, LabelPreviewColor4];
            foreach (var label in ColorLabels = [LabelColor11, LabelColor21, LabelColor31, LabelColor41, LabelColor12, LabelColor22, LabelColor32, LabelColor42])
            {
                label.Click += ColorLabels_Click;
                label.MouseDown += ColorLabels_MouseDown;
                label.MouseMove += ColorLabels_MouseMove;
                label.MouseUp += ColorLabels_MouseUp;
            }

            RadioButtonThemeSystem.Tag = 0;
            RadioButtonThemeLight.Tag = 1;
            RadioButtonThemeDark.Tag = 2;
            RadioButtonThemeSystem.CheckedChanged += RadioButtonTheme_CheckedChanged;
            RadioButtonThemeLight.CheckedChanged += RadioButtonTheme_CheckedChanged;
            RadioButtonThemeDark.CheckedChanged += RadioButtonTheme_CheckedChanged;
        }

        private void RefreshSettings()
        {
            CheckBoxStartup.Checked = IsSetStartUp = (bool)OperateStartUp(0);
            CheckBoxTopMost.Checked = AppConfig.General.TopMost;
            CheckBoxMemClean.Checked = AppConfig.General.MemClean;
            CheckBoxDraggable.Checked = AppConfig.Display.Draggable;
            CheckBoxShowXOnly.Checked = AppConfig.Display.ShowXOnly;
            CheckBoxRulesMan.Checked = AppConfig.Display.CustomText;
            CheckBoxCeiling.Checked = AppConfig.Display.Ceiling;
            ComboBoxCountdownEnd.SelectedIndex = AppConfig.Display.EndIndex;
            CheckBoxPptSvc.Checked = AppConfig.Display.SeewoPptsvc;
            CheckBoxUniTopMost.Checked = MainForm.UniTopMost;
            ComboBoxScreens.SelectedIndex = AppConfig.Display.ScreenIndex;
            ComboBoxPosition.SelectedIndex = (int)AppConfig.Display.Position;
            ComboBoxShowXOnly.SelectedIndex = AppConfig.Display.X;
            UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: AppConfig.Font);
            ChangePptsvcStyle(null, null);
            SelectedColors = AppConfig.GlobalColors;
            ComboBoxShowXOnly.SelectedIndex = AppConfig.Display.ShowXOnly ? AppConfig.Display.X : 0;
            ComboBoxNtpServers.SelectedIndex = AppConfig.NtpServer;
            EditedCustomTexts = AppConfig.GlobalCustomTexts;
            EditedCustomRules = AppConfig.CustomRules;
            EditedExamInfo = AppConfig.Exams;
            CheckBoxTrayText.Enabled = CheckBoxTrayIcon.Checked = AppConfig.General.TrayIcon;
            CheckBoxTrayText.Checked = AppConfig.General.TrayText;
            CheckBoxAutoSwitch.Checked = AppConfig.General.AutoSwitch;
            ComboBoxAutoSwitchIntervel.SelectedIndex = AppConfig.General.Interval;
            ApplyRadios();
        }

        private void ApplyRadios()
        {
            var option = AppConfig.Dark;

            RadioButtonThemeSystem.Checked = option == 0;
            RadioButtonThemeLight.Checked = option == 1;
            RadioButtonThemeDark.Checked = option == 2 && ThemeManager.IsDarkModeSupported;
        }

        private void ChangeCustomTextStyle(object sender)
        {
            var cb = (CheckBox)sender;

            if (cb == CheckBoxShowXOnly)
            {
                CheckBoxRulesMan.Enabled = !cb.Checked;
                ButtonRulesMan.Enabled = false;
            }
            else
            {
                ButtonRulesMan.Enabled = cb.Checked;
                CheckBoxShowXOnly.Enabled = !cb.Checked;
            }
        }

        private void ChangePptsvcStyle(object sender, EventArgs e)
        {
            var a = CheckBoxTopMost.Checked;
            var b = ComboBoxPosition.SelectedIndex == 0;
            var c = CheckBoxDraggable.Checked;

            if (!a)
            {
                UpdateSettingsArea(SettingsArea.SetPPTService, false);
            }
            else if ((a && c) || (a && b))
            {
                UpdateSettingsArea(SettingsArea.SetPPTService);
            }
            else
            {
                UpdateSettingsArea(SettingsArea.SetPPTService, false, 1);
            }

            SettingsChanged(sender, e);
        }

        private bool IsSettingsFormatValid()
        {
            int ColorCheckMsg = 0;
            var Length = 4;
            SelectedColors = new ColorSetObject[Length];

            for (int i = 0; i < Length; i++)
            {
                var Fore = ColorLabels[i].BackColor;
                var Back = ColorLabels[i + Length].BackColor;

                if (!Validator.IsNiceContrast(Fore, Back))
                {
                    ColorCheckMsg = i + 1;
                    break;
                }

                SelectedColors[i] = new(Fore, Back);
            }

            if (ColorCheckMsg != 0)
            {
                NavBar.SwitchTo(PageAppearance);
                MessageX.Error($"第{ColorCheckMsg}组颜色的对比度较低，将无法看清文字。\n\n请更换其它背景颜色或文字颜色！");
                return false;
            }

            return true;
        }

        private void StartSyncTime(string Server)
        {
            try
            {
                var ExitCode = (int)ProcessHelper.Run("cmd", $"/c net stop w32time & sc config w32time start= auto & net start w32time && w32tm /config /manualpeerlist:{Server} /syncfromflags:manual /reliable:YES /update && w32tm /resync && w32tm /resync", 2, true);
                SwitchToToolsSafe();
                MessageX.Info($"命令执行完成！\n\n返回值为 {ExitCode} (0x{ExitCode:X})\n(0 代表成功，其他值为失败)");
            }
            #region 来自网络
            /*
                 
                检测用户是否点击了 UAC 提示框的 "否" 参考:

                c# - Run process as administrator from a non-admin application - Stack Overflow
                https://stackoverflow.com/a/20872219/21094697
                 
            */
            catch (Win32Exception ex) when (ex.NativeErrorCode == Constants.ERROR_CANCELLED)
            {
                SwitchToToolsSafe();
                MessageX.Error("授权失败，请在 UAC 对话框弹出时点击 \"是\"。", ex);
            }
            #endregion
            catch (Exception ex)
            {
                SwitchToToolsSafe();
                MessageX.Error("命令执行时发生了错误。", ex);
            }
            finally
            {
                BeginInvoke(() => UpdateSettingsArea(SettingsArea.SyncTime, false));
            }
        }

        private void UpdateSettingsArea(SettingsArea Where, bool IsWorking = true, int SubCase = 0, Font NewFont = null)
        {
            switch (Where)
            {
                case SettingsArea.SyncTime:
                    IsSyncingTime = IsWorking;
                    ButtonSyncTime.Enabled = !IsWorking;
                    ComboBoxNtpServers.Enabled = !IsWorking;
                    ButtonRestart.Enabled = !IsWorking;
                    ButtonSave.Enabled = !IsWorking && UserChanged;
                    ButtonCancel.Enabled = !IsWorking;
                    ButtonSyncTime.Text = IsWorking ? "正在同步中，请稍候..." : "立即同步(&S)";
                    break;
                case SettingsArea.Funny:
                    GBoxRestart.Text = IsWorking ? "关闭倒计时" : "重启倒计时";
                    LabelRestart.Text = $"用于更改了屏幕缩放之后, 可以点击此按钮来重启程序以确保 UI 正常显示。{(IsWorking ? "(●'◡'●)" : "")}";
                    ButtonRestart.Text = IsWorking ? "点击关闭(&G)" : "点击重启(&R)";
                    break;
                case SettingsArea.SetPPTService:
                    CheckBoxPptSvc.Enabled = IsWorking;
                    CheckBoxPptSvc.Checked = IsWorking && AppConfig.Display.SeewoPptsvc;
                    CheckBoxPptSvc.Text = IsWorking ? "启用此功能(&X)" : $"此项暂不可用，因为倒计时没有{(SubCase == 0 ? "顶置" : "在左上角")}。";
                    break;
                case SettingsArea.ChangeFont:
                    SelectedFont = NewFont;
                    LabelFont.Text = $"当前字体: {NewFont.Name}, {NewFont.Size}pt, {NewFont.Style}";
                    break;
                case SettingsArea.LastColor:
                    SetLabelColors(SelectedColors);
                    break;
                case SettingsArea.SelectedColor:
                    for (int i = 0; i < 4; i++)
                    {
                        ColorPreviewLabels[i].ForeColor = ColorLabels[i].BackColor;
                        ColorPreviewLabels[i].BackColor = ColorLabels[i + 4].BackColor;
                    }
                    break;
            }
        }

        private void SetLabelColors(ColorSetObject[] Colors)
        {
            for (int i = 0; i < 4; i++)
            {
                ColorLabels[i].BackColor = Colors[i].Fore;
                ColorPreviewLabels[i].ForeColor = Colors[i].Fore;
                ColorLabels[i + 4].BackColor = Colors[i].Back;
                ColorPreviewLabels[i].BackColor = Colors[i].Back;
            }
        }

        private void SwitchToToolsSafe()
        {
            BeginInvoke(() => NavBar.SwitchTo(PageTools));
        }

        private bool Save()
        {
            if (IsSyncingTime)
            {
                MessageX.Warn("无法执行此操作，请等待同步网络时钟完成！");
                return false;
            }

            if (IsSettingsFormatValid())
            {
                InvokeChangeRequired = true;
                UserChanged = false;
                Close();
            }

            return true;
        }

        private void SaveSettings()
        {
            var flag = IsSetStartUp;

            if ((IsSetStartUp = CheckBoxStartup.Checked) != flag)
            {
                OperateStartUp(IsSetStartUp ? 1 : 2);
            }

            App.AppConfig = new()
            {
                General = new()
                {
                    AutoSwitch = CheckBoxAutoSwitch.Checked,
                    Interval = ComboBoxAutoSwitchIntervel.SelectedIndex,
                    TrayIcon = CheckBoxTrayIcon.Checked,
                    TrayText = CheckBoxTrayText.Checked,
                    MemClean = CheckBoxMemClean.Checked,
                    TopMost = CheckBoxTopMost.Checked,
                    UniTopMost = CheckBoxUniTopMost.Checked
                },

                Display = new()
                {
                    ShowXOnly = CheckBoxShowXOnly.Checked,
                    X = ComboBoxShowXOnly.SelectedIndex,
                    Ceiling = CheckBoxCeiling.Checked,
                    EndIndex = ComboBoxCountdownEnd.SelectedIndex,
                    CustomText = CheckBoxRulesMan.Checked,
                    ScreenIndex = ComboBoxScreens.SelectedIndex,
                    Position = (CountdownPosition)ComboBoxPosition.SelectedIndex,
                    Draggable = CheckBoxDraggable.Checked,
                    SeewoPptsvc = CheckBoxPptSvc.Checked
                },

                Exams = EditedExamInfo,
                ExamIndex = AppConfig.ExamIndex,
                GlobalCustomTexts = EditedCustomTexts,
                GlobalColors = SelectedColors,
                CustomRules = EditedCustomRules,
                CustomColors = AppConfig.CustomColors,
                Font = SelectedFont,
                NtpServer = ComboBoxNtpServers.SelectedIndex,
                Dark = SelectedTheme,
                Location = AppConfig.Location
            };

            RefreshNeeded = true;
        }

        private object OperateStartUp(int Operation)
        {
            var KeyName = App.AppNameEngOld;
            var AppPath = $"\"{App.CurrentExecutablePath}\"";
            using var Helper = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);

            switch (Operation)
            {
                case 0:
                    return Helper.GetState(KeyName, AppPath, "");
                case 1:
                    if (Win32User.NotElevated) Helper.Set(KeyName, AppPath);
                    return null;
                default:
                    if (Win32User.NotElevated) Helper.Delete(KeyName);
                    return null;
            }
        }
    }
}
