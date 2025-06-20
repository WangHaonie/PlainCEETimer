﻿using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class ExamInfoDialog(ExamInfoObject Existing) : AppDialog(AppFormParam.BindButtons), IListViewSubDialog<ExamInfoObject>
    {
        public ExamInfoObject Data { get; set; } = Existing;

        private Label LabelName;
        private Label LabelCounter;
        private Label LabelStart;
        private Label LabelEnd;
        private PlainTextBox TextBoxName;
        private DateTimePicker DTPStart;
        private DateTimePicker DTPEnd;
        private string CurrentExamName;
        private readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

        protected override void OnInitializing()
        {
            Text = "考试信息 - 高考倒计时";
            AutoSizeMode = AutoSizeMode.GrowOnly;

            this.AddControls(b =>
            [
                LabelName = b.Label("考试名称"),
                LabelStart = b.Label("考试开始"),
                LabelEnd = b.Label("考试结束"),
                LabelCounter = b.Label("00/00"),
                TextBoxName = b.TextBox(215, TextBoxName_TextChanged).With(c => c.MaxLength = 99),
                DTPStart = b.DateTimePicker(250, DTP_ValueChanged),
                DTPEnd = b.DateTimePicker(250, DTP_ValueChanged),
            ]);

            base.OnInitializing();
        }

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeFirstControl(LabelName, 3, 6);
            ArrangeControlXT(TextBoxName, LabelName);
            CenterControlY(LabelName, TextBoxName, isHighDpi ? 0 : -1);
            ArrangeControlXRT(LabelCounter, TextBoxName, LabelName, 2);
            ArrangeControlYL(LabelStart, LabelName);
            ArrangeControlYL(LabelEnd, LabelStart);
            ArrangeControlYL(DTPStart, TextBoxName, 0, 3);
            ArrangeControlYL(DTPEnd, DTPStart, 0, 3);
            CenterControlY(LabelStart, DTPStart);
            CenterControlY(LabelEnd, DTPEnd);
            ArrangeCommonButtonsR(ButtonA, ButtonB, DTPEnd, 1, 3);
        }

        protected override void OnLoad()
        {
            if (Data != null)
            {
                TextBoxName.Text = Data.Name;
                DTPStart.Value = Data.Start;
                DTPEnd.Value = Data.End;
            }

            TextBoxName_TextChanged(null, null);
        }

        protected override bool OnClickButtonA()
        {
            if (string.IsNullOrWhiteSpace(CurrentExamName) || !Validator.IsValidExamLength(CurrentExamName.Length))
            {
                MessageX.Error("输入的考试名称有误！\n\n请检查输入的考试名称是否太长或太短！");
                return false;
            }

            var StartTime = DTPStart.Value;
            var EndTime = DTPEnd.Value;
            var ExamSpan = EndTime - StartTime;
            var TotalSeconds = (int)ExamSpan.TotalSeconds;

            if (EndTime <= StartTime || TotalSeconds < 1)
            {
                MessageX.Error("考试时长无效！请检查相应日期时间是否合理。");
                return false;
            }

            var TimeMsg = "";

            if (ExamSpan.TotalDays > 4)
            {
                TimeMsg = $"{ExamSpan.TotalDays:0} 天";
            }
            else if (ExamSpan.TotalMinutes < 40 && TotalSeconds > 60)
            {
                TimeMsg = $"{ExamSpan.TotalMinutes:0} 分钟";
            }
            else if (TotalSeconds < 60)
            {
                TimeMsg = $"{TotalSeconds:0} 秒";
            }

            if (!string.IsNullOrEmpty(TimeMsg) && MessageX.Warn($"检测到设置的考试时间较长或较短！当前考试时长: {TimeMsg}。\n\n如果你确认当前设置的是正确的考试时间，请点击 是 继续，否则请点击 否。", buttons: MessageButtons.YesNo) != DialogResult.Yes)
            {
                return false;
            }

            Data = new()
            {
                Name = CurrentExamName,
                Start = StartTime,
                End = EndTime
            };

            return base.OnClickButtonA();
        }

        private void DTP_ValueChanged(object sender, EventArgs e)
        {
            UserChanged();
        }

        private void TextBoxName_TextChanged(object sender, EventArgs e)
        {
            CurrentExamName = TextBoxName.Text.RemoveIllegalChars();
            int CharCount = CurrentExamName.Length;
            LabelCounter.Text = $"{CharCount}/{Validator.MaxExamNameLength}";
            LabelCounter.ForeColor = Validator.IsValidExamLength(CharCount) ? (IsDark ? ThemeManager.DarkFore : Color.Black) : Color.Red;
            UserChanged();
        }
    }
}
