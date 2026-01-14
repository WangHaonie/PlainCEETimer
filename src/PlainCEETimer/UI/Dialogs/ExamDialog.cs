using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ExamDialog(Exam data) : AppDialog, IListViewChildDialog<Exam>
{
    public Exam Data => data;

    protected override AppFormParam Params => AppFormParam.BindButtons;

    private int Mode;
    private bool IsEnabled;
    private CountdownFormat Format;
    private PlainLabel LabelName;
    private PlainLabel LabelCounter;
    private PlainLabel LabelStart;
    private PlainLabel LabelEnd;
    private PlainTextBox TextBoxName;
    private DateTimePicker DTPStart;
    private DateTimePicker DTPEnd;
    private PlainGroupBox GBoxContent;
    private PlainLabel LabelCountdownEnd;
    private PlainLabel LabelCountdownFormat;
    private PlainButton ButtonRulesMan;
    private PlainComboBox ComboBoxCountdownFormat;
    private PlainComboBox ComboBoxCountdownEnd;
    private PlainCheckBox CheckBoxEnableSettings;
    private EventHandler OnUserChanged;
    private ExamSettings Settings;
    private CountdownRule[] Rules;
    private CountdownRule[] DefaultRules;
    private CountdownRule[] GlobalRules;

    private string CurrentExamName;
    private readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

    protected override void OnInitializing()
    {
        Text = "考试信息 - 高考倒计时";
        OnUserChanged = (_, _) => UserChanged();

        this.AddControls(b =>
        [
            LabelName = b.Label("考试名称"),
            LabelStart = b.Label("考试开始"),
            LabelEnd = b.Label("考试结束"),
            LabelCounter = b.Label("00/00"),

            TextBoxName = b.TextBox(218, false, (_, _) =>
            {
                CurrentExamName = TextBoxName.Text.RemoveIllegalChars();
                int count = CurrentExamName.Length;
                LabelCounter.Text = $"{count}/{ConfigValidator.MaxExamNameLength}";
                LabelCounter.ForeColor = ConfigValidator.IsValidExamLength(count) ? (IsDark ? Colors.DarkForeText : Color.Black) : Color.Red;
                UserChanged();
            }).With(c => c.MaxLength = 99),

            DTPStart = b.DateTimePicker(255, OnUserChanged),
            DTPEnd = b.DateTimePicker(255, OnUserChanged),

            GBoxContent = b.GroupBox(null,
            [
                LabelCountdownEnd = b.Label("当考试开始后, 显示"),
                ComboBoxCountdownEnd = b.ComboBox(187, OnUserChanged, Ph.ComboBoxEndItems),
                LabelCountdownFormat = b.Label("倒计时内容格式"),

                ComboBoxCountdownFormat = b.ComboBox(117, (_, _) =>
                {
                    ButtonRulesMan.Enabled = ComboBoxCountdownFormat.SelectedIndex == 8;
                    UserChanged();
                }, Ph.ComboBoxFormatItems),

                ButtonRulesMan = b.Button("管理规则(&R)", true, (_, _) =>
                {
                    var dialog = new RulesManager()
                    {
                        Data = Rules,
                        FixedData = GetDefRules(),
                    };

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        Rules = dialog.Data;
                        DefaultRules = dialog.FixedData;
                        UserChanged();
                    }
                }).Disable()
            ]),

            CheckBoxEnableSettings = b.CheckBox("为该考试使用特定的设置", (_, _) =>
            {
                EnableControls(CheckBoxEnableSettings.Checked);
                UserChanged();
            })
        ]);

        base.OnInitializing();
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeFirstControl(LabelName, 3, 6);
        ArrangeControlXT(TextBoxName, LabelName);
        CenterControlY(LabelName, TextBoxName, isHighDpi ? 0 : -1);
        ArrangeControlXRT(LabelCounter, TextBoxName, LabelName, 2);
        ArrangeControlYL(LabelStart, LabelName);
        ArrangeControlYL(LabelEnd, LabelStart);
        ArrangeControlYL(DTPStart, TextBoxName, 0, 3);
        ArrangeControlYL(DTPEnd, DTPStart, 0, 3);
        CenterControlY(LabelStart, DTPStart);
        CenterControlY(LabelEnd, DTPEnd);
        CheckBoxEnableSettings.BringToFront();
        ArrangeControlYL(CheckBoxEnableSettings, LabelEnd, 10);
        CompactControlY(CheckBoxEnableSettings, DTPEnd, 3);
        ArrangeControlYL(GBoxContent, LabelEnd, 3);
        CompactControlY(GBoxContent, DTPEnd, 5);
        GroupBoxArrageControl(ComboBoxCountdownEnd, 0, 3);
        GroupBoxArrageControl(LabelCountdownEnd, -3);
        CenterControlY(LabelCountdownEnd, ComboBoxCountdownEnd);
        CompactControlX(ComboBoxCountdownEnd, LabelCountdownEnd);
        ArrangeControlYL(LabelCountdownFormat, LabelCountdownEnd);
        ArrangeControlXT(ComboBoxCountdownFormat, LabelCountdownFormat);
        CompactControlY(ComboBoxCountdownFormat, ComboBoxCountdownEnd, 4);
        CenterControlY(LabelCountdownFormat, ComboBoxCountdownFormat);
        ArrangeControlXT(ButtonRulesMan, ComboBoxCountdownFormat, 5);
        CenterControlY(ButtonRulesMan, ComboBoxCountdownFormat);
        GroupBoxAutoAdjustHeight(GBoxContent, ButtonRulesMan, 5);
        GBoxContent.Width = ButtonRulesMan.Right + ScaleToDpi(5);
        ArrangeCommonButtonsR(ButtonA, ButtonB, GBoxContent, 0, 3);
        InitWindowSize(ButtonB, 5, 5);
    }

    protected override void OnLoad()
    {
        var a = App.AppConfig;
        GlobalRules = a.GlobalRules;

        if (data != null)
        {
            TextBoxName.Text = data.Name;
            DTPStart.Value = data.Start;
            DTPEnd.Value = data.End;
            Settings = data.Settings;

            if (Settings == null)
            {
                var d = a.Display;
                Mode = d.Mode;
                Format = d.Format;
            }
            else
            {
                IsEnabled = Settings.Enabled;
                Mode = Settings.Mode;
                Format = Settings.Format;
                Rules = Settings.Rules;
                DefaultRules = Settings.DefRules;
                CheckBoxEnableSettings.Checked = IsEnabled;
            }

            ComboBoxCountdownEnd.SelectedIndex = Mode;
            ComboBoxCountdownFormat.SelectedIndex = (int)Format;
        }
        else
        {
            var date = DateTime.Now.TruncateToSeconds();
            DTPStart.Value = date;
            DTPEnd.Value = date;
        }

        EnableControls(IsEnabled);
    }

    protected override bool OnClickButtonA()
    {
        if (string.IsNullOrWhiteSpace(CurrentExamName) || !ConfigValidator.IsValidExamLength(CurrentExamName.Length))
        {
            MessageX.Error("输入的考试名称有误！\n\n请检查输入的考试名称是否太长或太短！");
            return false;
        }

        var start = DTPStart.Value;
        var end = DTPEnd.Value;
        var span = end - start;
        var ts = (long)span.TotalSeconds;

        if (end <= start || ts < 1)
        {
            MessageX.Error("考试时长无效！请检查相应日期时间是否合理。");
            return false;
        }

        var tmp = "";

        if (span.TotalDays > 4)
        {
            tmp = span.TotalDays.ToString("0") + " 天";
        }
        else if (span.TotalMinutes < 40 && ts > 60)
        {
            tmp = span.TotalMinutes.ToString("0") + " 分钟";
        }
        else if (ts < 60)
        {
            tmp = ts.ToString("0") + " 秒";
        }

        if (!string.IsNullOrEmpty(tmp) && MessageX.Warn($"检测到设置的考试时间太长或太短！当前考试时长: {tmp}。\n\n如果你确认当前设置的是正确的考试时间，请点击 是，否则请点击 否。", MessageButtons.YesNo) != DialogResult.Yes)
        {
            return false;
        }

        data = new()
        {
            Name = CurrentExamName,
            Start = start,
            End = end,

            Settings = new()
            {
                Enabled = CheckBoxEnableSettings.Checked,
                Mode = ComboBoxCountdownEnd.SelectedIndex,
                Format = (CountdownFormat)ComboBoxCountdownFormat.SelectedIndex,
                Rules = Rules,
                DefRules = DefaultRules
            }
        };

        return base.OnClickButtonA();
    }

    private CountdownRule[] GetDefRules()
    {
        if (DefaultRules == null || DefaultRules.Length < 3)
        {
            return GlobalRules;
        }

        return DefaultRules;
    }

    private void EnableControls(bool enabled)
    {
        LabelCountdownEnd.Enabled = enabled;
        LabelCountdownFormat.Enabled = enabled;
        ComboBoxCountdownEnd.Enabled = enabled;
        ComboBoxCountdownFormat.Enabled = enabled;
        ButtonRulesMan.Enabled = enabled && ComboBoxCountdownFormat.SelectedIndex == 8;
    }
}
