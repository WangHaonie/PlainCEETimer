using System;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Dialogs;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Forms;

public sealed class SettingsForm : AppForm
{
    protected override AppFormParam Params => AppFormParam.CenterScreen | AppFormParam.OnEscClosing | AppFormParam.ModelessDialog | AppFormParam.CompositedStyle;

    private bool AllowThemeChanging;
    private bool IsSyncingTime;
    private bool UserChanged;
    private bool CanSaveChanges;
    private bool AllowExit;
    private int SelectedTheme;
    private AppConfig AppConfig;
    private GeneralObject General;
    private DisplayObject Display;
    private ColorBlock BlockBorderColor;
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
    private PlainComboBox ComboBoxAutoSwitchInterval;
    private PlainComboBox ComboBoxBorderColor;
    private PlainComboBox ComboBoxCountdownEnd;
    private PlainComboBox ComboBoxCountdownFormat;
    private PlainComboBox ComboBoxNtpServers;
    private PlainComboBox ComboBoxPosition;
    private PlainComboBox ComboBoxScreens;
    private PlainLabel LabelCountdownEnd;
    private PlainLabel LabelCountdownFormat;
    private PlainLabel LabelExamInfo;
    private PlainLabel LabelOpacity;
    private PlainLabel LabelPosition;
    private PlainLabel LabelPptsvc;
    private PlainLabel LabelRestart;
    private PlainLabel LabelScreens;
    private PlainLabel LabelColor;
    private PlainLabel LabelColorP1;
    private PlainLabel LabelColorP2;
    private PlainLabel LabelColorP3;
    private PlainLabel LabelColorWelcome;
    private PlainLabel LabelSyncTime;
    private PlainLabel LabelMaxCpp;
    private NavigationView NavBar;
    private NavigationPage PageAppearance;
    private PlainButton ButtonCancel;
    private PlainButton ButtonExamInfo;
    private PlainButton ButtonRestart;
    private PlainButton ButtonRulesMan;
    private PlainButton ButtonDefaultColor;
    private PlainButton ButtonSave;
    private PlainButton ButtonSyncTime;
    private PlainCheckBox CheckBoxAutoSwitch;
    private PlainCheckBox CheckBoxBorderColor;
    private PlainCheckBox CheckBoxDraggable;
    private PlainCheckBox CheckBoxMemClean;
    private PlainCheckBox CheckBoxPptSvc;
    private PlainCheckBox CheckBoxStartup;
    private PlainCheckBox CheckBoxTopMost;
    private PlainCheckBox CheckBoxTrayIcon;
    private PlainCheckBox CheckBoxTrayText;
    private PlainCheckBox CheckBoxUniTopMost;
    private PlainNumericUpDown NudOpacity;
    private PlainNumericUpDown NudMaxCpp;
    private PlainGroupBox GBoxContent;
    private PlainGroupBox GBoxDraggable;
    private PlainGroupBox GBoxExamInfo;
    private PlainGroupBox GBoxMainForm;
    private PlainGroupBox GBoxOthers;
    private PlainGroupBox GBoxPptsvc;
    private PlainGroupBox GBoxColors;
    private PlainGroupBox GBoxRestart;
    private PlainGroupBox GBoxSyncTime;
    private PlainGroupBox GBoxTheme;
    private PlainRadioButton RadioButtonThemeDark;
    private PlainRadioButton RadioButtonThemeLight;
    private PlainRadioButton RadioButtonThemeSystem;
    private Exam[] EditedExamInfo;
    private ColorBlock[] ColorBlocks;
    private CountdownRule[] EditedGlobalRules;
    private CountdownRule[] EditedCustomRules;
    private ColorPair[] SelectedColors;
    private readonly bool IsTaskStartUp = Startup.IsTaskSchd;

    protected override void OnInitializing()
    {
        Text = "设置 - 高考倒计时";
        AllowThemeChanging = ThemeManager.IsDarkModeSupported;

        this.AddControls(b =>
        [
            NavBar = b.NavBar(1, 1, 54, 334, 225, ScaleToDpi(25), ScaleToDpi(5),
            [
                b.NavPage("基本",
                [
                    GBoxExamInfo = b.GroupBox("考试信息",
                    [
                        LabelExamInfo = b.Label("在此添加考试信息以启动倒计时。"),

                        ButtonExamInfo = b.Button("管理(&G)", (_, _) =>
                        {
                            var dialog = new ExamManager()
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

                        ComboBoxAutoSwitchInterval = b.ComboBox(86, SettingsChanged,
                            "10 秒",
                            "15 秒",
                            "30 秒",
                            "45 秒",
                            "1 分钟",
                            "2 分钟",
                            "3 分钟",
                            "5 分钟",
                            "10 分钟",
                            "15 分钟",
                            "30 分钟",
                            "45 分钟",
                            "1 小时"
                        ).Disable()
                    ]),

                    GBoxOthers = b.GroupBox("其他",
                    [
                        CheckBoxStartup = b.CheckBox(null, (_, _) =>
                        {
                            var flag = CheckBoxStartup.Checked;

                            if (flag ^ (bool)CheckBoxStartup.Tag)
                            {
                                if (flag)
                                {
                                    if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                                    {
                                        CheckBoxStartup.Tag = true;
                                        UpdateSettingsArea(SettingsArea.StartUp);
                                    }
                                    else
                                    {
                                        UpdateSettingsArea(SettingsArea.StartUp, false);
                                    }
                                }
                                else
                                {
                                    CheckBoxStartup.Tag = false;
                                }
                            }

                            SettingsChanged();
                        }).Tag(IsTaskStartUp),

                        CheckBoxMemClean = b.CheckBox("自动清理程序占用的运行内存(&M)", SettingsChanged),

                        CheckBoxTopMost = b.CheckBox("顶置倒计时窗口(&T)", (_, _) =>
                        {
                            CheckBoxUniTopMost.Enabled = CheckBoxTopMost.Checked;

                            if (CheckBoxUniTopMost.Checked && !CheckBoxTopMost.Checked)
                            {
                                CheckBoxUniTopMost.Checked = false;
                                CheckBoxUniTopMost.Enabled = false;
                            }

                            UpdateOptionsForPptsvc();
                        }),

                        CheckBoxUniTopMost = b.CheckBox("顶置其他窗口(&U)", SettingsChanged),

                        CheckBoxTrayIcon = b.CheckBox("在托盘区域显示通知图标(&I)", (_, _) =>
                        {
                            var flag = CheckBoxTrayIcon.Checked;
                            CheckBoxTrayText.Enabled = flag;
                            CheckBoxTrayText.Checked = flag;
                            SettingsChanged();
                        }),

                        CheckBoxTrayText = b.CheckBox("鼠标悬停在通知图标上时显示倒计时内容(&N)", SettingsChanged),
                    ])
                ]),

                b.NavPage("显示",
                [
                    GBoxContent = b.GroupBox("倒计时",
                    [
                        LabelCountdownEnd = b.Label("当考试开始后, 显示"),
                        ComboBoxCountdownEnd = b.ComboBox(190, SettingsChanged, Ph.ComboBoxEndItems),
                        LabelCountdownFormat = b.Label("倒计时内容格式"),

                        ComboBoxCountdownFormat = b.ComboBox(119, (_, _) =>
                        {
                            ButtonRulesMan.Enabled = ComboBoxCountdownFormat.SelectedIndex == 8;
                            SettingsChanged();
                        }, Ph.ComboBoxFormatItems),

                        ButtonRulesMan = b.Button("管理规则(&R)", true, (_, _) =>
                        {
                            var dialog = new RulesManager()
                            {
                                Data = EditedCustomRules,
                                FixedData = EditedGlobalRules,
                            };

                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                EditedCustomRules = dialog.Data;
                                EditedGlobalRules = dialog.FixedData;
                                SettingsChanged();
                            }
                        }).Disable()
                    ]),

                    GBoxDraggable = b.GroupBox("多显示器与拖动",
                    [
                        LabelScreens = b.Label("固定在屏幕"),
                        ComboBoxScreens = b.ComboBox(107, SettingsChanged, DisplayHelper.GetSystemDisplays()),
                        LabelPosition = b.Label("位置"),

                        ComboBoxPosition = b.ComboBox(84, (_, _) => UpdateOptionsForPptsvc(),
                            "左上角",
                            "左侧居中",
                            "左下角",
                            "顶部居中",
                            "屏幕中心",
                            "底部居中",
                            "右上角",
                            "右侧居中",
                            "右下角"
                        ),

                        CheckBoxDraggable = b.CheckBox("允许拖动倒计时窗口(&D)", (_, _) =>
                        {
                            var flag = !CheckBoxDraggable.Checked;
                            ComboBoxScreens.Enabled = flag;
                            ComboBoxPosition.Enabled = flag;
                            UpdateOptionsForPptsvc();
                        })
                    ]),

                    GBoxPptsvc = b.GroupBox("兼容希沃PPT小工具",
                    [
                        LabelPptsvc = b.Label("(仅个别机型) 用于避免希沃PPT小工具内置白板打开后底部工具栏消失的情况。"),
                        CheckBoxPptSvc = b.CheckBox(null, SettingsChanged)
                    ])
                ]),

                PageAppearance = b.NavPage("外观",
                [
                    GBoxColors = b.GroupBox("字体颜色",
                    [
                        LabelColor = b.Label("点击色块来选择文字、背景颜色；将一个色块拖放到其它色块上可快速应用相同的颜色；将准心拖出本窗口可以选取屏幕上的颜色。"),
                        LabelColorP1 = b.Label("[1]考试前"),
                        LabelColorP2 = b.Label("[2]考试中"),
                        LabelColorP3 = b.Label("[3]考试后"),
                        LabelColorWelcome = b.Label("[4]欢迎信息"),

                        ButtonDefaultColor = b.Button("重置(&M)", ContextMenuBuilder.Build(b =>
                        [
                            b.Menu("白底(&L)",
                            [
                                b.Item("所有", ItemsLight_Click).Default(),
                                b.Separator(),
                                b.Item("1", ItemsLight_Click),
                                b.Item("2", ItemsLight_Click),
                                b.Item("3", ItemsLight_Click),
                                b.Item("4", ItemsLight_Click)
                            ]),

                            b.Menu("黑底(&D)",
                            [
                                b.Item("所有", ItemsDark_Click).Default(),
                                b.Separator(),
                                b.Item("1", ItemsDark_Click),
                                b.Item("2", ItemsDark_Click),
                                b.Item("3", ItemsDark_Click),
                                b.Item("4", ItemsDark_Click)
                            ]),
                        ])),

                        BlockPreviewColor1 = b.Block($"距离...{Ph.Start}..."),
                        BlockPreviewColor2 = b.Block($"距离...{Ph.End}..."),
                        BlockPreviewColor3 = b.Block($"距离...{Ph.Past}..."),
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

                b.NavPage("样式",
                [
                    GBoxTheme = b.GroupBox("应用主题样式",
                    [
                        RadioButtonThemeSystem = b.RadioButton("系统默认", RadioButtonTheme_CheckedChanged).Tag(0),
                        RadioButtonThemeLight = b.RadioButton("浅色", RadioButtonTheme_CheckedChanged).Tag(1),
                        RadioButtonThemeDark = b.RadioButton("深色", RadioButtonTheme_CheckedChanged).Tag(2),
                    ]),

                    GBoxMainForm = b.GroupBox("主窗口样式",
                    [
                        LabelOpacity = b.Label("窗口不透明度"),
                        NudOpacity = b.NumericUpDown(50, ConfigValidator.MinOpacity, ConfigValidator.MaxOpacity, SettingsChanged),
                        LabelMaxCpp = b.Label("考试切换菜单单页最大项数"),
                        NudMaxCpp = b.NumericUpDown(50, ConfigValidator.MinCpp, ConfigValidator.MaxCpp, SettingsChanged),

                        CheckBoxBorderColor = b.CheckBox("窗口边框颜色", (_, _) =>
                        {
                            var flag = CheckBoxBorderColor.Checked;
                            ComboBoxBorderColor.Enabled = flag;
                            BlockBorderColor.Enabled = flag;
                            SettingsChanged();
                        }),

                        ComboBoxBorderColor = b.ComboBox(115, (_, _) =>
                        {
                            BlockBorderColor.Visible = ComboBoxBorderColor.SelectedIndex == 0;
                            SettingsChanged();
                        },
                            "自定义",
                            "跟随文字颜色",
                            "跟随背景颜色",
                            "跟随系统主题色"
                        ).Disable(),

                        BlockBorderColor = b.Block(true, null, SettingsChanged).Disable()
                    ])
                ]),

                b.NavPage("高级",
                [
                    GBoxSyncTime = b.GroupBox("同步网络时钟",
                    [
                        LabelSyncTime = b.Label("将尝试设置自动启动 Windows Time 服务，以及更改默认 NTP 服务器并与之同步。"),

                        ComboBoxNtpServers = b.ComboBox(130, SettingsChanged,
                            "time.windows.com",
                            "ntp.aliyun.com",
                            "ntp.tencent.com",
                            "time.cloudflare.com"
                        ),

                        ButtonSyncTime = b.Button("立即同步(&Y)", true, (_, _) =>
                        {
                            var server = ((ComboData)ComboBoxNtpServers.SelectedItem).Display;
                            UpdateSettingsArea(SettingsArea.SyncTime);
                            ConsoleWindow.Run("cmd", $"/c net stop w32time & sc config w32time start= auto & net start w32time && w32tm /config /manualpeerlist:{server} /syncfromflags:manual /reliable:YES /update && w32tm /resync && w32tm /resync", x => UpdateSettingsArea(SettingsArea.SyncTime, false));
                        })
                    ]),

                    GBoxRestart = b.GroupBox(null,
                    [
                        LabelRestart = b.Label(null),

                        ButtonRestart = b.Button(null, true, (_, _) => App.Exit(!AllowExit)).With(x => x.MouseDown += (_, e) =>
                        {
                            if (e.Button == MouseButtons.Right)
                            {
                                UpdateSettingsArea(SettingsArea.Restart);
                                AllowExit = true;
                            }
                            else if (!AllowExit && e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
                            {
                                if (MessageX.Info("是否重启到命令行模式？", MessageButtons.YesNo) == DialogResult.Yes)
                                {
                                    ProcessHelper.Run("cmd", $"/k title PlainCEETimer && \"{App.ExecutablePath}\" /? & echo PlainCEETimer 命令行选项 & echo. & echo 请在此处输入命令行 & echo 或者输入 PlainCEETimer /h 获取帮助 && cd /d {App.ExecutableDir}", true);
                                    App.Exit();
                                }
                            }
                        })
                    ])
                ])
            ]),

            ButtonSave = b.Button("保存(&S)", (_, _) => SaveChanges()).Disable(),
            ButtonCancel = b.Button("取消(&C)", (_, _) => Close())
        ]);

        ColorBlocks = [BlockColor11, BlockColor21, BlockColor31, BlockColor41, BlockColor12, BlockColor22, BlockColor32, BlockColor42];

        foreach (var block in ColorBlocks)
        {
            block.Fellows = ColorBlocks;
        }

        UpdateSettingsArea(SettingsArea.StartUp, false);
    }

    protected override void RunLayout(bool isHighDpi)
    {
        GroupBoxArrageFirstControl(LabelExamInfo, 0, 2);
        ArrangeControlYL(ButtonExamInfo, LabelExamInfo, isHighDpi ? 3 : 2, 3);
        ArrangeControlXT(CheckBoxAutoSwitch, ButtonExamInfo, 30);
        CenterControlY(CheckBoxAutoSwitch, ButtonExamInfo, 1);
        ArrangeControlXT(ComboBoxAutoSwitchInterval, CheckBoxAutoSwitch);
        CenterControlY(ComboBoxAutoSwitchInterval, ButtonExamInfo);
        GroupBoxAutoAdjustHeight(GBoxExamInfo, ButtonExamInfo, 7);

        ArrangeControlYL(GBoxOthers, GBoxExamInfo, 0, 2);

        GroupBoxArrageFirstControl(CheckBoxStartup, 4, 2);
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
        ArrangeControlYL(LabelCountdownFormat, LabelCountdownEnd);
        ArrangeControlXT(ComboBoxCountdownFormat, LabelCountdownFormat);
        CompactControlY(ComboBoxCountdownFormat, ComboBoxCountdownEnd, 5);
        CenterControlY(LabelCountdownFormat, ComboBoxCountdownFormat);
        ArrangeControlXT(ButtonRulesMan, ComboBoxCountdownFormat, 5);
        CenterControlY(ButtonRulesMan, ComboBoxCountdownFormat);
        GroupBoxAutoAdjustHeight(GBoxContent, ButtonRulesMan, 6);

        ArrangeControlYL(GBoxDraggable, GBoxContent, 0, 2);

        GroupBoxArrageFirstControl(ComboBoxScreens, 0, 2);
        GroupBoxArrageFirstControl(LabelScreens);
        CenterControlY(LabelScreens, ComboBoxScreens);
        CompactControlX(ComboBoxScreens, LabelScreens);
        ArrangeControlXRT(LabelPosition, ComboBoxScreens, LabelScreens);
        ArrangeControlXRT(ComboBoxPosition, LabelPosition, ComboBoxScreens);
        CompactControlY(CheckBoxDraggable, ComboBoxScreens, 3);
        AlignControlXL(CheckBoxDraggable, LabelScreens, 4);
        GroupBoxAutoAdjustHeight(GBoxDraggable, CheckBoxDraggable, 3);

        ArrangeControlYL(GBoxPptsvc, GBoxDraggable, 0, 2);

        GroupBoxArrageFirstControl(LabelPptsvc);
        SetLabelAutoWrap(LabelPptsvc, true);
        CompactControlY(CheckBoxPptSvc, LabelPptsvc);
        AlignControlXL(CheckBoxPptSvc, LabelPptsvc, 4);
        GBoxPptsvc.Height = GBoxDraggable.Height + ScaleToDpi(isHighDpi ? 8 : 1);


        GroupBoxArrageFirstControl(LabelColor);
        SetLabelAutoWrap(LabelColor, true);
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


        if (AllowThemeChanging)
        {
            GroupBoxArrageFirstControl(RadioButtonThemeSystem, 4, 4);
            ArrangeControlXT(RadioButtonThemeLight, RadioButtonThemeSystem, 6);
            ArrangeControlXT(RadioButtonThemeDark, RadioButtonThemeLight, 6);
            GroupBoxAutoAdjustHeight(GBoxTheme, RadioButtonThemeSystem, 6);

            ArrangeControlYL(GBoxMainForm, GBoxTheme, 0, 2);
        }
        else
        {
            GBoxTheme.Delete();
        }

        GroupBoxArrageFirstControl(LabelOpacity);
        GroupBoxArrageFirstControl(NudOpacity, 0, 2);
        CenterControlY(LabelOpacity, NudOpacity);
        CompactControlX(NudOpacity, LabelOpacity, 5);
        ArrangeControlYL(NudMaxCpp, NudOpacity, 0, 3);
        ArrangeControlYL(LabelMaxCpp, LabelOpacity);
        CenterControlY(LabelMaxCpp, NudMaxCpp);
        CompactControlX(NudMaxCpp, LabelMaxCpp, 5);
        Control yLast = NudMaxCpp;

        if (SystemVersion.IsWindows11)
        {
            ArrangeControlYL(CheckBoxBorderColor, LabelMaxCpp, 4);
            ArrangeControlXT(ComboBoxBorderColor, CheckBoxBorderColor);
            CompactControlY(ComboBoxBorderColor, NudMaxCpp, 4);
            CenterControlY(CheckBoxBorderColor, ComboBoxBorderColor, 1);
            ArrangeControlXT(BlockBorderColor, ComboBoxBorderColor, 5);
            CenterControlY(BlockBorderColor, ComboBoxBorderColor);
            yLast = ComboBoxBorderColor;
        }
        else
        {
            CheckBoxBorderColor.Delete();
            ComboBoxBorderColor.Delete();
            BlockBorderColor.Delete();
        }

        GroupBoxAutoAdjustHeight(GBoxMainForm, yLast, 6);


        GroupBoxArrageFirstControl(LabelSyncTime);
        SetLabelAutoWrap(LabelSyncTime, true);
        ArrangeControlYL(ComboBoxNtpServers, LabelSyncTime, 4, 3);
        ArrangeControlXT(ButtonSyncTime, ComboBoxNtpServers, 5);
        GroupBoxAutoAdjustHeight(GBoxSyncTime, ComboBoxNtpServers, 6);

        ArrangeControlYL(GBoxRestart, GBoxSyncTime, 0, 2);
        GroupBoxArrageFirstControl(LabelRestart);
        UpdateSettingsArea(SettingsArea.Restart, false);
        SetLabelAutoWrap(LabelRestart, true);
        ArrangeControlYL(ButtonRestart, LabelRestart, isHighDpi ? 3 : 2, 3);
        GroupBoxAutoAdjustHeight(GBoxRestart, ButtonRestart, 5);

        ArrangeCommonButtonsR(ButtonSave, ButtonCancel, NavBar, -4, 3);
    }

    protected override void OnLoad()
    {
        RefreshSettings();
    }

    protected override bool OnClosing(CloseReason closeReason)
    {
        return IsSyncingTime || (UserChanged && ShowUnsavedWarning("检测到当前设置未保存，是否立即进行保存？", SaveChanges, ref UserChanged));
    }

    protected override void OnClosed()
    {
        if (CanSaveChanges)
        {
            SaveSettings();
        }
    }

    private void SettingsChanged(object sender, EventArgs e)
    {
        EnsureLoaded(() =>
        {
            UserChanged = true;
            ButtonSave.Enabled = true;
        });
    }

    private void RadioButtonTheme_CheckedChanged(object sender, EventArgs e)
    {
        SelectedTheme = (int)((RadioButton)sender).Tag;
        SettingsChanged();
    }

    private void ItemsDark_Click(object sender, EventArgs e)
    {
        ResetColor(true, sender);
    }

    private void ItemsLight_Click(object sender, EventArgs e)
    {
        ResetColor(false, sender);
    }

    private void SettingsChanged()
    {
        SettingsChanged(null, null);
    }

    private void RefreshSettings()
    {
        AppConfig = App.AppConfig;
        General = AppConfig.General;
        Display = AppConfig.Display;

        EditedExamInfo = AppConfig.Exams;
        EditedCustomRules = AppConfig.CustomRules;
        EditedGlobalRules = AppConfig.GlobalRules;
        SelectedColors = AppConfig.DefaultColors;
        ComboBoxNtpServers.SelectedIndex = AppConfig.NtpServer;
        var dark = AppConfig.Dark;
        RadioButtonThemeSystem.Checked = dark == 0;
        RadioButtonThemeLight.Checked = dark == 1;
        RadioButtonThemeDark.Checked = dark == 2 && ThemeManager.IsDarkModeSupported;
        SelectedTheme = dark;

        CheckBoxAutoSwitch.Checked = General.AutoSwitch;
        ComboBoxAutoSwitchInterval.SelectedIndex = General.Interval;
        var ti = General.TrayIcon;
        CheckBoxTrayText.Enabled = ti;
        CheckBoxTrayIcon.Checked = ti;
        CheckBoxTrayText.Checked = General.TrayText;
        CheckBoxMemClean.Checked = General.MemClean;
        CheckBoxTopMost.Checked = General.TopMost;
        CheckBoxUniTopMost.Checked = MainForm.UniTopMost;
        NudOpacity.Value = General.Opacity;
        NudMaxCpp.Value = General.CountPerPage;

        if (SystemVersion.IsWindows11)
        {
            var border = General.BorderColor;
            CheckBoxBorderColor.Checked = border.Enabled;
            ComboBoxBorderColor.SelectedIndex = border.Type;
            BlockBorderColor.Color = border.Color;
        }

        ComboBoxCountdownEnd.SelectedIndex = Display.Mode;
        ComboBoxCountdownFormat.SelectedIndex = (int)Display.Format;
        ComboBoxScreens.SelectedIndex = Display.Screen;
        ComboBoxPosition.SelectedIndex = (int)Display.Position;
        CheckBoxDraggable.Checked = Display.Drag;
        CheckBoxPptSvc.Checked = Display.SeewoPptsvc;
        UpdateOptionsForPptsvc();

        ApplyColorBlocks(SelectedColors);

        CheckBoxStartup.Checked = Startup.GetRegistryState() || IsTaskStartUp;
        UpdateSettingsArea(SettingsArea.StartUp, IsTaskStartUp);
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
                break;
            case SettingsArea.Restart:
                GBoxRestart.Text = isWorking ? "关闭倒计时" : "重启倒计时";
                LabelRestart.Text = $"用于更改了屏幕缩放之后, 可以点击此按钮来重启程序以确保 UI 正常显示。{(isWorking ? "(●'◡'●)" : "")}";
                ButtonRestart.Text = isWorking ? "点击关闭(&G)" : "点击重启(&R)";
                break;
            case SettingsArea.PPTService:
                CheckBoxPptSvc.Enabled = isWorking;
                CheckBoxPptSvc.Checked = isWorking && Display.SeewoPptsvc;
                CheckBoxPptSvc.Text = isWorking ? "启用此功能(&X)" : $"此项暂不可用，因为倒计时没有{(subCase == 0 ? "顶置" : "在左上角")}";
                break;
            case SettingsArea.StartUp:
                CheckBoxStartup.Text = $"开机时自动运行倒计时{(isWorking ? "*" : "")}(&B)";
                break;
        }
    }

    private void UpdateOptionsForPptsvc()
    {
        var topmost = CheckBoxTopMost.Checked;
        var topleft = ComboBoxPosition.SelectedIndex == 0;
        var drag = CheckBoxDraggable.Checked;

        bool working;
        int sub;

        if (!topmost)
        {
            working = false;
            sub = 0;
        }
        else if (topleft || drag)
        {
            working = true;
            sub = 0;
        }
        else
        {
            working = false;
            sub = 1;
        }

        UpdateSettingsArea(SettingsArea.PPTService, working, sub);
        SettingsChanged();
    }

    private void ApplyColorBlocks(ColorPair[] colors)
    {
        for (int i = 0; i < 4; i++)
        {
            ApplyColorBlocks(colors, i);
        }
    }

    private void ResetColor(bool isDark, object sender)
    {
        var colors = isDark ? DefaultValues.DarkColors : DefaultValues.LightColors;
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

    private void ApplyColorBlocks(ColorPair[] colors, int index)
    {
        ColorBlocks[index].Color = colors[index].Fore;
        ColorBlocks[index + 4].Color = colors[index].Back;
    }

    private bool CheckSyncTime()
    {
        if (IsSyncingTime)
        {
            MessageX.Error("无法执行此操作，请等待同步网络时钟完成！");
            return false;
        }

        return true;
    }

    private bool CollectColors()
    {
        int index = -1;
        var length = 4;
        SelectedColors = new ColorPair[length];

        for (int i = 0; i < length; i++)
        {
            var cp = new ColorPair(ColorBlocks[i].Color, ColorBlocks[i + length].Color);

            if (!cp.Readable)
            {
                index = i;
                break;
            }

            SelectedColors[i] = cp;
        }

        if (index != -1)
        {
            NavBar.SwitchTo(PageAppearance);
            MessageX.Error($"第{index + 1}组颜色的对比度较低，将无法看清文字。\n\n请更换其它背景颜色或文字颜色！");
            return false;
        }

        return true;
    }

    private bool SaveChanges()
    {
        if (CheckSyncTime() && CollectColors())
        {
            CanSaveChanges = true;
            UserChanged = false;
            Close();
            return true;
        }

        return false;
    }

    private void SaveSettings()
    {
        Startup.SetAll(CheckBoxStartup.Checked, (bool)CheckBoxStartup.Tag);

        AppConfig.Exams = EditedExamInfo;
        AppConfig.CustomRules = EditedCustomRules;
        AppConfig.GlobalRules = EditedGlobalRules;
        AppConfig.DefaultColors = SelectedColors;
        AppConfig.NtpServer = ComboBoxNtpServers.SelectedIndex;
        AppConfig.Dark = SelectedTheme;

        General.AutoSwitch = CheckBoxAutoSwitch.Checked;
        General.Interval = ComboBoxAutoSwitchInterval.SelectedIndex;
        General.TrayIcon = CheckBoxTrayIcon.Checked;
        General.TrayText = CheckBoxTrayText.Checked;
        General.MemClean = CheckBoxMemClean.Checked;
        General.TopMost = CheckBoxTopMost.Checked;
        General.UniTopMost = CheckBoxUniTopMost.Checked;
        General.Opacity = (int)NudOpacity.Value;
        General.CountPerPage = (int)NudMaxCpp.Value;
        General.BorderColor = new(CheckBoxBorderColor.Checked, ComboBoxBorderColor.SelectedIndex, BlockBorderColor.Color);

        Display.Mode = ComboBoxCountdownEnd.SelectedIndex;
        Display.Format = (CountdownFormat)ComboBoxCountdownFormat.SelectedIndex;
        Display.Screen = ComboBoxScreens.SelectedIndex;
        Display.Position = (CountdownPosition)ComboBoxPosition.SelectedIndex;
        Display.Drag = CheckBoxDraggable.Checked;
        Display.SeewoPptsvc = CheckBoxPptSvc.Checked;

        ConfigValidator.DemandConfig();
        ConfigValidator.SaveConfig();
        EndModelessDialog(true, false);
    }
}
