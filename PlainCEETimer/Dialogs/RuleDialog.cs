using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public partial class RuleDialog : AppDialog, ISubDialog<CustomRuleObject>
    {
        public string[] GlobalTexts { private get; set; }
        public ColorSetObject[] GlobalColors { private get; set; }
        public CustomRuleObject Data { get; set; }

        private bool IsEditMode;
        private readonly Dictionary<int, Cache> TemporaryChanges = new(3);

        private struct Cache(Color fore, Color back, string text)
        {
            public Color Fore = fore;
            public Color Back = back;
            public string Text = text;
        }

        public RuleDialog() : base(AppFormParam.AllControl | AppFormParam.CompositedStyle)
        {
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            BindComboData(ComboBoxRuleType,
            [
                new($"开始{Constants.PH_START}", 0),
                new(Constants.PH_LEFT, 1),
                new(Constants.PH_PAST, 2)
            ]);

            LabelFore.Click += ColorLabels_Click;
            LabelBack.Click += ColorLabels_Click;
            IsEditMode = Data != null;

            if (IsEditMode)
            {
                ComboBoxRuleType.SelectedIndex = (int)Data.Phase;
                var Ticks = Data.Tick;
                var tmp = Data.Colors;
                NUDDays.Value = Ticks.Days;
                NUDHours.Value = Ticks.Hours;
                NUDMinutes.Value = Ticks.Minutes;
                NUDSeconds.Value = Ticks.Seconds;
                ApplyColorBlock(tmp.Fore, tmp.Back);
                TextBoxCustomText.Text = Data.Text;
            }
            else
            {
                GetNewData();
            }

            ComboBoxRuleType.SelectedIndexChanged += ComboBoxRuleType_SelectedIndexChanged;
            TextBoxCustomText.TextChanged += TextBoxCustomText_TextChanged;
        }

        protected override void AdjustUI()
        {
            CompactControlsX(LabelChar7, LabelFore);
            CompactControlsX(LabelBack, LabelChar7);

            WhenHighDpi(() =>
            {
                AlignControlsX([ComboBoxRuleType, NUDDays, NUDHours, NUDMinutes, NUDSeconds], LabelChar1);
                AlignControlsX(TextBoxCustomText, LabelCustomText);

            });
        }

        protected override bool ButtonA_Click()
        {
            var d = (int)NUDDays.Value;
            var h = (int)NUDHours.Value;
            var m = (int)NUDMinutes.Value;
            var s = (int)NUDSeconds.Value;

            if (d == 0 && m == 0 && h == 0 && s == 0)
            {
                MessageX.Error("时刻不能为0，请重新设置！");
                return false;
            }

            var Fore = LabelFore.BackColor;
            var Back = LabelBack.BackColor;

            if (!Validator.IsNiceContrast(Fore, Back))
            {
                MessageX.Error("选择的颜色相似或对比度较低，将无法看清文字。\n\n请尝试更换其它背景颜色或文字颜色！");
                return false;
            }

            var Text = TextBoxCustomText.Text.RemoveIllegalChars();

            if (!Validator.VerifyCustomText(Text, out string ErrorMsg) && !string.IsNullOrEmpty(ErrorMsg))
            {
                MessageX.Error(ErrorMsg);
                return false;
            }

            Data = new()
            {
                Phase = (CountdownPhase)ComboBoxRuleType.SelectedIndex,
                Tick = new(d, h, m, s),
                Text = Text,
                Colors = new(Fore, Back)
            };

            return base.ButtonA_Click();
        }

        private void ComboBoxRuleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsEditMode)
            {
                WhenLoaded(() =>
                {
                    var Index = ComboBoxRuleType.SelectedIndex;

                    if (TemporaryChanges.ContainsKey(Index))
                    {
                        var Temp = TemporaryChanges[Index];
                        ApplyColorBlock(Temp.Fore, Temp.Back);
                        TextBoxCustomText.Text = Temp.Text;
                    }
                    else
                    {
                        GetNewData();
                    }
                });
            }

            UserChanged();
        }

        private void NUD_TextChanged(object sender, EventArgs e)
        {
            UserChanged();
        }

        private void ColorLabels_Click(object sender, EventArgs e)
        {
            var LabelSender = (Label)sender;
            var ColorDialogMain = new ColorDialogEx();

            if (ColorDialogMain.ShowDialog(LabelSender.BackColor, this) == DialogResult.OK)
            {
                LabelSender.BackColor = ColorDialogMain.Color;
                LabelColorPreview.ForeColor = LabelFore.BackColor;
                LabelColorPreview.BackColor = LabelBack.BackColor;
                UserChanged();

                if (!IsEditMode)
                {
                    SaveTemp();
                }
            }
        }

        private void TextBoxCustomText_TextChanged(object sender, EventArgs e)
        {
            if (!IsEditMode)
            {
                WhenLoaded(SaveTemp);
            }

            UserChanged();
        }

        private void LinkReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var flag = (LinkLabel)sender == LinkResetColor;
            GetNewData(false, flag, !flag);
            UserChanged();
        }

        private void SaveTemp()
        {
            TemporaryChanges[ComboBoxRuleType.SelectedIndex] = new(LabelFore.BackColor, LabelBack.BackColor, TextBoxCustomText.Text);
        }

        private void GetNewData(bool All = true, bool ColorOnly = false, bool TextOnly = false)
        {
            var Index = ComboBoxRuleType.SelectedIndex;

            if (All || ColorOnly)
            {
                var Colors = GlobalColors[Index];
                ApplyColorBlock(Colors.Fore, Colors.Back);
            }

            if (All || TextOnly)
            {
                TextBoxCustomText.Text = GlobalTexts[Index];
            }

            if (!IsEditMode)
            {
                SaveTemp();
            }
        }

        private void ApplyColorBlock(Color fore, Color back)
        {
            LabelFore.BackColor = fore;
            LabelColorPreview.ForeColor = fore;
            LabelBack.BackColor = back;
            LabelColorPreview.BackColor = back;
        }
    }
}
