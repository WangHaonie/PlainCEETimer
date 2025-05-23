﻿using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Dialogs
{
    public sealed partial class ExamInfoDialog : AppDialog, ISubDialog<ExamInfoObject>
    {
        public ExamInfoObject Data { get; set; }

        private string CurrentExamName;
        private readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

        public ExamInfoDialog(ExamInfoObject Existing = null) : base(AppFormParam.BindButtons)
        {
            InitializeComponent();

            if (Existing != null)
            {
                TextBoxName.Text = Existing.Name;
                DTPStart.Value = Existing.Start;
                DTPEnd.Value = Existing.End;
            }
        }

        protected override void OnLoad()
        {
            TextBoxName_TextChanged(null, null);
        }

        protected override void AdjustUI()
        {
            WhenHighDpi(() =>
            {
                AlignControlsX(TextBoxName, LabelName);
                AlignControlsX(DTPStart, LabelStart);
                AlignControlsX(DTPEnd, LabelEnd);
            });
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
            var TotalSeconds = ExamSpan.TotalSeconds;

            if (EndTime <= StartTime)
            {
                MessageX.Error("考试结束时间必须在开始时间之后！");
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

            if (!string.IsNullOrEmpty(TimeMsg))
            {
                if (MessageX.Warn($"检测到设置的考试时间太长或太短！\n\n当前考试时长: {TimeMsg}。\n\n如果你确认当前设置的是正确的考试时间，请点击 是，否则请点击 否。", Buttons: MessageButtons.YesNo) != DialogResult.Yes)
                {
                    return false;
                }
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
