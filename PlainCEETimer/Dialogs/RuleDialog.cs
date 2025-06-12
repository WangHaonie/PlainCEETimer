using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.WinForms;

namespace PlainCEETimer.Dialogs
{
    public sealed class RuleDialog : AppDialog, IListViewSubDialog<CustomRuleObject>
    {
        public string[] GlobalTexts { private get; set; }
        public ColorSetObject[] GlobalColors { private get; set; }
        public CustomRuleObject Data { get; set; }

        private bool IsEditMode;
        private ComboBoxEx ComboBoxRuleType;
        private Label LabelCharExam;
        private Label LabelCharDay;
        private Label LabelCharHour;
        private Label LabelCharMinute;
        private Label LabelCharSecond;
        private Label LabelFore;
        private Label LabelBack;
        private Label LabelCustomText;
        private Label BlockPreview;
        private Label BlockFore;
        private Label BlockBack;
        private PlainLinkLabel LinkResetColor;
        private PlainLinkLabel LinkResetText;
        private PlainNumericUpDown NUDDays;
        private PlainNumericUpDown NUDHours;
        private PlainNumericUpDown NUDMinutes;
        private PlainNumericUpDown NUDSeconds;
        private PlainTextBox TextBoxCustomText;
        private EventHandler OnUserChanged;
        private readonly Dictionary<int, Cache> TemporaryChanges = new(3);

        private struct Cache(Color fore, Color back, string text)
        {
            public Color Fore = fore;
            public Color Back = back;
            public string Text = text;
        }

        public RuleDialog() : base(AppFormParam.AllControl | AppFormParam.CompositedStyle) { }

        protected override void OnInitializing()
        {
            Text = "自定义规则 - 高考倒计时";
            OnUserChanged = (_, _) => UserChanged();

            this.AddControls(b =>
            [
                LabelCharExam = b.Label(3, 3, "距离考试"),
                LabelCharDay = b.Label("天"),
                LabelCharHour = b.Label("时"),
                LabelCharMinute = b.Label("分"),
                LabelCharSecond = b.Label("秒"),
                LabelFore = b.Label("文字颜色"),
                LabelBack = b.Label("背景颜色"),
                LabelCustomText = b.Label("自定义文本"),
                LinkResetColor = b.Link("重置", LinkReset_LinkClicked),
                LinkResetText = b.Link("重置", LinkReset_LinkClicked),
                BlockFore = b.Block(ColorLabels_Click),
                BlockBack = b.Block(ColorLabels_Click),
                BlockPreview = b.Block("颜色效果预览", ColorLabels_Click),

                TextBoxCustomText = b.TextBox(295, (_, _) =>
                {
                    if (!IsEditMode)
                    {
                        WhenLoaded(SaveTemp);
                    }

                    UserChanged();
                }),

                ComboBoxRuleType = b.ComboBox(82, (_, _) =>
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

                NUDDays = b.NumericUpDown(53, 65535M, OnUserChanged),
                NUDHours = b.NumericUpDown(40, 23M, OnUserChanged),
                NUDMinutes = b.NumericUpDown(40, 59M, OnUserChanged),
                NUDSeconds = b.NumericUpDown(40, 59M, OnUserChanged),
            ]);

            base.OnInitializing();
        }

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeControlXTop(ComboBoxRuleType, LabelCharExam);
            CenterControlY(LabelCharExam, ComboBoxRuleType);
            ArrangeControlXTop(NUDDays, ComboBoxRuleType, 6, 0);
            ArrangeControlXRightTop(LabelCharDay, NUDDays, LabelCharExam);
            ArrangeControlXRightTop(NUDHours, LabelCharDay, NUDDays);
            ArrangeControlXRightTop(LabelCharHour, NUDHours, LabelCharDay);
            ArrangeControlXRightTop(NUDMinutes, LabelCharHour, NUDHours);
            ArrangeControlXRightTop(LabelCharMinute, NUDMinutes, LabelCharHour);
            ArrangeControlXRightTop(NUDSeconds, LabelCharMinute, NUDMinutes);
            ArrangeControlXRightTop(LabelCharSecond, NUDSeconds, LabelCharMinute);
            ArrangeControlYLeft(BlockFore, ComboBoxRuleType, 0, 3);
            ArrangeControlXTopRtl(LabelFore, BlockFore);
            AlignControlLeft(LabelFore, LabelCharExam);
            CenterControlY(LabelFore, BlockFore);
            ArrangeControlXRightTop(LabelBack, BlockFore, LabelFore);
            ArrangeControlXRightTop(BlockBack, LabelBack, BlockFore);
            ArrangeControlYRight(LinkResetColor, LabelCharSecond);
            AlignControlTop(LinkResetColor, LabelBack, -1);
            ArrangeControlXLeftTopRtl(BlockPreview, LinkResetColor, BlockBack);
            ArrangeControlYLeft(TextBoxCustomText, BlockFore, 0, 3);
            CenterControlY(LabelCustomText, TextBoxCustomText, isHighDpi ? 0 : -1);
            AlignControlLeft(LabelCustomText, LabelFore);
            CompactControlX(TextBoxCustomText, LabelCustomText);
            ArrangeControlXTop(LinkResetText, TextBoxCustomText);
            AlignControlTop(LinkResetText, LabelCustomText, -1);
            AlignControlLeft(LinkResetText, LinkResetColor);
            ArrangeControlYRight(ButtonB, LinkResetText, -3);
            CompactControlY(ButtonB, TextBoxCustomText, 3);
            ArrangeControlXTopRtl(ButtonA, ButtonB, -3);
        }

        protected override void OnLoad()
        {
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

            var Fore = BlockFore.BackColor;
            var Back = BlockBack.BackColor;

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
            using var ColorDialogMain = new ColorDialogEx();

            if (ColorDialogMain.ShowDialog(LabelSender.BackColor, this) == DialogResult.OK)
            {
                LabelSender.BackColor = ColorDialogMain.Color;
                BlockPreview.ForeColor = BlockFore.BackColor;
                BlockPreview.BackColor = BlockBack.BackColor;
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
            TemporaryChanges[ComboBoxRuleType.SelectedIndex] = new(BlockFore.BackColor, BlockBack.BackColor, TextBoxCustomText.Text);
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
            BlockFore.BackColor = fore;
            BlockPreview.ForeColor = fore;
            BlockBack.BackColor = back;
            BlockPreview.BackColor = back;
        }
    }
}
