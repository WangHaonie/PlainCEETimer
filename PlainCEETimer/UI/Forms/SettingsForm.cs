using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Dialogs;

namespace PlainCEETimer.UI.Forms
{
    public sealed class SettingsForm : AppForm
    {
        public bool RefreshNeeded { get; private set; }

        private bool AllowThemeChanging;
        private bool IsColorLabelsDragging;
        private bool IsSyncingTime;
        private bool UserChanged;
        private bool InvokeChangeRequired;
        private bool IsFunnyClick;
        private bool IsSetStartUp;
        private int SelectedTheme;
        private string[] EditedCustomTexts;
        private ColorSetObject[] SelectedColors;
        private Action<Label> ColorBlockBindings;
        private ComboBoxEx ComboBoxAutoSwitchInterval;
        private ComboBoxEx ComboBoxCountdownEnd;
        private ComboBoxEx ComboBoxNtpServers;
        private ComboBoxEx ComboBoxPosition;
        private ComboBoxEx ComboBoxScreens;
        private ComboBoxEx ComboBoxShowXOnly;
        private ContextMenu ContextMenuDefaultColor;
        private CustomRuleObject[] EditedCustomRules;
        private ExamInfoObject[] EditedExamInfo;
        private Font SelectedFont;
        private Label LabelColor;
        private Label BlockColor11;
        private Label BlockColor12;
        private Label BlockColor21;
        private Label BlockColor22;
        private Label BlockColor31;
        private Label BlockColor32;
        private Label BlockColor41;
        private Label BlockColor42;
        private Label LabelColorP1;
        private Label LabelColorP2;
        private Label LabelColorP3;
        private Label LabelColorWelcome;
        private Label LabelCountdownEnd;
        private Label LabelExamInfo;
        private Label LabelFont;
        private Label LabelPosition;
        private Label BlockPreviewColor1;
        private Label BlockPreviewColor2;
        private Label BlockPreviewColor3;
        private Label BlockPreviewColor4;
        private Label LabelPptsvc;
        private Label LabelRestart;
        private Label LabelScreens;
        private Label LabelSyncTime;
        private Label[] ColorLabels;
        private Label[] ColorPreviewLabels;
        private NavigationBar NavBar;
        private NavigationPage PageAppearance;
        private NavigationPage PageDisplay;
        private NavigationPage PageGeneral;
        private NavigationPage PageAdvanced;
        private Panel PageNavPages;
        private PlainButton ButtonCancel;
        private PlainButton ButtonDefaultColor;
        private PlainButton ButtonDefaultFont;
        private PlainButton ButtonExamInfo;
        private PlainButton ButtonFont;
        private PlainButton ButtonRestart;
        private PlainButton ButtonRulesMan;
        private PlainButton ButtonSave;
        private PlainButton ButtonSyncTime;
        private PlainCheckBox CheckBoxAutoSwitch;
        private PlainCheckBox CheckBoxCeiling;
        private PlainCheckBox CheckBoxDraggable;
        private PlainCheckBox CheckBoxMemClean;
        private PlainCheckBox CheckBoxPptSvc;
        private PlainCheckBox CheckBoxRulesMan;
        private PlainCheckBox CheckBoxShowXOnly;
        private PlainCheckBox CheckBoxStartup;
        private PlainCheckBox CheckBoxTopMost;
        private PlainCheckBox CheckBoxTrayIcon;
        private PlainCheckBox CheckBoxTrayText;
        private PlainCheckBox CheckBoxUniTopMost;
        private PlainGroupBox GBoxColors;
        private PlainGroupBox GBoxContent;
        private PlainGroupBox GBoxDraggable;
        private PlainGroupBox GBoxExamInfo;
        private PlainGroupBox GBoxFont;
        private PlainGroupBox GBoxOthers;
        private PlainGroupBox GBoxPptsvc;
        private PlainGroupBox GBoxRestart;
        private PlainGroupBox GBoxSyncTime;
        private PlainGroupBox GBoxTheme;
        private PlainRadioButton RadioButtonThemeDark;
        private PlainRadioButton RadioButtonThemeLight;
        private PlainRadioButton RadioButtonThemeSystem;
        private readonly ConfigObject AppConfig = App.AppConfig;

        public SettingsForm() : base(AppFormParam.CompositedStyle | AppFormParam.CenterScreen) { }

        protected override void OnInitializing()
        {
            Text = "设置 - 高考倒计时";
            AllowThemeChanging = ThemeManager.IsDarkModeSupported;

            ContextMenuDefaultColor = ContextMenuBuilder.Build(b =>
            [
                b.Item("白底(&L)", (_, _) =>
                {
                    SetLabelColors(DefaultValues.CountdownDefaultColorsLight);
                    SettingsChanged();
                }),

                b.Item("黑底(&D)", (_, _) =>
                {
                    SetLabelColors(DefaultValues.CountdownDefaultColorsDark);
                    SettingsChanged();
                })
            ]);

            ColorBlockBindings = c =>
            {
                c.MouseDown += (_, e) => IsColorLabelsDragging = e.Button == MouseButtons.Left;

                c.MouseMove += (_, _) =>
                {
                    if (IsColorLabelsDragging)
                    {
                        Cursor = Cursors.Cross;
                    }
                };

                c.MouseUp += (sender, _) =>
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
                            SettingsChanged();
                        }
                    }
                };
            };

            this.AddControls(b =>
            [
                PageNavPages = b.Panel(56, 1, 334, 260,
                [
                    PageGeneral = b.Page(
                    [
                        GBoxExamInfo = b.GroupBox("考试信息",
                        [
                            LabelExamInfo = b.Label("在此添加考试信息以启动倒计时。"),

                            ButtonExamInfo = b.Button("管理(&G)", (_, _) =>
                            {
                                var ExamMan = new ExamInfoManager()
                                {
                                    Data = EditedExamInfo
                                };

                                if (ExamMan.ShowDialog() == DialogResult.OK)
                                {
                                    EditedExamInfo = ExamMan.Data;
                                    SettingsChanged();
                                }
                            }),

                            CheckBoxAutoSwitch = b.CheckBox("启用自动切换", (_, _) =>
                            {
                                ComboBoxAutoSwitchInterval.Enabled = CheckBoxAutoSwitch.Checked;
                                SettingsChanged();
                            }),

                            ComboBoxAutoSwitchInterval = b.ComboBox(86, false, SettingsChanged, "10 秒", "15 秒", "30 秒", "45 秒", "1 分钟", "2 分钟", "3 分钟", "5 分钟", "10 分钟", "15 分钟", "30 分钟", "45 分钟", "1 小时")
                        ]),

                        GBoxTheme = b.GroupBox("应用主题设定",
                        [
                            RadioButtonThemeSystem = b.RadioButton("跟随系统", RadioButtonTheme_CheckedChanged).With(x => x.Tag = 0),
                            RadioButtonThemeLight = b.RadioButton("浅色", RadioButtonTheme_CheckedChanged).With(x => x.Tag = 1),
                            RadioButtonThemeDark = b.RadioButton("深色", RadioButtonTheme_CheckedChanged).With(x => x.Tag = 2),
                        ]),

                        GBoxOthers = b.GroupBox("其他",
                        [
                            CheckBoxStartup = b.CheckBox("系统运行时自动启动倒计时(&B)", SettingsChanged),
                            CheckBoxMemClean = b.CheckBox("自动清理倒计时占用的运行内存(&M)", SettingsChanged),

                            CheckBoxTopMost = b.CheckBox("顶置倒计时窗口(&T)", (_, _) =>
                            {
                                CheckBoxUniTopMost.Enabled = CheckBoxTopMost.Checked;

                                if (CheckBoxUniTopMost.Checked && !CheckBoxTopMost.Checked)
                                {
                                    CheckBoxUniTopMost.Checked = false;
                                    CheckBoxUniTopMost.Enabled = false;
                                }

                                ChangePptsvcStyle(null, null);
                            }, true, true),

                            CheckBoxUniTopMost = b.CheckBox("顶置其他窗口(&U)", SettingsChanged),

                            CheckBoxTrayIcon = b.CheckBox("在托盘区域显示通知图标(&I)", (_, _) =>
                            {
                                CheckBoxTrayText.Enabled = CheckBoxTrayIcon.Checked;
                                CheckBoxTrayText.Checked = CheckBoxTrayIcon.Checked && AppConfig.General.TrayText;
                                SettingsChanged();
                            }),

                            CheckBoxTrayText = b.CheckBox("鼠标悬停在通知图标上时显示倒计时内容(&N)", SettingsChanged),
                        ])
                    ]),

                    PageDisplay = b.Page(
                    [
                        GBoxContent = b.GroupBox("倒计时内容",
                        [
                            LabelCountdownEnd = b.Label("当考试开始后, 显示"),

                            ComboBoxCountdownEnd = b.ComboBox(190, SettingsChanged, "<程序欢迎信息>", "考试还有多久结束", "考试还有多久结束 和 已过去了多久"),

                            ComboBoxShowXOnly = b.ComboBox(38, false, (_, _) =>
                            {
                                var Index = ComboBoxShowXOnly.SelectedIndex;
                                CheckBoxCeiling.Visible = Index == 0;
                                CheckBoxCeiling.Checked = Index == 0 && AppConfig.Display.Ceiling;
                                SettingsChanged();
                            }, "天", "时", "分", "秒"),

                            CheckBoxShowXOnly = b.CheckBox("只显示", (sender, _) =>
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

                            CheckBoxCeiling = b.CheckBox("不足一天按整天计算(&N)", SettingsChanged),

                            ButtonRulesMan = b.Button("规则管理器(&R)", false, true, (_, _) =>
                            {
                                var Manager = new RulesManager()
                                {
                                    Data = EditedCustomRules,
                                    ColorPresets = SelectedColors,
                                    CustomTextPreset = EditedCustomTexts
                                };

                                if (Manager.ShowDialog() == DialogResult.OK)
                                {
                                    EditedCustomRules = Manager.Data;
                                    EditedCustomTexts = Manager.CustomTextPreset;
                                    SettingsChanged();
                                }
                            }),

                            CheckBoxRulesMan = b.CheckBox("自定义不同时刻的颜色和内容:", (sender, _) =>
                            {
                                ChangeCustomTextStyle(sender);
                                SettingsChanged();
                            })
                        ]),

                        GBoxDraggable = b.GroupBox("多显示器与拖动",
                        [
                            LabelScreens = b.Label("固定在屏幕"),

                            ComboBoxScreens = b.ComboBox(107, (_, _) =>
                            {
                                ComboBoxPosition.SelectedIndex = ComboBoxPosition.Enabled ? (int)AppConfig.Display.Position : 3;
                                SettingsChanged();
                            }, GetScreensData()),

                            LabelPosition = b.Label("位置"),

                            ComboBoxPosition = b.ComboBox(84, ChangePptsvcStyle, "左上角", "左侧居中", "左下角", "顶部居中", "屏幕中心", "底部居中", "右上角", "右侧居中", "右下角"),

                            CheckBoxDraggable = b.CheckBox("允许拖动倒计时窗口(&D)", (_, _) =>
                            {
                                var flag = !CheckBoxDraggable.Checked;
                                ComboBoxScreens.SelectedIndex = CheckBoxDraggable.Checked ? 0 : AppConfig.Display.ScreenIndex;
                                ComboBoxPosition.SelectedIndex = CheckBoxDraggable.Checked ? 3 : (int)AppConfig.Display.Position;
                                ComboBoxScreens.Enabled = flag;
                                ComboBoxPosition.Enabled = flag;
                                ChangePptsvcStyle(null, null);
                            })
                        ]),

                        GBoxPptsvc = b.GroupBox("兼容希沃PPT小工具",
                        [
                            LabelPptsvc = b.Label("(仅个别机型) 用于修复希沃PPT小工具的内置白板打开后底部工具栏消失的问题。"),
                            CheckBoxPptSvc = b.CheckBox(null, SettingsChanged)
                        ])
                    ]),

                    PageAppearance = b.Page(
                    [
                        GBoxFont = b.GroupBox("字体和大小",
                        [
                            LabelFont = b.Label(null),

                            ButtonFont = b.Button("选择字体(&F)", true, true, (_, _) =>
                            {
                                var Dialog = new FontDialogEx(SelectedFont);

                                if (Dialog.ShowDialog(this) == DialogResult.OK)
                                {
                                    SettingsChanged();
                                    UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: Dialog.Font);
                                }
                            }),

                            ButtonDefaultFont = b.Button("恢复默认(&H)", true, true, (_, _) =>
                            {
                                UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: DefaultValues.CountdownDefaultFont);
                                SettingsChanged();
                            })
                        ]),

                        GBoxColors = b.GroupBox("字体颜色",
                        [
                            LabelColor = b.Label("点击色块来选择文字、背景颜色。将一个色块拖放到其它色块上可快速应用相同的颜色。"),
                            LabelColorP1 = b.Label("[1]考试前"),
                            LabelColorP2 = b.Label("[2]考试中"),
                            LabelColorP3 = b.Label("[3]考试后"),
                            LabelColorWelcome = b.Label("[4]欢迎信息"),
                            ButtonDefaultColor = b.Button("恢复默认(&M)", true, true, (sender, _) => ShowBottonMenu(ContextMenuDefaultColor, sender)),

                            BlockColor11 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),
                            BlockColor21 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),
                            BlockColor31 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),
                            BlockColor41 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),
                            BlockColor12 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),
                            BlockColor22 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),
                            BlockColor32 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),
                            BlockColor42 = b.Block(ColorBlocks_Click).With(ColorBlockBindings),

                            BlockPreviewColor1 = b.Block($"距离...{Constants.PH_START}..."),
                            BlockPreviewColor2 = b.Block($"距离...{Constants.PH_LEFT}..."),
                            BlockPreviewColor3 = b.Block($"距离...{Constants.PH_PAST}..."),
                            BlockPreviewColor4 = b.Block("欢迎使用...")
                        ])
                    ]),

                    PageAdvanced = b.Page(
                    [
                        GBoxSyncTime = b.GroupBox("同步网络时钟",
                        [
                            LabelSyncTime = b.Label("将尝试自动启动 Windows Time 服务, 并设置默认 NTP 服务器然后与之同步。"),

                            ComboBoxNtpServers = b.ComboBox(130, SettingsChanged, "time.windows.com", "ntp.aliyun.com", "ntp.tencent.com", "time.cloudflare.com"),

                            ButtonSyncTime = b.Button("立即同步(&Y)", true, true, (_, _) =>
                            {
                                if (UACHelper.EnsureUAC(MessageX))
                                {
                                    var server = ((ComboData)ComboBoxNtpServers.SelectedItem).Display;
                                    UpdateSettingsArea(SettingsArea.SyncTime);
                                    new Action(() => StartSyncTime(server)).Start();
                                }
                            })
                        ]),

                        GBoxRestart = b.GroupBox(null,
                        [
                            LabelRestart = b.Label(null),

                            ButtonRestart = b.Button(null, true, true, (_, _) => App.Exit(IsFunnyClick ? ExitReason.UserShutdown : ExitReason.UserRestart))
                            .With(x => x.MouseDown += (_, e) =>
                            {
                                if (e.Button == MouseButtons.Right)
                                {
                                    UpdateSettingsArea(SettingsArea.Funny);
                                    IsFunnyClick = true;
                                }
                                else if (!IsFunnyClick && e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
                                {
                                    if (MessageX.Info("是否重启到命令行模式？", buttons: MessageButtons.YesNo) == DialogResult.Yes)
                                    {
                                        ProcessHelper.Run("cmd", $"/k title PlainCEETimer && \"{App.CurrentExecutablePath}\" /? & echo PlainCEETimer 命令行选项 & echo. & echo 请在此处输入命令行 & echo 或者输入 PlainCEETimer /h 获取帮助 && cd /d {App.CurrentExecutableDir}", ShowWindow: true);
                                        App.Exit(ExitReason.Normal);
                                    }
                                }
                            })
                        ])
                    ])
                ]),

                b.Panel(1, 1, 54, 260,
                [
                    NavBar = new NavigationBar(["基本", "显示", "外观", "高级"], [PageGeneral, PageDisplay, PageAppearance, PageAdvanced])
                    {
                        Indent = ScaleToDpi(5),
                        ItemHeight = ScaleToDpi(25)
                    }
                ]),

                ButtonSave = b.Button("保存(&S)", false, (_, _) => Save()),
                ButtonCancel = b.Button("取消(&C)", (_, _) => Close())
            ]);

            ColorPreviewLabels = [BlockPreviewColor1, BlockPreviewColor2, BlockPreviewColor3, BlockPreviewColor4];
            ColorLabels = [BlockColor11, BlockColor21, BlockColor31, BlockColor41, BlockColor12, BlockColor22, BlockColor32, BlockColor42];
        }

        protected override void StartLayout(bool isHighDpi)
        {
            GroupBoxArrageFirst(LabelExamInfo, 0, 2);
            ArrangeControlYLeft(ButtonExamInfo, LabelExamInfo, isHighDpi ? 3 : 2, 3);
            GroupBoxAlignControlRight(GBoxExamInfo, ComboBoxAutoSwitchInterval, ButtonExamInfo, -6, 1);
            CenterControlY(ComboBoxAutoSwitchInterval, ButtonExamInfo);
            ArrangeControlXTopRtl(CheckBoxAutoSwitch, ComboBoxAutoSwitchInterval);
            CenterControlY(CheckBoxAutoSwitch, ComboBoxAutoSwitchInterval, 1);
            GroupBoxAutoAdjustHeight(GBoxExamInfo, ButtonExamInfo, 7);

            if (AllowThemeChanging)
            {
                ArrangeControlYLeft(GBoxTheme, GBoxExamInfo, 0, 2);

                GroupBoxArrageFirst(RadioButtonThemeSystem, 4);
                ArrangeControlXTop(RadioButtonThemeLight, RadioButtonThemeSystem, 6);
                ArrangeControlXTop(RadioButtonThemeDark, RadioButtonThemeLight, 6);
                GroupBoxAutoAdjustHeight(GBoxTheme, RadioButtonThemeSystem, 5);
            }
            else
            {
                PageGeneral.Controls.Remove(GBoxTheme);
                GBoxTheme.Dispose();
                GBoxTheme = null;
            }

            ArrangeControlYLeft(GBoxOthers, AllowThemeChanging ? GBoxTheme : GBoxExamInfo, 0, 2);

            GroupBoxArrageFirst(CheckBoxStartup, 3, 2);
            ArrangeControlYLeft(CheckBoxMemClean, CheckBoxStartup, 0, 4);
            ArrangeControlYLeft(CheckBoxTopMost, CheckBoxMemClean, 0, 4);
            ArrangeControlXTop(CheckBoxUniTopMost, CheckBoxTopMost);
            ArrangeControlYLeft(CheckBoxTrayIcon, CheckBoxTopMost, 0, 4);
            ArrangeControlYLeft(CheckBoxTrayText, CheckBoxTrayIcon, 0, 4);
            AlignControlYRight(CheckBoxUniTopMost, CheckBoxTrayText);
            GroupBoxAutoAdjustHeight(GBoxOthers, CheckBoxTrayText, 4);


            GroupBoxArrageFirst(ComboBoxCountdownEnd);
            GroupBoxArrageFirst(LabelCountdownEnd);
            CenterControlY(LabelCountdownEnd, ComboBoxCountdownEnd);
            ArrangeControlX(ComboBoxCountdownEnd, LabelCountdownEnd);
            CompactControlY(ComboBoxShowXOnly, ComboBoxCountdownEnd, 3);
            AlignControlLeft(CheckBoxShowXOnly, LabelCountdownEnd, 3);
            CenterControlY(CheckBoxShowXOnly, ComboBoxShowXOnly, 1);
            ArrangeControlX(ComboBoxShowXOnly, CheckBoxShowXOnly, -2);
            ArrangeControlXRightTop(CheckBoxCeiling, ComboBoxShowXOnly, CheckBoxShowXOnly, 6);
            AlignControlLeft(CheckBoxCeiling, ComboBoxCountdownEnd);
            CompactControlY(ButtonRulesMan, ComboBoxShowXOnly, 3);
            CenterControlY(CheckBoxRulesMan, ButtonRulesMan, 1);
            AlignControlLeft(CheckBoxRulesMan, CheckBoxShowXOnly);
            ArrangeControlX(ButtonRulesMan, CheckBoxRulesMan, 3);
            AlignControlYRight(ButtonRulesMan, ComboBoxCountdownEnd, 1);
            GroupBoxAutoAdjustHeight(GBoxContent, ButtonRulesMan, 6);

            ArrangeControlYLeft(GBoxDraggable, GBoxContent, 0, 2);

            GroupBoxArrageFirst(ComboBoxScreens, 0, 2);
            GroupBoxArrageFirst(LabelScreens);
            CenterControlY(LabelScreens, ComboBoxScreens);
            ArrangeControlX(ComboBoxScreens, LabelScreens);
            ArrangeControlXRightTop(LabelPosition, ComboBoxScreens, LabelScreens);
            ArrangeControlXRightTop(ComboBoxPosition, LabelPosition, ComboBoxScreens);
            CompactControlY(CheckBoxDraggable, ComboBoxScreens, 3);
            AlignControlLeft(CheckBoxDraggable, CheckBoxShowXOnly);
            GroupBoxAutoAdjustHeight(GBoxDraggable, CheckBoxDraggable, 3);

            ArrangeControlYLeft(GBoxPptsvc, GBoxDraggable, 0, 2);

            GroupBoxArrageFirst(LabelPptsvc);
            SetLabelAutoWrap(LabelPptsvc);
            CompactControlY(CheckBoxPptSvc, LabelPptsvc);
            AlignControlLeft(CheckBoxPptSvc, CheckBoxDraggable);
            GBoxPptsvc.Height = GBoxDraggable.Height + ScaleToDpi(isHighDpi ? 8 : 0);


            GroupBoxArrageFirst(LabelFont);
            ArrangeControlYLeft(ButtonFont, LabelFont, isHighDpi ? 3 : 2, 3);
            ArrangeControlXTop(ButtonDefaultFont, ButtonFont, 3);
            GroupBoxAutoAdjustHeight(GBoxFont, ButtonFont, 5);

            ArrangeControlYLeft(GBoxColors, GBoxFont, 0, 2);

            GroupBoxArrageFirst(LabelColor);
            SetLabelAutoWrap(LabelColor);
            ArrangeControlYLeft(LabelColorP1, LabelColor, 0, 3);
            ArrangeControlYLeft(LabelColorP2, LabelColorP1, 0, 6);
            ArrangeControlYLeft(LabelColorP3, LabelColorP2, 0, 6);
            ArrangeControlYLeft(LabelColorWelcome, LabelColorP3, 0, 6);
            ArrangeControlYLeft(ButtonDefaultColor, LabelColorWelcome, isHighDpi ? 3 : 2, 4);
            ArrangeControlXTop(BlockColor41, LabelColorWelcome, 3, -1);
            ArrangeControlXLeftTop(BlockColor31, BlockColor41, LabelColorP3, 0, -1);
            ArrangeControlXLeftTop(BlockColor21, BlockColor31, LabelColorP2, 0, -1);
            ArrangeControlXLeftTop(BlockColor11, BlockColor21, LabelColorP1, 0, -1);
            ArrangeControlXTop(BlockColor42, BlockColor41, 6);
            ArrangeControlXLeftTop(BlockColor32, BlockColor42, BlockColor31);
            ArrangeControlXLeftTop(BlockColor22, BlockColor32, BlockColor21);
            ArrangeControlXLeftTop(BlockColor12, BlockColor22, BlockColor11);
            ArrangeControlXTop(BlockPreviewColor4, BlockColor42, 6);
            ArrangeControlXLeftTop(BlockPreviewColor3, BlockPreviewColor4, BlockColor32);
            ArrangeControlXLeftTop(BlockPreviewColor2, BlockPreviewColor3, BlockColor22);
            ArrangeControlXLeftTop(BlockPreviewColor1, BlockPreviewColor2, BlockColor12);
            GroupBoxAutoAdjustHeight(GBoxColors, ButtonDefaultColor, 5);


            GroupBoxArrageFirst(LabelSyncTime);
            SetLabelAutoWrap(LabelSyncTime);
            ArrangeControlYLeft(ComboBoxNtpServers, LabelSyncTime, isHighDpi ? 3 : 2, 3);
            ArrangeControlXTop(ButtonSyncTime, ComboBoxNtpServers, 3);
            GroupBoxAutoAdjustHeight(GBoxSyncTime, ComboBoxNtpServers, 6);

            ArrangeControlYLeft(GBoxRestart, GBoxSyncTime, 0, 2);
            GroupBoxArrageFirst(LabelRestart);
            UpdateSettingsArea(SettingsArea.Funny, false);
            SetLabelAutoWrap(LabelRestart);
            ArrangeControlYLeft(ButtonRestart, LabelRestart, isHighDpi ? 3 : 2, 3);
            GroupBoxAutoAdjustHeight(GBoxRestart, ButtonRestart, 5);


            ArrangeControlYRight(ButtonCancel, PageNavPages, -4, 3);
            ArrangeControlXTopRtl(ButtonSave, ButtonCancel, -3);
        }

        protected override void OnLoad()
        {
            RefreshNeeded = false;
            RefreshSettings();
            UpdateSettingsArea(SettingsArea.LastColor);
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

        private void SettingsChanged(object sender, EventArgs e)
        {
            WhenLoaded(() =>
            {
                UserChanged = true;
                ButtonSave.Enabled = true;
            });
        }

        private void ColorBlocks_Click(object sender, EventArgs e)
        {
            var LabelSender = (Label)sender;
            var Dialog = new ColorDialogEx();

            if (Dialog.ShowDialog(LabelSender.BackColor, this) == DialogResult.OK)
            {
                LabelSender.BackColor = Dialog.Color;
                UpdateSettingsArea(SettingsArea.SelectedColor);
                SettingsChanged();
            }
        }

        private void RadioButtonTheme_CheckedChanged(object sender, EventArgs e)
        {
            SelectedTheme = (int)((RadioButton)sender).Tag;
            SettingsChanged();
        }

        private string[] GetScreensData()
        {
            var CurrentScreens = Screen.AllScreens;
            var Length = CurrentScreens.Length;
            Screen CurrentScreen;
            var Monitors = new string[Length];

            for (int i = 0; i < Length; i++)
            {
                CurrentScreen = CurrentScreens[i];
                Monitors[i] = string.Format("{0} {1} ({2}x{3})", i + 1, CurrentScreen.DeviceName, CurrentScreen.Bounds.Width, CurrentScreen.Bounds.Height);
            }

            return Monitors;
        }

        private void SettingsChanged()
        {
            SettingsChanged(null, null);
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
            ComboBoxAutoSwitchInterval.SelectedIndex = AppConfig.General.Interval;
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

            SettingsChanged();
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
                SwitchToAdvancedSafe();
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
                SwitchToAdvancedSafe();
                MessageX.Error("授权失败，请在 UAC 对话框弹出时点击 \"是\"。", ex);
            }
            #endregion
            catch (Exception ex)
            {
                SwitchToAdvancedSafe();
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

        private void SwitchToAdvancedSafe()
        {
            BeginInvoke(() => NavBar.SwitchTo(PageAdvanced));
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
                    Interval = ComboBoxAutoSwitchInterval.SelectedIndex,
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
                    break;
                default:
                    if (Win32User.NotElevated) Helper.Delete(KeyName);
                    break;
            }

            return null;
        }
    }
}
