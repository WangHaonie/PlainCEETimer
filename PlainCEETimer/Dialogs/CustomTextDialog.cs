using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using System;

namespace PlainCEETimer.Dialogs
{
    public partial class CustomTextDialog : AppDialog
    {
        public string[] CustomTexts { get; set; } = [];

        private string P1TextRaw;
        private string P2TextRaw;
        private string P3TextRaw;
        private readonly EventHandler OnUserChanged;

        public CustomTextDialog() : base(AppDialogProp.BindButtons)
        {
            CompositedStyle = true;
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

            LabelInfo.Text = $"用于匹配规则之外。可用的占位符: {Placeholders.PH_PHINFO}。比如 \"{Placeholders.PH_EXAMNAME}还有{Placeholders.PH_DAYS}.{Placeholders.PH_HOURS}:{Placeholders.PH_MINUTES}:{Placeholders.PH_SECONDS}\"。";
        }

        protected override void AdjustUI()
        {
            SetLabelAutoWrap(LabelInfo, PanelMain);
            AlignControlsL(ButtonReset, ButtonA, LabelP3);
            AlignControlsX(TextBoxP1, LabelP1);
            AlignControlsX(TextBoxP2, LabelP2);
            AlignControlsX(TextBoxP3, LabelP3);
            AlignControlsX(ButtonReset, ButtonA);
            SetTextBoxMax(TextBoxP1, Validator.MaxCustomTextLength);
            SetTextBoxMax(TextBoxP2, Validator.MaxCustomTextLength);
            SetTextBoxMax(TextBoxP3, Validator.MaxCustomTextLength);
        }

        protected override void ButtonA_Click()
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
                    return;
                }
            }

            CustomTexts = tmp;
            base.ButtonA_Click();
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            TextBoxP1.Text = Placeholders.PH_P1;
            TextBoxP2.Text = Placeholders.PH_P2;
            TextBoxP3.Text = Placeholders.PH_P3;
            UserChanged();
        }

        private string RemoveInvalid(string s)
        {
            return s.RemoveIllegalChars();
        }
    }
}
