using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs;

public sealed class RuleDialog(CountdownRule existing, CountdownRule[] presets = null) : AppDialog, IListViewChildDialog<CountdownRule>
{
    public CountdownRule Data => data;

    protected override AppFormParam Params => AppFormParam.AllControl | AppFormParam.CompositedStyle;

    private bool IsEditMode;
    private bool IsGlobal;
    private ColorBlock BlockPreview;
    private ColorBlock BlockFore;
    private ColorBlock BlockBack;
    private PlainComboBox ComboBoxRuleType;
    private PlainComboBox ComboBoxPlaceholders;
    private PlainLabel LabelCharExam;
    private PlainLabel LabelCharDay;
    private PlainLabel LabelCharHour;
    private PlainLabel LabelCharMinute;
    private PlainLabel LabelCharSecond;
    private PlainLabel LabelFore;
    private PlainLabel LabelBack;
    private PlainLabel LabelCustomText;
    private PlainLinkLabel LinkResetColor;
    private PlainLinkLabel LinkResetText;
    private PlainNumericUpDown NUDDays;
    private PlainNumericUpDown NUDHours;
    private PlainNumericUpDown NUDMinutes;
    private PlainNumericUpDown NUDSeconds;
    private PlainTextBox TextBoxCustomText;
    private EventHandler OnUserChanged;
    private CountdownRule data = existing;
    private readonly CountdownRule[] Presets = presets ?? DefaultValues.GlobalDefaultRules;
    private readonly Dictionary<int, CountdownUpdatedEventArgs> TemporaryChanges = new(3);

    protected override void OnInitializing()
    {
        Text = "自定义规则 - 高考倒计时";
        OnUserChanged = (_, _) => UserChanged();

        this.AddControls(b =>
        [
            LabelCharExam = b.Label("距离考试"),
            LabelCharDay = b.Label("天"),
            LabelCharHour = b.Label("时"),
            LabelCharMinute = b.Label("分"),
            LabelCharSecond = b.Label("秒"),
            LabelFore = b.Label("文字颜色"),
            LabelBack = b.Label("背景颜色"),
            LabelCustomText = b.Label("自定义文本"),
            LinkResetColor = b.Link("重置", LinkReset_LinkClicked),
            LinkResetText = b.Link("重置", LinkReset_LinkClicked),
            BlockPreview = b.Block("颜色效果预览"),
            BlockFore = b.Block(true, BlockPreview, ColorBlocks_Click),
            BlockBack = b.Block(false, BlockPreview, ColorBlocks_Click),

            TextBoxCustomText = b.TextBox(295, true, (_, _) =>
            {
                if (!IsEditMode)
                {
                    EnsureLoaded(SaveTemp);
                }

                UserChanged();
            }).AsFocus(this),

            ComboBoxRuleType = b.ComboBox(82, (_, _) =>
            {
                if (!IsEditMode)
                {
                    EnsureLoaded(() =>
                    {
                        var index = ComboBoxRuleType.SelectedIndex;

                        if (TemporaryChanges.ContainsKey(index))
                        {
                            var tmp = TemporaryChanges[index];
                            ApplyColorBlock(new(tmp.ForeColor, tmp.BackColor));
                            TextBoxCustomText.Text = tmp.Content;
                        }
                        else
                        {
                            GetNewData();
                        }
                    });
                }

                UserChanged();
            }, Ph.RuleTypes),

            NUDDays = b.NumericUpDown(53, 0M, 65535M, OnUserChanged),
            NUDHours = b.NumericUpDown(40, 0M, 23M, OnUserChanged),
            NUDMinutes = b.NumericUpDown(40, 0M, 59M, OnUserChanged),
            NUDSeconds = b.NumericUpDown(40, 0M, 59M, OnUserChanged),

            ComboBoxPlaceholders = b.ComboBox(160, (_, _) =>
                TextBoxCustomText.InputFlyout(Ph.FormatPhs[ComboBoxPlaceholders.SelectedIndex]),
                $"{Ph.ExamName} - 考试名称",
                $"{Ph.Days} - 天/总天数",
                $"{Ph.DecimalDays} - 总天数 (一位小数)",
                $"{Ph.CeilingDays} - 总天数 (向上取整)",
                $"{Ph.Hours} - 时",
                $"{Ph.TotalHours} - 总小时",
                $"{Ph.DecimalHours} - 总小时 (一位小数)",
                $"{Ph.Minutes} - 分",
                $"{Ph.TotalMinutes} - 总分钟",
                $"{Ph.Seconds} - 秒",
                $"{Ph.TotalSeconds} - 总秒数"
            ),
        ]);

        TextBoxCustomText.ExpandableVisibleChanged += (_, v) => ComboBoxPlaceholders.Visible = v;

        ColorBlock[] blocks = [BlockFore, BlockBack];

        foreach (var block in blocks)
        {
            block.Fellows = blocks;
        }

        base.OnInitializing();
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeFirstControl(LabelCharExam);
        ArrangeControlXT(ComboBoxRuleType, LabelCharExam);
        CenterControlY(LabelCharExam, ComboBoxRuleType);
        ArrangeControlXT(NUDDays, ComboBoxRuleType, 6, 0);
        ArrangeControlXRT(LabelCharDay, NUDDays, LabelCharExam);
        ArrangeControlXRT(NUDHours, LabelCharDay, NUDDays);
        ArrangeControlXRT(LabelCharHour, NUDHours, LabelCharDay);
        ArrangeControlXRT(NUDMinutes, LabelCharHour, NUDHours);
        ArrangeControlXRT(LabelCharMinute, NUDMinutes, LabelCharHour);
        ArrangeControlXRT(NUDSeconds, LabelCharMinute, NUDMinutes);
        ArrangeControlXRT(LabelCharSecond, NUDSeconds, LabelCharMinute);
        ArrangeControlYL(LabelFore, LabelCharExam);
        ArrangeControlYL(BlockFore, ComboBoxRuleType, 0, 3);
        CenterControlY(LabelFore, BlockFore);
        ArrangeControlXRT(LabelBack, BlockFore, LabelFore);
        ArrangeControlXRT(BlockBack, LabelBack, BlockFore);
        ArrangeControlYL(LabelCustomText, LabelFore);
        ArrangeControlYL(TextBoxCustomText, BlockFore, 0, 3);
        CenterControlY(LabelCustomText, TextBoxCustomText, -1);
        CompactControlX(TextBoxCustomText, LabelCustomText);
        ArrangeControlXRT(LinkResetText, TextBoxCustomText, LabelCustomText);
        ArrangeControlXT(BlockPreview, BlockBack);
        AlignControlXR(BlockPreview, TextBoxCustomText);
        ArrangeControlXRT(LinkResetColor, BlockPreview, LabelBack);
        ArrangeCommonButtonsR(ButtonA, ButtonB, LinkResetText, -3, 6);
        ArrangeControlXLT(ComboBoxPlaceholders, TextBoxCustomText, BlockFore, 0, -6);
        ComboBoxPlaceholders.BringToFront();
        ComboBoxPlaceholders.Visible = false;
        InitWindowSize(ButtonB, 5, 5);
    }

    protected override void OnLoad()
    {
        IsEditMode = Data != null;

        if (IsEditMode)
        {
            if (IsGlobal = Data.Default)
            {
                Control[] rtctrls = [ComboBoxRuleType, NUDDays, NUDHours, NUDMinutes, NUDSeconds];

                foreach (var ctrl in rtctrls)
                {
                    ctrl.Enabled = false;
                }
            }

            ComboBoxRuleType.SelectedIndex = (int)Data.Phase;
            var tick = Data.Tick;
            NUDDays.Value = tick.Days;
            NUDHours.Value = tick.Hours;
            NUDMinutes.Value = tick.Minutes;
            NUDSeconds.Value = tick.Seconds;
            ApplyColorBlock(Data.Colors);
            TextBoxCustomText.Text = Data.Text;
        }
        else
        {
            GetNewData();
        }
    }

    protected override bool OnClickButtonA()
    {
        var d = (int)NUDDays.Value;
        var h = (int)NUDHours.Value;
        var m = (int)NUDMinutes.Value;
        var s = (int)NUDSeconds.Value;

        if (!IsGlobal && d == 0 && m == 0 && h == 0 && s == 0)
        {
            MessageX.Error("时刻不能为0，请重新设置！");
            return false;
        }

        var colors = new ColorPair(BlockFore.Color, BlockBack.Color);

        if (!colors.Readable)
        {
            MessageX.Error("选择的颜色相似或对比度较低，将无法看清文字。\n\n请尝试更换其它背景颜色或文字颜色！");
            return false;
        }

        var text = TextBoxCustomText.Text.RemoveIllegalChars();

        if (!ConfigValidator.VerifyCustomText(text))
        {
            MessageX.Error("自定义文本不能为空且至少包含一个占位符！");
            return false;
        }

        data = new()
        {
            Phase = IsGlobal ? Data.Phase : (CountdownPhase)ComboBoxRuleType.SelectedIndex,
            Tick = IsGlobal ? default : new(d, h, m, s),
            Text = text,
            Colors = colors,
            Default = IsGlobal
        };

        return base.OnClickButtonA();
    }

    private void ColorBlocks_Click(object sender, EventArgs e)
    {
        UserChanged();

        if (!IsEditMode)
        {
            SaveTemp();
        }
    }

    private void LinkReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        var flag = (LinkLabel)sender == LinkResetColor;
        GetNewData(false, flag, !flag);
        UserChanged();
    }

    private void SaveTemp()
    {
        TemporaryChanges[ComboBoxRuleType.SelectedIndex] = new(TextBoxCustomText.Text, BlockFore.Color, BlockBack.Color);
    }

    private void GetNewData(bool all = true, bool colorOnly = false, bool textOnly = false)
    {
        var rule = Presets[ComboBoxRuleType.SelectedIndex];

        if (all || colorOnly)
        {
            ApplyColorBlock(rule.Colors);
        }

        if (all || textOnly)
        {
            TextBoxCustomText.Text = rule.Text;
        }

        if (!IsEditMode)
        {
            SaveTemp();
        }
    }

    private void ApplyColorBlock(ColorPair colors)
    {
        BlockFore.Color = colors.Fore;
        BlockBack.Color = colors.Back;
    }
}
