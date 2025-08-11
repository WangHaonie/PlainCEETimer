using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class RuleDialog : AppDialog, IListViewSubDialog<CustomRuleObject>
    {
        public string[] GlobalTexts { private get; set; }
        public ColorSetObject[] GlobalColors { private get; set; }
        public CustomRuleObject Data { get; set; }

        protected override AppFormParam Params => AppFormParam.AllControl | AppFormParam.CompositedStyle;

        private bool IsEditMode;
        private ColorBlock BlockPreview;
        private ColorBlock BlockFore;
        private ColorBlock BlockBack;
        private PlainComboBox ComboBoxRuleType;
        private PlainLabel LabelCharExam;
        private PlainLabel LabelCharDay;
        private PlainLabel LabelCharHour;
        private PlainLabel LabelCharMinute;
        private PlainLabel LabelCharSecond;
        private PlainLabel LabelFore;
        private PlainLabel LabelBack;
        private PlainLabel LabelCustomText;
        private PlainLinkLabel LinkResetColor;
        private PlainLinkLabel LinkResetText;
        private PlainNumericUpDown NUDDays;
        private PlainNumericUpDown NUDHours;
        private PlainNumericUpDown NUDMinutes;
        private PlainNumericUpDown NUDSeconds;
        private PlainTextBox TextBoxCustomText;
        private EventHandler OnUserChanged;
        private readonly Dictionary<int, Temp> TemporaryChanges = new(3);

        private struct Temp(ColorSetObject colors, string text)
        {
            public ColorSetObject Colors = colors;
            public string Text = text;
        }

        protected override void OnInitializing()
        {
            Text = "自定义规则 - 高考倒计时";
            OnUserChanged = (_, _) => UserChanged();

            this.AddControls(b =>
            [
                LabelCharExam = b.Label("距离考试"),
                LabelCharDay = b.Label("天"),
                LabelCharHour = b.Label("时"),
                LabelCharMinute = b.Label("分"),
                LabelCharSecond = b.Label("秒"),
                LabelFore = b.Label("文字颜色"),
                LabelBack = b.Label("背景颜色"),
                LabelCustomText = b.Label("自定义文本"),
                LinkResetColor = b.Link("重置", LinkReset_LinkClicked),
                LinkResetText = b.Link("重置", LinkReset_LinkClicked),
                BlockPreview = b.Block("颜色效果预览"),
                BlockFore = b.Block(true, BlockPreview, ColorBlocks_Click),
                BlockBack = b.Block(false, BlockPreview, ColorBlocks_Click),

                TextBoxCustomText = b.TextBox(295, true, (_, _) =>
                {
                    if (!IsEditMode)
                    {
                        EnsureLoaded(SaveTemp);
                    }

                    UserChanged();
                }),

                ComboBoxRuleType = b.ComboBox(82, (_, _) =>
                {
                    if (!IsEditMode)
                    {
                        EnsureLoaded(() =>
                        {
                            var index = ComboBoxRuleType.SelectedIndex;

                            if (TemporaryChanges.ContainsKey(index))
                            {
                                var tmp = TemporaryChanges[index];
                                ApplyColorBlock(tmp.Colors);
                                TextBoxCustomText.Text = tmp.Text;
                            }
                            else
                            {
                                GetNewData();
                            }
                        });
                    }

                    UserChanged();
                }, Constants.PhRuleTypes),

                NUDDays = b.NumericUpDown(53, 65535M, OnUserChanged),
                NUDHours = b.NumericUpDown(40, 23M, OnUserChanged),
                NUDMinutes = b.NumericUpDown(40, 59M, OnUserChanged),
                NUDSeconds = b.NumericUpDown(40, 59M, OnUserChanged),
            ]);

            ColorBlock[] blocks = [BlockFore, BlockBack];

            foreach (var block in blocks)
            {
                block.Fellows = blocks;
            }

            base.OnInitializing();
        }

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeFirstControl(LabelCharExam);
            ArrangeControlXT(ComboBoxRuleType, LabelCharExam);
            CenterControlY(LabelCharExam, ComboBoxRuleType);
            ArrangeControlXT(NUDDays, ComboBoxRuleType, 6, 0);
            ArrangeControlXRT(LabelCharDay, NUDDays, LabelCharExam);
            ArrangeControlXRT(NUDHours, LabelCharDay, NUDDays);
            ArrangeControlXRT(LabelCharHour, NUDHours, LabelCharDay);
            ArrangeControlXRT(NUDMinutes, LabelCharHour, NUDHours);
            ArrangeControlXRT(LabelCharMinute, NUDMinutes, LabelCharHour);
            ArrangeControlXRT(NUDSeconds, LabelCharMinute, NUDMinutes);
            ArrangeControlXRT(LabelCharSecond, NUDSeconds, LabelCharMinute);
            ArrangeControlYL(LabelFore, LabelCharExam);
            ArrangeControlYL(BlockFore, ComboBoxRuleType, 0, 3);
            CenterControlY(LabelFore, BlockFore);
            ArrangeControlXRT(LabelBack, BlockFore, LabelFore);
            ArrangeControlXRT(BlockBack, LabelBack, BlockFore);
            ArrangeControlYL(LabelCustomText, LabelFore);
            ArrangeControlYL(TextBoxCustomText, BlockFore, 0, 3);
            CenterControlY(LabelCustomText, TextBoxCustomText, -1);
            CompactControlX(TextBoxCustomText, LabelCustomText);
            ArrangeControlXRT(LinkResetText, TextBoxCustomText, LabelCustomText);
            ArrangeControlXT(BlockPreview, BlockBack);
            AlignControlXR(BlockPreview, TextBoxCustomText);
            ArrangeControlXRT(LinkResetColor, BlockPreview, LabelBack);
            ArrangeCommonButtonsR(ButtonA, ButtonB, LinkResetText, -3, 6);
        }

        protected override void OnLoad()
        {
            IsEditMode = Data != null;

            if (IsEditMode)
            {
                ComboBoxRuleType.SelectedIndex = (int)Data.Phase;
                var tick = Data.Tick;
                NUDDays.Value = tick.Days;
                NUDHours.Value = tick.Hours;
                NUDMinutes.Value = tick.Minutes;
                NUDSeconds.Value = tick.Seconds;
                ApplyColorBlock(Data.Colors);
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

            var fore = BlockFore.Color;
            var back = BlockBack.Color;

            if (!Validator.IsNiceContrast(fore, back))
            {
                MessageX.Error("选择的颜色相似或对比度较低，将无法看清文字。\n\n请尝试更换其它背景颜色或文字颜色！");
                return false;
            }

            var content = TextBoxCustomText.Text.RemoveIllegalChars();

            if (!Validator.VerifyCustomText(content, out string ErrorMsg) && !string.IsNullOrEmpty(ErrorMsg))
            {
                MessageX.Error(ErrorMsg);
                return false;
            }

            Data = new()
            {
                Phase = (CountdownPhase)ComboBoxRuleType.SelectedIndex,
                Tick = new(d, h, m, s),
                Text = content,
                Colors = new(fore, back)
            };

            return base.OnClickButtonA();
        }

        private void ColorBlocks_Click(object sender, EventArgs e)
        {
            UserChanged();

            if (!IsEditMode)
            {
                SaveTemp();
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
            TemporaryChanges[ComboBoxRuleType.SelectedIndex] = new(new(BlockFore.Color, BlockBack.Color), TextBoxCustomText.Text);
        }

        private void GetNewData(bool all = true, bool colorOnly = false, bool textOnly = false)
        {
            var index = ComboBoxRuleType.SelectedIndex;

            if (all || colorOnly)
            {
                ApplyColorBlock(GlobalColors[index]);
            }

            if (all || textOnly)
            {
                TextBoxCustomText.Text = GlobalTexts[index];
            }

            if (!IsEditMode)
            {
                SaveTemp();
            }
        }

        private void ApplyColorBlock(ColorSetObject colors)
        {
            BlockFore.Color = colors.Fore;
            BlockBack.Color = colors.Back;
        }
    }
}
