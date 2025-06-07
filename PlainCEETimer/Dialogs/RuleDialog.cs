using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Dialogs
{
    public sealed partial class RuleDialog : AppDialog, IListViewSubDialog<CustomRuleObject>
    {
        public string[] GlobalTexts { private get; set; }
        public ColorSetObject[] GlobalColors { private get; set; }
        public CustomRuleObject Data { get; set; }

        private bool IsEditMode;
        private ComboBoxEx ComboBoxRuleType;
        private PlainLabel LabelCustomText;
        private PlainLabel LabelColorPreview;
        private PlainLabel LabelFore;
        private PlainLabel LabelBack;
        private PlainLabel LabelChar1;
        private PlainLabel LabelChar7;
        private PlainLinkLabel LinkResetColor;
        private PlainNumericUpDown NUDDays;
        private PlainNumericUpDown NUDHours;
        private PlainNumericUpDown NUDMinutes;
        private PlainNumericUpDown NUDSeconds;
        private PlainTextBox TextBoxCustomText;
        private readonly EventHandler OnUserChanged;
        private readonly Dictionary<int, Cache> TemporaryChanges = new(3);

        private struct Cache(Color fore, Color back, string text)
        {
            public Color Fore = fore;
            public Color Back = back;
            public string Text = text;
        }

        public RuleDialog() : base(AppFormParam.AllControl | AppFormParam.CompositedStyle)
        {
            SuspendLayout();
            OnUserChanged = (_, _) => UserChanged();
            AutoScaleDimensions = new(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new(462, 102);
            Font = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "自定义规则 - 高考倒计时";

            this.AddControls(b =>
            [
                b.Modify(PanelMain, 0, 0, 454, 76, null, c => c.AddControls(b =>
                [
                    LabelChar1 = b.Label(3, 6, "当距离考试"),
                    b.Label(220, 6, "天"),
                    b.Label(284, 6, "时"),
                    b.Label(348, 6, "分"),
                    b.Label(412, 6, "秒 时,"),
                    b.Label(3, 29, "文字颜色为"),
                    LabelChar7 = b.Label(118, 29, ", 背景颜色为"),
                    LabelCustomText = b.Label(3, 53, "自定义文本"),

                    LinkResetColor = b.Link(412, 29, "重置", LinkReset_LinkClicked),
                    b.Link(412, 53, "重置", LinkReset_LinkClicked),

                    LabelFore = b.Block(77, 29, ColorLabels_Click),
                    LabelBack = b.Block(198, 29, ColorLabels_Click),
                    LabelColorPreview = b.Block(323, 29, ColorLabels_Click, "颜色效果预览"),

                    TextBoxCustomText = b.TextBox(77, 50, 333, (_, _) =>
                    {
                        if (!IsEditMode)
                        {
                            WhenLoaded(SaveTemp);
                        }

                        UserChanged();
                    }),

                    ComboBoxRuleType = b.ComboBox(77, 2, 82, (_, _) =>
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
                    },
                    [
                        new(Constants.PH_RTP1, 0),
                        new(Constants.PH_RTP2, 1),
                        new(Constants.PH_RTP3, 2)
                    ]),

                    NUDDays = b.NumericUpDown(165, 2, 53, 65535M, OnUserChanged),
                    NUDHours = b.NumericUpDown(242, 2, 40, 23M, OnUserChanged),
                    NUDMinutes = b.NumericUpDown(306, 2, 40, 59M, OnUserChanged),
                    NUDSeconds = b.NumericUpDown(370, 2, 40, 59M, OnUserChanged)
                ])),

                b.Modify(ButtonA, 298, 77, 75, 23, "确定(&O)", c => c.Enabled = false),
                b.Modify(ButtonB, 379, 77, 75, 23, "取消(&C)")
            ]);


            ResumeLayout(true);
        }

        protected override void OnLoad()
        {
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

        protected override bool OnClickButtonA()
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

            return base.OnClickButtonA();
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
