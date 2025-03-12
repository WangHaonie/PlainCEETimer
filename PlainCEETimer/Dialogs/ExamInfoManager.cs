using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;

namespace PlainCEETimer.Dialogs
{
    public partial class ExamInfoManager : AppDialog
    {
        public ExamInfoObject[] ExamInfo { get; set; }
        public bool AutoSwitch { get; set; }
        public bool ExamsChanged { get; set; }
        public int PeriodIndex { get; set; }

        public ExamInfoManager() : base(AppDialogProp.BindButtons | AppDialogProp.KeyPreview)
        {
            CompositedStyle = true;
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            LoadUI();
            LoadData();
        }

        private void LoadUI()
        {
            BindComboData(ComboBoxSwitchPeriod,
            [
                new("10 秒", 0),
                new("15 秒", 1),
                new("30 秒", 2),
                new("45 秒", 3),
                new("1 分钟", 4),
                new("2 分钟", 5),
                new("3 分钟", 6),
                new("5 分钟", 7),
                new("10 分钟", 8),
                new("15 分钟", 9),
                new("30 分钟", 10),
                new("45 分钟", 11),
                new("1 小时", 12),
            ]);
        }

        private void LoadData()
        {
            CheckBoxAutoSwitch.Checked = AutoSwitch;
            ComboBoxSwitchPeriod.SelectedIndex = PeriodIndex;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {

        }

        private void CheckBoxAutoSwitch_CheckedChanged(object sender, EventArgs e)
        {
            ComboBoxSwitchPeriod.Enabled = CheckBoxAutoSwitch.Checked;
        }

        protected override void ButtonA_Click(object sender, EventArgs e)
        {
            AutoSwitch = CheckBoxAutoSwitch.Checked;
            PeriodIndex = ComboBoxSwitchPeriod.SelectedIndex;
            base.ButtonA_Click(sender, e);
        }

        //private bool Tmp()
        //{
        //    var ExamName = TextBoxExamName.Text.RemoveIllegalChars();
        //    TimeSpan ExamTimeSpan = DtpExamEnd.Value - DtpExamStart.Value;
        //    string UniMsg = "";
        //    string TimeMsg = "";

        //    if (string.IsNullOrWhiteSpace(ExamName) || !ExamName.Length.IsValid())
        //    {
        //        MessageX.Error("输入的考试名称有误！\n\n请检查输入的考试名称是否太长或太短！", TabControlMain, TabPageGeneral);
        //        return false;
        //    }

        //    if (DtpExamEnd.Value <= DtpExamStart.Value)
        //    {
        //        MessageX.Error("考试结束时间必须在开始时间之后！", TabControlMain, TabPageGeneral);
        //        return false;
        //    }
        //    else if (ExamTimeSpan.TotalDays > 4)
        //    {
        //        TimeMsg = $"{ExamTimeSpan.TotalDays:0} 天";
        //    }
        //    else if (ExamTimeSpan.TotalMinutes < 40 && ExamTimeSpan.TotalSeconds > 60)
        //    {
        //        TimeMsg = $"{ExamTimeSpan.TotalMinutes:0} 分钟";
        //    }
        //    else if (ExamTimeSpan.TotalSeconds < 60)
        //    {
        //        TimeMsg = $"{ExamTimeSpan.TotalSeconds:0} 秒";
        //    }

        //    if (!string.IsNullOrEmpty(TimeMsg))
        //    {
        //        UniMsg = $"检测到设置的考试时间太长或太短！\n\n当前考试时长: {TimeMsg}。\n\n如果你确认当前设置的是正确的考试时间，请点击 是，否则请点击 否。";
        //    }

        //    if (!string.IsNullOrEmpty(UniMsg))
        //    {
        //        var _DialogResult = MessageX.Warn(UniMsg, TabControlMain, TabPageGeneral, Buttons: AppMessageBoxButtons.YesNo);

        //        if (_DialogResult is DialogResult.No or DialogResult.None)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //private void TextBoxExamName_TextChanged(object sender, EventArgs e)
        //{
        //    SettingsChanged(sender, e);
        //    int CharCount = TextBoxExamName.Text.RemoveIllegalChars().Length;
        //    LabelExamNameCounter.Text = $"{CharCount}/{ConfigPolicy.MaxExamNameLength}";
        //    LabelExamNameCounter.ForeColor = (CharCount.IsValid()) ? Color.Black : Color.Red;
        //}
    }
}
