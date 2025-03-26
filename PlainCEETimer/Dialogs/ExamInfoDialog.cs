using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public partial class ExamInfoDialog : AppDialog, ISubDialog<ExamInfoObject, ExamInfoDialog>
    {
        public ExamInfoObject Data { get; set; }

        private string CurrentExamName;

        public ExamInfoDialog(ExamInfoObject Existing = null) : base(AppDialogProp.BindButtons)
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
            TextBoxName_TextChanged(null, EventArgs.Empty);
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

        private void DTP_ValueChanged(object sender, EventArgs e)
        {
            UserChanged();
        }

        private void TextBoxName_TextChanged(object sender, EventArgs e)
        {
            CurrentExamName = TextBoxName.Text.RemoveIllegalChars();
            int CharCount = CurrentExamName.Length;
            LabelCounter.Text = $"{CharCount}/{ConfigPolicy.MaxExamNameLength}";
            LabelCounter.ForeColor = CharCount.IsValid() ? Color.Black : Color.Red;
            UserChanged();
        }

        protected override void ButtonA_Click()
        {
            if (string.IsNullOrWhiteSpace(CurrentExamName) || !CurrentExamName.Length.IsValid())
            {
                MessageX.Error("输入的考试名称有误！\n\n请检查输入的考试名称是否太长或太短！");
                return;
            }

            var StartTime = DTPStart.Value;
            var EndTime = DTPEnd.Value;
            var ExamSpan = EndTime - StartTime;
            var TotalSeconds = ExamSpan.TotalSeconds;

            if (EndTime <= StartTime || TotalSeconds < 1D)
            {
                MessageX.Error("考试结束时间必须在开始时间之后！");
                return;
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
                if (MessageX.Warn($"检测到设置的考试时间太长或太短！\n\n当前考试时长: {TimeMsg}。\n\n如果你确认当前设置的是正确的考试时间，请点击 是，否则请点击 否。", Buttons: AppMessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }

            Data = new()
            {
                Name = CurrentExamName,
                Start = StartTime,
                End = EndTime
            };

            base.ButtonA_Click();
        }
    }
}
