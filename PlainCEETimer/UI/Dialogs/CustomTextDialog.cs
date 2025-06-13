using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class CustomTextDialog : AppDialog
    {
        public string[] CustomTexts { get; set; } = new string[3];

        private string[] Presets;
        private ContextMenu ContextMenuReset;
        private Label LabelInfo;
        private Label LabelP1;
        private Label LabelP2;
        private Label LabelP3;
        private PlainTextBox TextBoxP1;
        private PlainTextBox TextBoxP2;
        private PlainTextBox TextBoxP3;
        private PlainTextBox[] TextBoxes;
        private PlainButton ButtonReset;
        private EventHandler OnUserChanged;

        public CustomTextDialog() : base(AppFormParam.BindButtons | AppFormParam.CompositedStyle) { }

        protected override void OnInitializing()
        {
            var lp1 = "考试未开始";
            var lp2 = "考试已开始";
            var lp3 = "考试已结束";
            Text = "全局自定义文本 - 高考倒计时";
            OnUserChanged = (_, _) => UserChanged();
            Presets = DefaultValues.GlobalDefaultCustomTexts;
            ContextMenuReset = ContextMenuBuilder.Build(m =>
            [
                m.Item(lp1, ItemsReset_Click),
                m.Item(lp2, ItemsReset_Click),
                m.Item(lp3, ItemsReset_Click)
            ]);

            this.AddControls(b =>
            [
                LabelP1 = b.Label(lp1),
                LabelP2 = b.Label(lp2),
                LabelP3 = b.Label(lp3),
                TextBoxP1 = b.TextBox(310, OnUserChanged),
                TextBoxP2 = b.TextBox(310, OnUserChanged),
                TextBoxP3 = b.TextBox(310, OnUserChanged),
                LabelInfo = b.Label(3, 0, null),
                ButtonReset = b.Button("重置(R)", (sender, _) => ShowBottonMenu(ContextMenuReset, sender))
            ]);

            TextBoxes = [TextBoxP1, TextBoxP2, TextBoxP3];
            base.OnInitializing();
        }

        protected override void StartLayout(bool isHighDpi)
        {
            SetLabelAutoWrap(LabelInfo, TextBoxP1.Width + LabelP1.Width + ScaleToDpi(3));
            LabelInfo.Text = $"用于匹配规则之外。可用的占位符: {Constants.PH_EXAMNAME}-考试名称 {Constants.PH_DAYS}-天 {Constants.PH_HOURS}-时 {Constants.PH_MINUTES}-分 {Constants.PH_SECONDS}-秒 {Constants.PH_CEILINGDAYS}-向上取整的天数 {Constants.PH_TOTALHOURS}-总小时数 {Constants.PH_TOTALMINUTES}-总分钟数 {Constants.PH_TOTALSECONDS}-总秒数。比如 \"{Constants.PH_EXAMNAME}还有{Constants.PH_DAYS}.{Constants.PH_HOURS}:{Constants.PH_MINUTES}:{Constants.PH_SECONDS}\"。";
            AlignControlLeft(LabelP1, LabelInfo);
            ArrangeControlYRight(TextBoxP1, LabelInfo, 0, 3);
            CenterControlY(LabelP1, TextBoxP1, isHighDpi ? 0 : -1);
            CompactControlX(TextBoxP1, LabelP1);
            ArrangeControlYRight(TextBoxP2, TextBoxP1, 0, 3);
            ArrangeControlYRight(TextBoxP3, TextBoxP2, 0, 3);
            AlignControlLeft(LabelP2, LabelP1);
            AlignControlLeft(LabelP3, LabelP2);
            CenterControlY(LabelP2, TextBoxP2, isHighDpi ? 0 : -1);
            CenterControlY(LabelP3, TextBoxP3, isHighDpi ? 0 : -1);
            ArrangeControlYRight(ButtonB, TextBoxP3, 1, 3);
            ArrangeControlXTopRtl(ButtonA, ButtonB, -3);
            ArrangeControlXTopRtl(ButtonReset, ButtonA);
            AlignControlLeft(ButtonReset, LabelP1, 3);
        }

        protected override void OnLoad()
        {
            for (int i = 0; i < 3; i++)
            {
                TextBoxes[i].Text = CustomTexts[i];
            }
        }

        protected override bool OnClickButtonA()
        {
            string[] tmp = new string[3];
            string text;

            for (int i = 0; i < 3; i++)
            {
                text = TextBoxes[i].Text;

                if (!Validator.VerifyCustomText(text.RemoveIllegalChars(), out string ErrorMsg, i + 1) && !string.IsNullOrEmpty(ErrorMsg))
                {
                    MessageX.Error(ErrorMsg);
                    return false;
                }

                tmp[i] = text;
            }

            CustomTexts = tmp;
            return base.OnClickButtonA();
        }

        private void ItemsReset_Click(object sender, EventArgs e)
        {
            var itemIndex = ((MenuItem)sender).Index;
            TextBoxes[itemIndex].Text = Presets[itemIndex];
            UserChanged();
        }
    }
}
