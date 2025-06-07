using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Dialogs
{
    public sealed partial class CustomTextDialog : AppDialog
    {
        public string[] CustomTexts { get; set; } = [];

        private string P1TextRaw;
        private string P2TextRaw;
        private string P3TextRaw;
        private readonly EventHandler OnUserChanged;

        private PlainLabel LabelInfo;
        private PlainLabel LabelP1;
        private PlainLabel LabelP2;
        private PlainLabel LabelP3;
        private PlainTextBox TextBoxP1;
        private PlainTextBox TextBoxP2;
        private PlainTextBox TextBoxP3;
        private PlainButton ButtonReset;

        public CustomTextDialog() : base(AppFormParam.BindButtons | AppFormParam.CompositedStyle)
        {
            SuspendLayout();
            OnUserChanged = (_, _) => UserChanged();
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(385, 152);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CustomTextDialog";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "全局自定义文本 - 高考倒计时";

            this.AddControls(b =>
            [
                b.Modify(PanelMain, 0, 0, 382, 128, null, c => c.AddControls(b =>
                [
                    LabelP1 = b.Label(3, 55, "考试未开始"),
                    LabelP2 = b.Label(3, 81, "考试已开始"),
                    LabelP3 = b.Label(3, 107, "考试已结束"),

                    TextBoxP1 = b.TextBox(76, 51, 304, OnUserChanged),
                    TextBoxP2 = b.TextBox(76, 77, 304, OnUserChanged),
                    TextBoxP3 = b.TextBox(76, 103, 304, OnUserChanged),

                    LabelInfo = b.Label(3, 3, $"用于匹配规则之外。可用的占位符: {Constants.PH_EXAMNAME}-考试名称 {Constants.PH_DAYS}-天 {Constants.PH_HOURS}-时 {Constants.PH_MINUTES}-分 {Constants.PH_SECONDS}-秒 {Constants.PH_CEILINGDAYS}-向上取整的天数 {Constants.PH_TOTALHOURS}-总小时数 {Constants.PH_TOTALMINUTES}-总分钟数 {Constants.PH_TOTALSECONDS}-总秒数。比如 \"{Constants.PH_EXAMNAME}还有{Constants.PH_DAYS}.{Constants.PH_HOURS}:{Constants.PH_MINUTES}:{Constants.PH_SECONDS}\"。")
                ])),

                b.Modify(ButtonA, 227, 128, 75, 23, "确定(&O)"),
                b.Modify(ButtonB, 308, 128, 75, 23, "取消(&C)"),

                ButtonReset = b.Button(3, 128, "重置(R)", (_, _) =>
                {
                    TextBoxP1.Text = Constants.PH_P1;
                    TextBoxP2.Text = Constants.PH_P2;
                    TextBoxP3.Text = Constants.PH_P3;
                    UserChanged();
                })
            ]);

            ResumeLayout(true);
        }

        protected override void OnLoad()
        {
            TextBoxP1.Text = CustomTexts[0];
            TextBoxP2.Text = CustomTexts[1];
            TextBoxP3.Text = CustomTexts[2];
        }

        protected override void AdjustUI()
        {
            SetLabelAutoWrap(LabelInfo, PanelMain);
            AlignControlsL(ButtonReset, ButtonA, LabelP3);
            AlignControlsX(TextBoxP1, LabelP1);
            AlignControlsX(TextBoxP2, LabelP2);
            AlignControlsX(TextBoxP3, LabelP3);
            AlignControlsX(ButtonReset, ButtonA);
        }

        protected override bool OnClickButtonA()
        {
            P1TextRaw = RemoveInvalid(TextBoxP1.Text);
            P2TextRaw = RemoveInvalid(TextBoxP2.Text);
            P3TextRaw = RemoveInvalid(TextBoxP3.Text);
            string[] tmp = [P1TextRaw, P2TextRaw, P3TextRaw];

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

        private string RemoveInvalid(string s) => s.RemoveIllegalChars();
    }
}
