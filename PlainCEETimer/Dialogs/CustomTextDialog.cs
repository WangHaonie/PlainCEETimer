using System;
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

        public CustomTextDialog() : base(AppFormParam.BindButtons | AppFormParam.CompositedStyle)
        {
            InitializeComponent();
            OnUserChanged = new((_, _) => UserChanged());
        }

        protected override void OnLoad()
        {
            TextBoxP1.Text = CustomTexts[0];
            TextBoxP2.Text = CustomTexts[1];
            TextBoxP3.Text = CustomTexts[2];
            TextBoxP1.TextChanged += OnUserChanged;
            TextBoxP2.TextChanged += OnUserChanged;
            TextBoxP3.TextChanged += OnUserChanged;

            LabelInfo.Text = $"用于匹配规则之外。可用的占位符: {Constants.PH_EXAMNAME}-考试名称 {Constants.PH_DAYS}-天 {Constants.PH_HOURS}-时 {Constants.PH_MINUTES}-分 {Constants.PH_SECONDS}-秒 {Constants.PH_CEILINGDAYS}-向上取整的天数 {Constants.PH_TOTALHOURS}-总小时数 {Constants.PH_TOTALMINUTES}-总分钟数 {Constants.PH_TOTALSECONDS}-总秒数。比如 \"{Constants.PH_EXAMNAME}还有{Constants.PH_DAYS}.{Constants.PH_HOURS}:{Constants.PH_MINUTES}:{Constants.PH_SECONDS}\"。";
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

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            TextBoxP1.Text = Constants.PH_P1;
            TextBoxP2.Text = Constants.PH_P2;
            TextBoxP3.Text = Constants.PH_P3;
            UserChanged();
        }

        private string RemoveInvalid(string s) => s.RemoveIllegalChars();
    }
}
