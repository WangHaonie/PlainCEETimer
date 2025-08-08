using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class CustomTextDialog() : AppDialog(AppFormParam.AllControl | AppFormParam.CompositedStyle)
    {
        public string[] CustomTexts { get; set; } = new string[3];

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
                TextBoxP1 = b.TextBox(303, OnUserChanged),
                TextBoxP2 = b.TextBox(303, OnUserChanged),
                TextBoxP3 = b.TextBox(303, OnUserChanged),
                LabelInfo = b.Label("用于匹配规则之外。可用的占位符: "),

                ComboBoxPlaceholders = b.ComboBox(165, null,
                    $"{Constants.PH_EXAMNAME} - 考试名称",
                    $"{Constants.PH_DAYS} - 天/总天数",
                    $"{Constants.PH_DECIMALDAYS} - 保留一位小数的总天数",
                    $"{Constants.PH_CEILINGDAYS} - 向上取整的天数",
                    $"{Constants.PH_HOURS} - 时",
                    $"{Constants.PH_TOTALHOURS} - 总小时",
                    $"{Constants.PH_DECIMALHOURS} - 保留一位小数的总小时数",
                    $"{Constants.PH_MINUTES} - 分",
                    $"{Constants.PH_TOTALMINUTES} - 总分钟数",
                    $"{Constants.PH_SECONDS} - 秒",
                    $"{Constants.PH_TOTALSECONDS} - 总秒数"
                ),

                ButtonReset = b.Button("重置(R)", ContextMenuBuilder.Build(m =>
                [
                    m.Item("所有", ItemsReset_Click),
                    m.Separator(),
                    m.Item(lp1, ItemsReset_Click),
                    m.Item(lp2, ItemsReset_Click),
                    m.Item(lp3, ItemsReset_Click)
                ])),
            ]);

            TextBoxes = [TextBoxP1, TextBoxP2, TextBoxP3];
            base.OnInitializing();
        }

        protected override void StartLayout(bool isHighDpi)
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
            if (e.KeyCode == Keys.F2 && ActiveControl is PlainTextBox t)
            {
                t.AppendText(GetPlaceholder(ComboBoxPlaceholders.SelectedIndex));
            }

            base.OnKeyDown(e);
        }

        protected override bool OnClickButtonA()
        {
            string[] tmp = new string[3];
            string text;

            for (int i = 0; i < 3; i++)
            {
                text = TextBoxes[i].Text;

                if (!Validator.VerifyCustomText(text.RemoveIllegalChars(), out string msg, i + 1) && !string.IsNullOrEmpty(msg))
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
                ResetTexts();
            }
            else
            {
                ResetTexts(itemIndex);
            }

            UserChanged();
        }

        private string GetPlaceholder(int i) => i switch
        {
            1 => Constants.PH_DAYS,
            2 => Constants.PH_DECIMALDAYS,
            3 => Constants.PH_CEILINGDAYS,
            4 => Constants.PH_HOURS,
            5 => Constants.PH_TOTALHOURS,
            6 => Constants.PH_DECIMALHOURS,
            7 => Constants.PH_MINUTES,
            8 => Constants.PH_TOTALMINUTES,
            9 => Constants.PH_SECONDS,
            10 => Constants.PH_TOTALSECONDS,
            _ => Constants.PH_EXAMNAME,
        };

        private void ResetTexts()
        {
            for (int i = 0; i < 3; i++)
            {
                ResetTexts(i);
            }
        }

        private void ResetTexts(int Index)
        {
            TextBoxes[Index].Text = Presets[Index];
        }
    }
}
