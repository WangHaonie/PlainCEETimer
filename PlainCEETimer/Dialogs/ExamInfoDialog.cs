using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Dialogs
{
    public sealed class ExamInfoDialog : AppDialog, IListViewSubDialog<ExamInfoObject>
    {
        public ExamInfoObject Data { get; set; }

        private string CurrentExamName;
        private PlainLabel LabelCounter;
        private PlainLabel LabelEnd;
        private DateTimePicker DTPEnd;
        private PlainLabel LabelStart;
        private DateTimePicker DTPStart;
        private PlainLabel LabelName;
        private PlainTextBox TextBoxName;
        private readonly bool IsDark = ThemeManager.ShouldUseDarkMode;

        public ExamInfoDialog(ExamInfoObject Existing = null) : base(AppFormParam.BindButtons)
        {
            SuspendLayout();
            AutoScaleDimensions = new(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new(367, 114);
            Font = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExamInfoDialog";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "考试信息 - 高考倒计时";

            this.AddControls(b =>
            [
                b.Modify(PanelMain, 3, 3, 362, 87, null, c => c.AddControls(b =>
                [
                    LabelName = b.Label(3, 6, "考试名称: "),
                    LabelStart = b.Label(3, 35, "开始日期和时间: "),
                    LabelEnd = b.Label(3, 62, "结束日期和时间: "),
                    LabelCounter = b.Label(319, 6, $"0/{Validator.MaxExamNameLength}"),

                    TextBoxName = b.TextBox(69, 2, 248, TextBoxName_TextChanged),

                    DTPStart = b.New<DateTimePicker>(109, 32, 246, 23, null, c =>
                    {
                        c.Format = DateTimePickerFormat.Custom;
                        c.CustomFormat = "yyyy-MM-dd dddd HH:mm:ss";
                        c.ValueChanged += DTP_ValueChanged;
                    }),

                    DTPEnd = b.New<DateTimePicker>(109, 58, 246, 23, null, c =>
                    {
                        c.Format = DateTimePickerFormat.Custom;
                        c.CustomFormat = "yyyy-MM-dd dddd HH:mm:ss";
                        c.ValueChanged += DTP_ValueChanged;
                    })
                ])),

                b.Modify(ButtonA, 202, 90, 75, 23, "确定(&O)", c => c.Enabled = false),
                b.Modify(ButtonB, 283, 90, 75, 23, "取消(&C)")
            ]);
            ResumeLayout(true);

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

            if (!string.IsNullOrEmpty(TimeMsg) && MessageX.Warn($"检测到设置的考试时间较长或较短！当前考试时长: {TimeMsg}。\n\n如果你确认当前设置的是正确的考试时间，请点击 是 继续，否则请点击 否。", Buttons: MessageButtons.YesNo) != DialogResult.Yes)
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
