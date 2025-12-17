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
    private bool IsFunnyClick;
    private int SelectedTheme;
    private AppConfig AppConfig;
    private GeneralObject General;
    private DisplayObject Display;
    private ColorBlock BlockBorderColor;
    private PlainComboBox ComboBoxAutoSwitchInterval;
    private PlainComboBox ComboBoxBorderColor;
    private PlainComboBox ComboBoxCountdownEnd;
    private PlainComboBox ComboBoxNtpServers;
    private PlainComboBox ComboBoxPosition;
    private PlainComboBox ComboBoxScreens;
    private PlainLabel LabelCountdownEnd;
    private PlainLabel LabelExamInfo;
    private PlainLabel LabelOpacity;
    private PlainLabel LabelPosition;
    private PlainLabel LabelPptsvc;
    private PlainLabel LabelRestart;
    private PlainLabel LabelScreens;
    private PlainLabel LabelSyncTime;
    private NavigationPage PageDisplay;
    private NavigationPage PageGeneral;
    private NavigationPage PageAdvanced;
    private Panel PageNavPages;
    private PlainButton ButtonCancel;
    private PlainButton ButtonExamInfo;
    private PlainButton ButtonRestart;
    private PlainButton ButtonRulesMan;
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
    private PlainGroupBox GBoxContent;
    private PlainGroupBox GBoxDraggable;
    private PlainGroupBox GBoxExamInfo;
    private PlainGroupBox GBoxMainForm;
    private PlainGroupBox GBoxOthers;
    private PlainGroupBox GBoxPptsvc;
    private PlainGroupBox GBoxRestart;
    private PlainGroupBox GBoxSyncTime;
    private PlainGroupBox GBoxTheme;
    private PlainRadioButton RadioButtonThemeDark;
    private PlainRadioButton RadioButtonThemeLight;
    private PlainRadioButton RadioButtonThemeSystem;
    private CountdownRule[] EditedGlobalRules;
    private CountdownRule[] EditedCustomRules;
    private Exam[] EditedExamInfo;
    private readonly bool IsTaskStartUp = Startup.IsTaskSchd;

    private PlainLabel LabelCountdownFormat;
    private PlainComboBox ComboBoxCountdownFormat;

    protected override void OnInitializing()
    {
        Text = "设置 - 高考倒计时";
        AllowThemeChanging = ThemeManager.IsDarkModeSupported;

        this.AddControls(b =>
        [
            PageNavPages = b.Panel(56, 1, 334, 260,
            [
                PageGeneral = b.NavPage(
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

                    GBoxTheme = b.GroupBox("应用主题设定",
                    [
                        RadioButtonThemeSystem = b.RadioButton("系统默认", RadioButtonTheme_CheckedChanged).Tag(0),
                        RadioButtonThemeLight = b.RadioButton("浅色", RadioButtonTheme_CheckedChanged).Tag(1),
                        RadioButtonThemeDark = b.RadioButton("深色", RadioButtonTheme_CheckedChanged).Tag(2),
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

                        CheckBoxMemClean = b.CheckBox("自动清理倒计时占用的运行内存(&M)", SettingsChanged),

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

                PageDisplay = b.NavPage(
                [
                    GBoxContent = b.GroupBox("倒计时",
                    [
                        LabelCountdownEnd = b.Label("当考试开始后, 显示"),

                        ComboBoxCountdownEnd = b.ComboBox(190, SettingsChanged,
                            "<程序欢迎信息>",
                            "考试还有多久结束",
                            "考试还有多久结束 和 已过去了多久"
                        ),

                        LabelCountdownFormat = b.Label("倒计时内容格式"),

                        ComboBoxCountdownFormat = b.ComboBox(119, (_, _) =>
                        {
                            ButtonRulesMan.Enabled = ComboBoxCountdownFormat.SelectedIndex == 8;
                            SettingsChanged();
                        },
                            "默认",
                            "仅总天数",
                            "仅总天数 (一位小数)",
                            "仅总天数 (向上取整)",
                            "仅总小时",
                            "仅总小时 (一位小数)",
                            "仅总分钟",
                            "仅总秒数",
                            "自定义"
                        ),

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

                PageAdvanced = b.NavPage(
                [
                    GBoxSyncTime = b.GroupBox("同步网络时钟",
                    [
                        LabelSyncTime = b.Label("将尝试自动启动 Windows Time 服务, 并设置默认 NTP 服务器然后与之同步。"),

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

                        ButtonRestart = b.Button(null, true, (_, _) => App.Exit(!IsFunnyClick)).With(x => x.MouseDown += (_, e) =>
                        {
                            if (e.Button == MouseButtons.Right)
                            {
                                UpdateSettingsArea(SettingsArea.Restart);
                                IsFunnyClick = true;
                            }
                            else if (!IsFunnyClick && e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
                            {
                                if (MessageX.Info("是否重启到命令行模式？", MessageButtons.YesNo) == DialogResult.Yes)
                                {
                                    ProcessHelper.Run("cmd", $"/k title PlainCEETimer && \"{App.ExecutablePath}\" /? & echo PlainCEETimer 命令行选项 & echo. & echo 请在此处输入命令行 & echo 或者输入 PlainCEETimer /h 获取帮助 && cd /d {App.ExecutableDir}", true);
                                    App.Exit();
                                }
                            }
                        })
                    ]),

                    GBoxMainForm = b.GroupBox("主窗口样式",
                    [
                        LabelOpacity = b.Label("窗口不透明度 "),
                        NudOpacity = b.NumericUpDown(50, Validator.MaxOpacity, SettingsChanged).With(x => x.Minimum = Validator.MinOpacity),

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
                ])
            ]),

            b.Panel(1, 1, 54, 260,
            [
                new NavigationBar(["基本", "显示", "高级"], [PageGeneral, PageDisplay, PageAdvanced])
                {
                    Indent = ScaleToDpi(5),
                    ItemHeight = ScaleToDpi(25)
                }.AsFocus(this)
            ]),

            ButtonSave = b.Button("保存(&S)", (_, _) => SaveChanges()).Disable(),
            ButtonCancel = b.Button("取消(&C)", (_, _) => Close())
        ]);

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
            RemoveControls(PageGeneral, GBoxTheme);
        }

        ArrangeControlYL(GBoxOthers, AllowThemeChanging ? GBoxTheme : GBoxExamInfo, 0, 2);

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
        SetLabelAutoWrap(LabelPptsvc);
        CompactControlY(CheckBoxPptSvc, LabelPptsvc);
        AlignControlXL(CheckBoxPptSvc, LabelPptsvc, 4);
        GBoxPptsvc.Height = GBoxDraggable.Height + ScaleToDpi(isHighDpi ? 8 : 1);


        GroupBoxArrageFirstControl(LabelOpacity);
        GroupBoxArrageFirstControl(NudOpacity, 0, 2);
        CenterControlY(LabelOpacity, NudOpacity);
        CompactControlX(NudOpacity, LabelOpacity);
        Control yLast = NudOpacity;

        if (SystemVersion.IsWindows11)
        {
            ArrangeControlYL(CheckBoxBorderColor, LabelOpacity, 4);
            ArrangeControlXT(ComboBoxBorderColor, CheckBoxBorderColor);
            CompactControlY(ComboBoxBorderColor, NudOpacity, 4);
            CenterControlY(CheckBoxBorderColor, ComboBoxBorderColor, 1);
            ArrangeControlXT(BlockBorderColor, ComboBoxBorderColor, 5);
            CenterControlY(BlockBorderColor, ComboBoxBorderColor);
            yLast = ComboBoxBorderColor;
        }
        else
        {
            RemoveControls(GBoxMainForm, CheckBoxBorderColor, ComboBoxBorderColor, BlockBorderColor);
        }

        GroupBoxAutoAdjustHeight(GBoxMainForm, yLast, 6);

        ArrangeControlYL(GBoxSyncTime, GBoxMainForm, 0, 2);
        GroupBoxArrageFirstControl(LabelSyncTime);
        SetLabelAutoWrap(LabelSyncTime);
        ArrangeControlYL(ComboBoxNtpServers, LabelSyncTime, 4, 3);
        ArrangeControlXT(ButtonSyncTime, ComboBoxNtpServers, 5);
        GroupBoxAutoAdjustHeight(GBoxSyncTime, ComboBoxNtpServers, 6);

        ArrangeControlYL(GBoxRestart, GBoxSyncTime, 0, 2);
        GroupBoxArrageFirstControl(LabelRestart);
        UpdateSettingsArea(SettingsArea.Restart, false);
        SetLabelAutoWrap(LabelRestart);
        ArrangeControlYL(ButtonRestart, LabelRestart, isHighDpi ? 3 : 2, 3);
        GroupBoxAutoAdjustHeight(GBoxRestart, ButtonRestart, 5);

        ArrangeCommonButtonsR(ButtonSave, ButtonCancel, PageNavPages, -4, 3);
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
        CheckBoxDraggable.Checked = Display.Draggable;
        CheckBoxPptSvc.Checked = Display.SeewoPptsvc;
        UpdateOptionsForPptsvc();

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

    private bool SaveChanges()
    {
        if (IsSyncingTime)
        {
            MessageX.Error("无法执行此操作，请等待同步网络时钟完成！");
            return false;
        }

        CanSaveChanges = true;
        UserChanged = false;
        Close();
        return true;
    }

    private void SaveSettings()
    {
        Startup.SetAll(CheckBoxStartup.Checked, (bool)CheckBoxStartup.Tag);

        AppConfig.Exams = EditedExamInfo;
        AppConfig.CustomRules = EditedCustomRules;
        AppConfig.GlobalRules = EditedGlobalRules;
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
        General.BorderColor = new(CheckBoxBorderColor.Checked, ComboBoxBorderColor.SelectedIndex, BlockBorderColor.Color);

        Display.Mode = ComboBoxCountdownEnd.SelectedIndex;
        Display.Format = (CountdownFormat)ComboBoxCountdownFormat.SelectedIndex;
        Display.Screen = ComboBoxScreens.SelectedIndex;
        Display.Position = (CountdownPosition)ComboBoxPosition.SelectedIndex;
        Display.Draggable = CheckBoxDraggable.Checked;
        Display.SeewoPptsvc = CheckBoxPptSvc.Checked;

        Validator.DemandConfig();
        Validator.SaveConfig();
        EndModelessDialog(true, false);
    }
}
