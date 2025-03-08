using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using System;

namespace PlainCEETimer.Dialogs
{
    public partial class CustomTextDialog : DialogEx
    {
        public string[] CustomText { get; set; } = [];

        private string P1TextRaw;
        private string P2TextRaw;
        private string P3TextRaw;
        private readonly EventHandler OnUserChanged;

        public CustomTextDialog() : base(DialogExProp.BindButtons)
        {
            InitializeComponent();
            OnUserChanged = new((_, _) => UserChanged());
        }

        protected override void OnLoad()
        {
            TextBoxP1.Text = CustomText[0];
            TextBoxP2.Text = CustomText[1];
            TextBoxP3.Text = CustomText[2];
            TextBoxP1.TextChanged += OnUserChanged;
            TextBoxP2.TextChanged += OnUserChanged;
            TextBoxP3.TextChanged += OnUserChanged;

            LabelInfo.Text = $"自定义倒计时的内容，用占位符表示变量:\n{Placeholders.PH_PHINFO}\n比如 \"{Placeholders.PH_EXAMNAME}还有{Placeholders.PH_DAYS}.{Placeholders.PH_HOURS}:{Placeholders.PH_MINUTES}:{Placeholders.PH_SECONDS}\"。";
        }

        protected override void AdjustUI()
        {
            SetLabelAutoWrap(LabelInfo, PanelMain);
            AdjustPanel();
            AlignControlsL(ButtonReset, ButtonA, LabelP3);
            AlignControlsX(TextBoxP1, LabelP1);
            AlignControlsX(TextBoxP2, LabelP2);
            AlignControlsX(TextBoxP3, LabelP3);
            AlignControlsX(ButtonReset, ButtonA);
            SetTextBoxMax(TextBoxP1, ConfigPolicy.MaxCustomTextLength);
            SetTextBoxMax(TextBoxP2, ConfigPolicy.MaxCustomTextLength);
            SetTextBoxMax(TextBoxP3, ConfigPolicy.MaxCustomTextLength);
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            UserChanged();

            TextBoxP1.Text = Placeholders.PH_P1;
            TextBoxP2.Text = Placeholders.PH_P2;
            TextBoxP3.Text = Placeholders.PH_P3;
        }

        protected override void ButtonA_Click()
        {
            P1TextRaw = RemoveInvalid(TextBoxP1.Text);
            P2TextRaw = RemoveInvalid(TextBoxP2.Text);
            P3TextRaw = RemoveInvalid(TextBoxP3.Text);
            string[] tmp = [P1TextRaw, P2TextRaw, P3TextRaw];

            if (!(bool)CustomRuleHelper.CheckCustomText(tmp, out string ErrorMsg) && !string.IsNullOrWhiteSpace(ErrorMsg))
            {
                MessageX.Error(ErrorMsg);
                return;
            }

            CustomText = tmp;
            base.ButtonA_Click();
        }

        private string RemoveInvalid(string s)
        {
            return s.RemoveIllegalChars();
        }
    }
}
