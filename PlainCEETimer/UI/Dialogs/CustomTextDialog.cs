using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class CustomTextDialog : AppDialog
    {
        public string[] CustomTexts { get; set; } = new string[3];

        private Label LabelInfo;
        private Label LabelP1;
        private Label LabelP2;
        private Label LabelP3;
        private PlainTextBox TextBoxP1;
        private PlainTextBox TextBoxP2;
        private PlainTextBox TextBoxP3;
        private PlainButton ButtonReset;
        private EventHandler OnUserChanged;

        public CustomTextDialog() : base(AppFormParam.BindButtons | AppFormParam.CompositedStyle) { }

        protected override void OnInitializing()
        {
            Text = "全局自定义文本 - 高考倒计时";
            OnUserChanged = (_, _) => UserChanged();

            this.AddControls(b =>
            [
                LabelP1 = b.Label(3, 3, "考试未开始"),
                LabelP2 = b.Label("考试已开始"),
                LabelP3 = b.Label("考试已结束"),
                TextBoxP1 = b.TextBox(310, OnUserChanged),
                TextBoxP2 = b.TextBox(310, OnUserChanged),
                TextBoxP3 = b.TextBox(310, OnUserChanged),
                LabelInfo = b.Label(3, 0, null),

                ButtonReset = b.Button("重置(R)", (_, _) =>
                {
                    TextBoxP1.Text = Constants.PH_P1;
                    TextBoxP2.Text = Constants.PH_P2;
                    TextBoxP3.Text = Constants.PH_P3;
                    UserChanged();
                })
            ]);

            base.OnInitializing();
        }

        protected override void StartLayout(bool isHighDpi)
        {
            SetLabelAutoWrap(LabelInfo, TextBoxP1.Width + LabelP1.Width + ScaleToDpi(3));
            LabelInfo.Text = $"用于匹配规则之外。可用的占位符: {Constants.PH_EXAMNAME}-考试名称 {Constants.PH_DAYS}-天 {Constants.PH_HOURS}-时 {Constants.PH_MINUTES}-分 {Constants.PH_SECONDS}-秒 {Constants.PH_CEILINGDAYS}-向上取整的天数 {Constants.PH_TOTALHOURS}-总小时数 {Constants.PH_TOTALMINUTES}-总分钟数 {Constants.PH_TOTALSECONDS}-总秒数。比如 \"{Constants.PH_EXAMNAME}还有{Constants.PH_DAYS}.{Constants.PH_HOURS}:{Constants.PH_MINUTES}:{Constants.PH_SECONDS}\"。";
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
            TextBoxP1.Text = CustomTexts[0];
            TextBoxP2.Text = CustomTexts[1];
            TextBoxP3.Text = CustomTexts[2];
        }

        protected override bool OnClickButtonA()
        {
            string[] tmp = [RemoveInvalid(TextBoxP1.Text), RemoveInvalid(TextBoxP2.Text), RemoveInvalid(TextBoxP3.Text)];

            for (int i = 0; i < 3; i++)
            {
                if (!Validator.VerifyCustomText(tmp[i], out string ErrorMsg, i + 1) && !string.IsNullOrEmpty(ErrorMsg))
                {
                    MessageX.Error(ErrorMsg);
                    return false;
                }
            }

            CustomTexts = tmp;
            return base.OnClickButtonA();
        }

        private string RemoveInvalid(string s)
        {
            return s.RemoveIllegalChars();
        }
    }
}
