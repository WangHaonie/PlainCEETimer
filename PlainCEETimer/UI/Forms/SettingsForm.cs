using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Dialogs;

namespace PlainCEETimer.UI.Forms
{
    public sealed class SettingsForm() : AppForm(AppFormParam.CompositedStyle | AppFormParam.CenterScreen | AppFormParam.OnEscClosing)
    {
        public bool RefreshNeeded { get; private set; }

        private bool AllowThemeChanging;
        private bool IsSyncingTime;
        private bool UserChanged;
        private bool InvokeChangeRequired;
        private bool IsFunnyClick;
        private bool IsSetStartUp;
        private int SelectedTheme;
        private string[] EditedCustomTexts;
        private ColorSetObject[] SelectedColors;
        private ColorBlock BlockColor11;
        private ColorBlock BlockColor12;
        private ColorBlock BlockColor21;
        private ColorBlock BlockColor22;
        private ColorBlock BlockColor31;
        private ColorBlock BlockColor32;
        private ColorBlock BlockColor41;
        private ColorBlock BlockColor42;
        private ColorBlock BlockPreviewColor1;
        private ColorBlock BlockPreviewColor2;
        private ColorBlock BlockPreviewColor3;
        private ColorBlock BlockPreviewColor4;
        private ColorBlock[] ColorBlocks;
        private ComboBoxEx ComboBoxAutoSwitchInterval;
        private ComboBoxEx ComboBoxCountdownEnd;
        private ComboBoxEx ComboBoxNtpServers;
        private ComboBoxEx ComboBoxPosition;
        private ComboBoxEx ComboBoxScreens;
        private ComboBoxEx ComboBoxShowXOnly;
        private CustomRuleObject[] EditedCustomRules;
        private ExamInfoObject[] EditedExamInfo;
        private Font SelectedFont;
        private Label LabelColor;
        private Label LabelColorP1;
        private Label LabelColorP2;
        private Label LabelColorP3;
        private Label LabelColorWelcome;
        private Label LabelCountdownEnd;
        private Label LabelExamInfo;
        private Label LabelFont;
        private Label LabelPosition;
        private Label LabelPptsvc;
        private Label LabelRestart;
        private Label LabelScreens;
        private Label LabelSyncTime;
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

        protected override void OnInitializing()
        {
            Text = "设置 - 高考倒计时";
            AllowThemeChanging = ThemeManager.IsDarkModeSupported;

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
                                var dialog = new ExamInfoManager()
                                {
                                    Data = EditedExamInfo
                                };

                                if (dialog.ShowDialog() == DialogResult.OK)
                                {
                                    EditedExamInfo = dialog.Data;
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
                            }),

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
                                var index = ComboBoxShowXOnly.SelectedIndex;
                                CheckBoxCeiling.Visible = index == 0;
                                CheckBoxCeiling.Checked = index == 0 && AppConfig.Display.Ceiling;
                                SettingsChanged();
                            }, "天", "时", "分", "秒"),

                            CheckBoxShowXOnly = b.CheckBox("只显示", (sender, _) =>
                            {
                                ComboBoxShowXOnly.Enabled = CheckBoxShowXOnly.Checked;
                                CheckBoxCeiling.Enabled = ComboBoxShowXOnly.Enabled;
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
                                var dialog = new RulesManager()
                                {
                                    Data = EditedCustomRules,
                                    ColorPresets = SelectedColors,
                                    CustomTextPreset = EditedCustomTexts
                                };

                                if (dialog.ShowDialog() == DialogResult.OK)
                                {
                                    EditedCustomRules = dialog.Data;
                                    EditedCustomTexts = dialog.CustomTextPreset;
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
                                var dialog = new FontDialogEx(SelectedFont);

                                if (dialog.ShowDialog(this) == DialogResult.OK)
                                {
                                    SettingsChanged();
                                    ChangeDisplayFont(dialog.Font);
                                }
                            }),

                            ButtonDefaultFont = b.Button("恢复默认(&H)", true, true, (_, _) =>
                            {
                                ChangeDisplayFont(DefaultValues.CountdownDefaultFont);
                                SettingsChanged();
                            })
                        ]),

                        GBoxColors = b.GroupBox("字体颜色",
                        [
                            LabelColor = b.Label("点击色块来选择文字、背景颜色；将一个色块拖放到其它色块上可快速应用相同的颜色；将准心拖出本窗口可以选取屏幕上的颜色。"),
                            LabelColorP1 = b.Label("[1]考试前"),
                            LabelColorP2 = b.Label("[2]考试中"),
                            LabelColorP3 = b.Label("[3]考试后"),
                            LabelColorWelcome = b.Label("[4]欢迎信息"),

                            ButtonDefaultColor = b.Button("恢复默认(&M)", true, true, ContextMenuBuilder.Build(b =>
                            [
                                b.Menu("白底(&L)",
                                [
                                    b.Item("所有", ItemsLight_Click),
                                    b.Separator(),
                                    b.Item("1", ItemsLight_Click),
                                    b.Item("2", ItemsLight_Click),
                                    b.Item("3", ItemsLight_Click),
                                    b.Item("4", ItemsLight_Click)
                                ]),

                                b.Menu("黑底(&D)",
                                [
                                    b.Item("所有", ItemsDark_Click),
                                    b.Separator(),
                                    b.Item("1", ItemsDark_Click),
                                    b.Item("2", ItemsDark_Click),
                                    b.Item("3", ItemsDark_Click),
                                    b.Item("4", ItemsDark_Click)
                                ]),
                            ])),

                            BlockPreviewColor1 = b.Block($"距离...{Constants.PH_START}..."),
                            BlockPreviewColor2 = b.Block($"距离...{Constants.PH_LEFT}..."),
                            BlockPreviewColor3 = b.Block($"距离...{Constants.PH_PAST}..."),
                            BlockPreviewColor4 = b.Block("欢迎使用..."),

                            BlockColor11 = b.Block(true, BlockPreviewColor1, SettingsChanged),
                            BlockColor21 = b.Block(true, BlockPreviewColor2, SettingsChanged),
                            BlockColor31 = b.Block(true, BlockPreviewColor3, SettingsChanged),
                            BlockColor41 = b.Block(true, BlockPreviewColor4, SettingsChanged),
                            BlockColor12 = b.Block(false, BlockPreviewColor1, SettingsChanged),
                            BlockColor22 = b.Block(false, BlockPreviewColor2, SettingsChanged),
                            BlockColor32 = b.Block(false, BlockPreviewColor3, SettingsChanged),
                            BlockColor42 = b.Block(false, BlockPreviewColor4, SettingsChanged)
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
                                var server = ((ComboData)ComboBoxNtpServers.SelectedItem).Display;
                                UpdateSettingsArea(SettingsArea.SyncTime);
                                ConsoleWindow.Run("cmd", $"/c net stop w32time & sc config w32time start= auto & net start w32time && w32tm /config /manualpeerlist:{server} /syncfromflags:manual /reliable:YES /update && w32tm /resync && w32tm /resync", x => UpdateSettingsArea(SettingsArea.SyncTime, false));
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
                                    if (MessageX.Info("是否重启到命令行模式？", MessageButtons.YesNo) == DialogResult.Yes)
                                    {
                                        ProcessHelper.Run("cmd", $"/k title PlainCEETimer && \"{App.CurrentExecutablePath}\" /? & echo PlainCEETimer 命令行选项 & echo. & echo 请在此处输入命令行 & echo 或者输入 PlainCEETimer /h 获取帮助 && cd /d {App.CurrentExecutableDir}", true);
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

                ButtonSave = b.Button("保存(&S)", false, (_, _) => SaveChanges()),
                ButtonCancel = b.Button("取消(&C)", (_, _) => Close())
            ]);

            ColorBlocks = [BlockColor11, BlockColor21, BlockColor31, BlockColor41, BlockColor12, BlockColor22, BlockColor32, BlockColor42];

            foreach (var block in ColorBlocks)
            {
                block.Parent = this;
                block.Fellows = ColorBlocks;
            }
        }

        protected override void StartLayout(bool isHighDpi)
        {
            GroupBoxArrageFirstControl(LabelExamInfo, 0, 2);
            ArrangeControlYL(ButtonExamInfo, LabelExamInfo, isHighDpi ? 3 : 2, 3);
            ArrangeControlXT(CheckBoxAutoSwitch, ButtonExamInfo, 30);
            CenterControlY(CheckBoxAutoSwitch, ButtonExamInfo, 1);
            ArrangeControlXT(ComboBoxAutoSwitchInterval, CheckBoxAutoSwitch);
            CenterControlY(ComboBoxAutoSwitchInterval, ButtonExamInfo);
            GroupBoxAutoAdjustHeight(GBoxExamInfo, ButtonExamInfo, 7);

            if (AllowThemeChanging)
            {
                ArrangeControlYL(GBoxTheme, GBoxExamInfo, 0, 2);

                GroupBoxArrageFirstControl(RadioButtonThemeSystem, 4);
                ArrangeControlXT(RadioButtonThemeLight, RadioButtonThemeSystem, 6);
                ArrangeControlXT(RadioButtonThemeDark, RadioButtonThemeLight, 6);
                GroupBoxAutoAdjustHeight(GBoxTheme, RadioButtonThemeSystem, 5);
            }
            else
            {
                PageGeneral.Controls.Remove(GBoxTheme);
                GBoxTheme.Dispose();
                GBoxTheme = null;
            }

            ArrangeControlYL(GBoxOthers, AllowThemeChanging ? GBoxTheme : GBoxExamInfo, 0, 2);

            GroupBoxArrageFirstControl(CheckBoxStartup, 3, 2);
            ArrangeControlYL(CheckBoxMemClean, CheckBoxStartup, 0, 4);
            ArrangeControlYL(CheckBoxTopMost, CheckBoxMemClean, 0, 4);
            ArrangeControlXT(CheckBoxUniTopMost, CheckBoxTopMost, 30);
            ArrangeControlYL(CheckBoxTrayIcon, CheckBoxTopMost, 0, 4);
            ArrangeControlYL(CheckBoxTrayText, CheckBoxTrayIcon, 0, 4);
            GroupBoxAutoAdjustHeight(GBoxOthers, CheckBoxTrayText, 4);


            GroupBoxArrageFirstControl(ComboBoxCountdownEnd);
            GroupBoxArrageFirstControl(LabelCountdownEnd);
            CenterControlY(LabelCountdownEnd, ComboBoxCountdownEnd);
            CompactControlX(ComboBoxCountdownEnd, LabelCountdownEnd);
            CompactControlY(ComboBoxShowXOnly, ComboBoxCountdownEnd, 3);
            AlignControlXL(CheckBoxShowXOnly, LabelCountdownEnd, 3);
            CenterControlY(CheckBoxShowXOnly, ComboBoxShowXOnly, 1);
            CompactControlX(ComboBoxShowXOnly, CheckBoxShowXOnly, -2);
            ArrangeControlXRT(CheckBoxCeiling, ComboBoxShowXOnly, CheckBoxShowXOnly, 6);
            AlignControlXL(CheckBoxCeiling, ComboBoxCountdownEnd);
            CompactControlY(ButtonRulesMan, ComboBoxShowXOnly, 3);
            CenterControlY(CheckBoxRulesMan, ButtonRulesMan, 1);
            AlignControlXL(CheckBoxRulesMan, CheckBoxShowXOnly);
            CompactControlX(ButtonRulesMan, CheckBoxRulesMan, 3);
            GroupBoxAutoAdjustHeight(GBoxContent, ButtonRulesMan, 6);

            ArrangeControlYL(GBoxDraggable, GBoxContent, 0, 2);

            GroupBoxArrageFirstControl(ComboBoxScreens, 0, 2);
            GroupBoxArrageFirstControl(LabelScreens);
            CenterControlY(LabelScreens, ComboBoxScreens);
            CompactControlX(ComboBoxScreens, LabelScreens);
            ArrangeControlXRT(LabelPosition, ComboBoxScreens, LabelScreens);
            ArrangeControlXRT(ComboBoxPosition, LabelPosition, ComboBoxScreens);
            CompactControlY(CheckBoxDraggable, ComboBoxScreens, 3);
            AlignControlXL(CheckBoxDraggable, CheckBoxShowXOnly);
            GroupBoxAutoAdjustHeight(GBoxDraggable, CheckBoxDraggable, 3);

            ArrangeControlYL(GBoxPptsvc, GBoxDraggable, 0, 2);

            GroupBoxArrageFirstControl(LabelPptsvc);
            SetLabelAutoWrap(LabelPptsvc);
            CompactControlY(CheckBoxPptSvc, LabelPptsvc);
            AlignControlXL(CheckBoxPptSvc, CheckBoxDraggable);
            GBoxPptsvc.Height = GBoxDraggable.Height + ScaleToDpi(isHighDpi ? 8 : 0);


            GroupBoxArrageFirstControl(LabelFont);
            ArrangeControlYL(ButtonFont, LabelFont, isHighDpi ? 3 : 2, 3);
            ArrangeControlXT(ButtonDefaultFont, ButtonFont, 3);
            GroupBoxAutoAdjustHeight(GBoxFont, ButtonFont, 5);

            ArrangeControlYL(GBoxColors, GBoxFont, 0, 2);

            GroupBoxArrageFirstControl(LabelColor);
            SetLabelAutoWrap(LabelColor);
            ArrangeControlYL(LabelColorP1, LabelColor, 0, 3);
            ArrangeControlYL(LabelColorP2, LabelColorP1, 0, 6);
            ArrangeControlYL(LabelColorP3, LabelColorP2, 0, 6);
            ArrangeControlYL(LabelColorWelcome, LabelColorP3, 0, 6);
            ArrangeControlYL(ButtonDefaultColor, LabelColorWelcome, isHighDpi ? 3 : 2, 4);
            ArrangeControlXT(BlockColor41, LabelColorWelcome, 3, -1);
            ArrangeControlXLT(BlockColor31, BlockColor41, LabelColorP3, 0, -1);
            ArrangeControlXLT(BlockColor21, BlockColor31, LabelColorP2, 0, -1);
            ArrangeControlXLT(BlockColor11, BlockColor21, LabelColorP1, 0, -1);
            ArrangeControlXT(BlockColor42, BlockColor41, 6);
            ArrangeControlXLT(BlockColor32, BlockColor42, BlockColor31);
            ArrangeControlXLT(BlockColor22, BlockColor32, BlockColor21);
            ArrangeControlXLT(BlockColor12, BlockColor22, BlockColor11);
            ArrangeControlXT(BlockPreviewColor4, BlockColor42, 6);
            ArrangeControlXLT(BlockPreviewColor3, BlockPreviewColor4, BlockColor32);
            ArrangeControlXLT(BlockPreviewColor2, BlockPreviewColor3, BlockColor22);
            ArrangeControlXLT(BlockPreviewColor1, BlockPreviewColor2, BlockColor12);
            GroupBoxAutoAdjustHeight(GBoxColors, ButtonDefaultColor, 5);


            GroupBoxArrageFirstControl(LabelSyncTime);
            SetLabelAutoWrap(LabelSyncTime);
            ArrangeControlYL(ComboBoxNtpServers, LabelSyncTime, isHighDpi ? 3 : 2, 3);
            ArrangeControlXT(ButtonSyncTime, ComboBoxNtpServers, 3);
            GroupBoxAutoAdjustHeight(GBoxSyncTime, ComboBoxNtpServers, 6);

            ArrangeControlYL(GBoxRestart, GBoxSyncTime, 0, 2);
            GroupBoxArrageFirstControl(LabelRestart);
            UpdateSettingsArea(SettingsArea.Funny, false);
            SetLabelAutoWrap(LabelRestart);
            ArrangeControlYL(ButtonRestart, LabelRestart, isHighDpi ? 3 : 2, 3);
            GroupBoxAutoAdjustHeight(GBoxRestart, ButtonRestart, 5);


            ArrangeCommonButtonsR(ButtonSave, ButtonCancel, PageNavPages, -4, 3);
        }

        protected override void OnLoad()
        {
            RefreshNeeded = false;
            RefreshSettings();
        }

        protected override void OnShown()
        {
            NavBar.Focus();
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return IsSyncingTime || (UserChanged && ShowUnsavedWarning("检测到当前设置未保存，是否立即进行保存？", SaveChanges, ref UserChanged));
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

        private void ItemsDark_Click(object sender, EventArgs e)
        {
            ResetColor(true, sender);
        }

        private void ItemsLight_Click(object sender, EventArgs e)
        {
            ResetColor(false, sender);
        }

        private void RadioButtonTheme_CheckedChanged(object sender, EventArgs e)
        {
            SelectedTheme = (int)((RadioButton)sender).Tag;
            SettingsChanged();
        }

        private string[] GetScreensData()
        {
            var screens = Screen.AllScreens;
            var count = screens.Length;
            var tmp = new string[count];
            Screen current;

            for (int i = 0; i < count; i++)
            {
                current = screens[i];
                tmp[i] = string.Format("{0} {1} ({2}x{3})", i + 1, current.DeviceName, current.Bounds.Width, current.Bounds.Height);
            }

            return tmp;
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
            CheckBoxCeiling.Enabled = false;
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
            ChangeDisplayFont(AppConfig.Font);
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
            ApplyColorBlocks(SelectedColors);
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

        private void ResetColor(bool isDark, object sender)
        {
            var colors = isDark ? DefaultValues.CountdownDefaultColorsDark : DefaultValues.CountdownDefaultColorsLight;
            var item = (MenuItem)sender;
            var index = item.Index - 2;

            if (index < 0)
            {
                ApplyColorBlocks(colors);
            }
            else
            {
                ApplyColorBlocks(colors, index);
            }

            SettingsChanged();
        }

        private void UpdateSettingsArea(SettingsArea area, bool isWorking = true, int subCase = 0)
        {
            switch (area)
            {
                case SettingsArea.SyncTime:
                    IsSyncingTime = isWorking;
                    ButtonSyncTime.Enabled = !isWorking;
                    ComboBoxNtpServers.Enabled = !isWorking;
                    ButtonRestart.Enabled = !isWorking;
                    ButtonSave.Enabled = !isWorking && UserChanged;
                    ButtonCancel.Enabled = !isWorking;
                    ButtonSyncTime.Text = isWorking ? "正在同步中，请稍候..." : "立即同步(&S)";
                    break;
                case SettingsArea.Funny:
                    GBoxRestart.Text = isWorking ? "关闭倒计时" : "重启倒计时";
                    LabelRestart.Text = $"用于更改了屏幕缩放之后, 可以点击此按钮来重启程序以确保 UI 正常显示。{(isWorking ? "(●'◡'●)" : "")}";
                    ButtonRestart.Text = isWorking ? "点击关闭(&G)" : "点击重启(&R)";
                    break;
                case SettingsArea.SetPPTService:
                    CheckBoxPptSvc.Enabled = isWorking;
                    CheckBoxPptSvc.Checked = isWorking && AppConfig.Display.SeewoPptsvc;
                    CheckBoxPptSvc.Text = isWorking ? "启用此功能(&X)" : $"此项暂不可用，因为倒计时没有{(subCase == 0 ? "顶置" : "在左上角")}。";
                    break;
            }
        }

        private void ChangeDisplayFont(Font newFont)
        {
            SelectedFont = newFont;
            LabelFont.Text = $"当前字体: {newFont.Name}, {newFont.Size}pt, {newFont.Style}";
        }

        private void ApplyColorBlocks(ColorSetObject[] colors)
        {
            for (int i = 0; i < 4; i++)
            {
                ApplyColorBlocks(colors, i);
            }
        }

        private void ApplyColorBlocks(ColorSetObject[] colors, int index)
        {
            ColorBlocks[index].Color = colors[index].Fore;
            ColorBlocks[index + 4].Color = colors[index].Back;
        }

        private bool SaveChanges()
        {
            if (IsSyncingTime)
            {
                MessageX.Warn("无法执行此操作，请等待同步网络时钟完成！");
                return false;
            }

            int index = -1;
            var Length = 4;
            SelectedColors = new ColorSetObject[Length];

            for (int i = 0; i < Length; i++)
            {
                var Fore = ColorBlocks[i].Color;
                var Back = ColorBlocks[i + Length].Color;

                if (!Validator.IsNiceContrast(Fore, Back))
                {
                    index = i;
                    break;
                }

                SelectedColors[i] = new(Fore, Back);
            }

            if (index != -1)
            {
                NavBar.SwitchTo(PageAppearance);
                MessageX.Error($"第{index}组颜色的对比度较低，将无法看清文字。\n\n请更换其它背景颜色或文字颜色！");
                return false;
            }

            InvokeChangeRequired = true;
            UserChanged = false;
            Close();

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

        private object OperateStartUp(int type)
        {
            var key = App.AppNameEngOld;
            var path = $"\"{App.CurrentExecutablePath}\"";
            using var helper = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);

            switch (type)
            {
                case 0:
                    return helper.GetState(key, path, "");
                case 1:
                    if (Win32User.NotElevated) helper.Set(key, path);
                    break;
                default:
                    if (Win32User.NotElevated) helper.Delete(key);
                    break;
            }

            return null;
        }
    }
}
