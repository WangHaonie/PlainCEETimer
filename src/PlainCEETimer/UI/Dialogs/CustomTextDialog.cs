using System;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs;

public sealed class CustomTextDialog : AppDialog
{
    public string[] CustomTexts { get; set; } = new string[3];

    protected override AppFormParam Params => AppFormParam.AllControl | AppFormParam.CompositedStyle;

    private string[] Presets;
    private PlainLabel LabelInfo;
    private PlainComboBox ComboBoxPlaceholders;
    private PlainLabel LabelP1;
    private PlainLabel LabelP2;
    private PlainLabel LabelP3;
    private PlainTextBox TextBoxP1;
    private PlainTextBox TextBoxP2;
    private PlainTextBox TextBoxP3;
    private PlainTextBox[] TextBoxes;
    private PlainButton ButtonReset;
    private EventHandler OnUserChanged;

    protected override void OnInitializing()
    {
        var lp1 = "考试未开始";
        var lp2 = "考试已开始";
        var lp3 = "考试已结束";
        Text = "全局自定义文本 - 高考倒计时";
        OnUserChanged = (_, _) => UserChanged();
        Presets = DefaultValues.GlobalDefaultCustomTexts;

        this.AddControls(b =>
        [
            LabelP1 = b.Label(lp1),
            LabelP2 = b.Label(lp2),
            LabelP3 = b.Label(lp3),
            TextBoxP1 = b.TextBox(303, true, OnUserChanged),
            TextBoxP2 = b.TextBox(303, true, OnUserChanged),
            TextBoxP3 = b.TextBox(303, true, OnUserChanged),
            LabelInfo = b.Label("用于匹配规则之外。可用的占位符: "),

            ComboBoxPlaceholders = b.ComboBox(160, null,
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

            ButtonReset = b.Button("重置(R)", ContextMenuBuilder.Build(m =>
            [
                m.Item("所有", ItemsReset_Click).Default(),
                m.Separator(),
                m.Item(lp1, ItemsReset_Click),
                m.Item(lp2, ItemsReset_Click),
                m.Item(lp3, ItemsReset_Click)
            ])),
        ]);

        TextBoxes = [TextBoxP1, TextBoxP2, TextBoxP3];

        foreach (var tb in TextBoxes)
        {
            tb.OnExpandableKeyDown = (target, e, l) =>
            {
                TryAppendPlaceHolders(target, e, true, l);
            };
        }

        base.OnInitializing();
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeFirstControl(LabelInfo, 3, 0);
        ArrangeFirstControl(ComboBoxPlaceholders);
        CenterControlY(LabelInfo, ComboBoxPlaceholders);
        CompactControlX(ComboBoxPlaceholders, LabelInfo);
        ArrangeControlYL(LabelP1, LabelInfo);
        ArrangeControlXT(TextBoxP1, LabelP1);
        CompactControlY(TextBoxP1, ComboBoxPlaceholders, 3);
        CenterControlY(LabelP1, TextBoxP1, isHighDpi ? 0 : -1);
        ArrangeControlYL(TextBoxP2, TextBoxP1, 0, 3);
        ArrangeControlYL(TextBoxP3, TextBoxP2, 0, 3);
        AlignControlXL(LabelP2, LabelP1);
        AlignControlXL(LabelP3, LabelP2);
        CenterControlY(LabelP2, TextBoxP2, isHighDpi ? 0 : -1);
        CenterControlY(LabelP3, TextBoxP3, isHighDpi ? 0 : -1);
        ArrangeCommonButtonsR(ButtonA, ButtonB, TextBoxP3, 1, 3);
        ArrangeControlXT(ButtonReset, ButtonA);
        AlignControlXL(ButtonReset, LabelP1, 3);
    }

    protected override void OnLoad()
    {
        for (int i = 0; i < 3; i++)
        {
            TextBoxes[i].Text = CustomTexts[i];
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var t = ActiveControl as PlainTextBox;
        TryAppendPlaceHolders(t, e, t != null);
        base.OnKeyDown(e);
    }

    protected override bool OnClickButtonA()
    {
        string[] tmp = new string[3];
        string text;

        for (int i = 0; i < 3; i++)
        {
            text = TextBoxes[i].Text;

            if (!Validator.VerifyCustomText(text.RemoveIllegalChars(), out var msg, i + 1) && !string.IsNullOrEmpty(msg))
            {
                MessageX.Error(msg);
                return false;
            }

            tmp[i] = text;
        }

        CustomTexts = tmp;
        return base.OnClickButtonA();
    }

    private void ItemsReset_Click(object sender, EventArgs e)
    {
        var itemIndex = ((MenuItem)sender).Index - 2;

        if (itemIndex < 0)
        {
            ResetAllTexts();
        }
        else
        {
            ResetText(itemIndex);
        }

        UserChanged();
    }

    private void TryAppendPlaceHolders(PlainTextBox tb, KeyEventArgs e, bool condition, int length = -1)
    {
        if (e.Modifiers == Keys.None && e.KeyCode == Keys.F2 && condition)
        {
            var text = Ph.FormatPhs[ComboBoxPlaceholders.SelectedIndex];
            tb.Input((length == -1 ? tb.Text.RemoveIllegalChars().Length : length) + text.Length, text);
        }
    }

    private void ResetAllTexts()
    {
        for (int i = 0; i < 3; i++)
        {
            ResetText(i);
        }
    }

    private void ResetText(int Index)
    {
        TextBoxes[Index].Text = Presets[Index];
    }
}
