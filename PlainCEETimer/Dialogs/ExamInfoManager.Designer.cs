namespace PlainCEETimer.Dialogs
{
    partial class ExamInfoManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //this.PanelMain = new System.Windows.Forms.Panel();
            //this.ButtonA = new System.Windows.Forms.Button();
            //this.ButtonB = new System.Windows.Forms.Button();
            this.LabelInfo = new System.Windows.Forms.Label();
            this.ListViewMain = new PlainCEETimer.Controls.ListViewEx();
            this.ColumnHeadExamName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeadStart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeadEnd = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GroupBoxEdit = new System.Windows.Forms.GroupBox();
            this.ButtonChange = new System.Windows.Forms.Button();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.LabelEnd = new System.Windows.Forms.Label();
            this.DTPEnd = new System.Windows.Forms.DateTimePicker();
            this.LabelStart = new System.Windows.Forms.Label();
            this.DTPStart = new System.Windows.Forms.DateTimePicker();
            this.LabelName = new System.Windows.Forms.Label();
            this.TextBoxName = new System.Windows.Forms.TextBox();
            this.GroupBoxAutoSwitch = new System.Windows.Forms.GroupBox();
            this.ComboBoxSwitchPeriod = new System.Windows.Forms.ComboBox();
            this.LabelSwitchPeriod = new System.Windows.Forms.Label();
            this.CheckBoxAutoSwitch = new System.Windows.Forms.CheckBox();
            this.PanelMain.SuspendLayout();
            this.GroupBoxEdit.SuspendLayout();
            this.GroupBoxAutoSwitch.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelInfo
            // 
            this.LabelInfo.AutoSize = true;
            this.LabelInfo.Location = new System.Drawing.Point(3, 3);
            this.LabelInfo.Name = "LabelInfo";
            this.LabelInfo.Size = new System.Drawing.Size(319, 15);
            this.LabelInfo.TabIndex = 1;
            this.LabelInfo.Text = "你可以在这里添加多个考试信息以便快速切换倒计时。";
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.LabelInfo);
            this.PanelMain.Controls.Add(this.ListViewMain);
            this.PanelMain.Location = new System.Drawing.Point(6, 6);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(534, 167);
            this.PanelMain.TabIndex = 2;
            // 
            // ListViewMain
            // 
            this.ListViewMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeadExamName,
            this.ColumnHeadStart,
            this.ColumnHeadEnd});
            this.ListViewMain.FullRowSelect = true;
            this.ListViewMain.GridLines = true;
            this.ListViewMain.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListViewMain.HideSelection = false;
            this.ListViewMain.Location = new System.Drawing.Point(3, 21);
            this.ListViewMain.Name = "ListViewMain";
            this.ListViewMain.Size = new System.Drawing.Size(523, 139);
            this.ListViewMain.TabIndex = 0;
            this.ListViewMain.UseCompatibleStateImageBehavior = false;
            this.ListViewMain.View = System.Windows.Forms.View.Details;
            // 
            // ColumnHeadExamName
            // 
            this.ColumnHeadExamName.Text = "考试名称";
            this.ColumnHeadExamName.Width = 100;
            // 
            // ColumnHeadStart
            // 
            this.ColumnHeadStart.Text = "开始日期和时间";
            this.ColumnHeadStart.Width = 180;
            // 
            // ColumnHeadEnd
            // 
            this.ColumnHeadEnd.Text = "结束日期和时间";
            this.ColumnHeadEnd.Width = 180;
            // 
            // GroupBoxEdit
            // 
            this.GroupBoxEdit.Controls.Add(this.ButtonChange);
            this.GroupBoxEdit.Controls.Add(this.ButtonAdd);
            this.GroupBoxEdit.Controls.Add(this.LabelEnd);
            this.GroupBoxEdit.Controls.Add(this.DTPEnd);
            this.GroupBoxEdit.Controls.Add(this.LabelStart);
            this.GroupBoxEdit.Controls.Add(this.DTPStart);
            this.GroupBoxEdit.Controls.Add(this.LabelName);
            this.GroupBoxEdit.Controls.Add(this.TextBoxName);
            this.GroupBoxEdit.Location = new System.Drawing.Point(9, 172);
            this.GroupBoxEdit.Name = "GroupBoxEdit";
            this.GroupBoxEdit.Size = new System.Drawing.Size(361, 132);
            this.GroupBoxEdit.TabIndex = 3;
            this.GroupBoxEdit.TabStop = false;
            this.GroupBoxEdit.Text = "编辑考试信息";
            // 
            // ButtonChange
            // 
            this.ButtonChange.Location = new System.Drawing.Point(280, 104);
            this.ButtonChange.Name = "ButtonChange";
            this.ButtonChange.Size = new System.Drawing.Size(75, 23);
            this.ButtonChange.TabIndex = 8;
            this.ButtonChange.Text = "更改(&E)";
            this.ButtonChange.UseVisualStyleBackColor = true;
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Location = new System.Drawing.Point(199, 104);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(75, 23);
            this.ButtonAdd.TabIndex = 7;
            this.ButtonAdd.Text = "添加(&A)";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // LabelEnd
            // 
            this.LabelEnd.AutoSize = true;
            this.LabelEnd.Location = new System.Drawing.Point(3, 81);
            this.LabelEnd.Name = "LabelEnd";
            this.LabelEnd.Size = new System.Drawing.Size(104, 15);
            this.LabelEnd.TabIndex = 5;
            this.LabelEnd.Text = "结束日期和时间: ";
            // 
            // DTPEnd
            // 
            this.DTPEnd.CustomFormat = "yyyy-MM-dd dddd HH:mm:ss";
            this.DTPEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTPEnd.Location = new System.Drawing.Point(109, 77);
            this.DTPEnd.Name = "DTPEnd";
            this.DTPEnd.Size = new System.Drawing.Size(246, 23);
            this.DTPEnd.TabIndex = 4;
            // 
            // LabelStart
            // 
            this.LabelStart.AutoSize = true;
            this.LabelStart.Location = new System.Drawing.Point(3, 54);
            this.LabelStart.Name = "LabelStart";
            this.LabelStart.Size = new System.Drawing.Size(104, 15);
            this.LabelStart.TabIndex = 3;
            this.LabelStart.Text = "开始日期和时间: ";
            // 
            // DTPStart
            // 
            this.DTPStart.CustomFormat = "yyyy-MM-dd dddd HH:mm:ss";
            this.DTPStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTPStart.Location = new System.Drawing.Point(109, 50);
            this.DTPStart.Name = "DTPStart";
            this.DTPStart.Size = new System.Drawing.Size(246, 23);
            this.DTPStart.TabIndex = 2;
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Location = new System.Drawing.Point(3, 25);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(65, 15);
            this.LabelName.TabIndex = 1;
            this.LabelName.Text = "考试名称: ";
            // 
            // TextBoxName
            // 
            this.TextBoxName.Location = new System.Drawing.Point(69, 21);
            this.TextBoxName.Name = "TextBoxName";
            this.TextBoxName.Size = new System.Drawing.Size(286, 23);
            this.TextBoxName.TabIndex = 0;
            // 
            // ButtonA
            // 
            this.ButtonA.Location = new System.Drawing.Point(376, 276);
            this.ButtonA.Name = "ButtonA";
            this.ButtonA.Size = new System.Drawing.Size(75, 23);
            this.ButtonA.TabIndex = 4;
            this.ButtonA.Text = "保存(&S)";
            this.ButtonA.UseVisualStyleBackColor = true;
            // 
            // ButtonB
            // 
            this.ButtonB.Location = new System.Drawing.Point(457, 276);
            this.ButtonB.Name = "ButtonB";
            this.ButtonB.Size = new System.Drawing.Size(75, 23);
            this.ButtonB.TabIndex = 5;
            this.ButtonB.Text = "取消(&C)";
            this.ButtonB.UseVisualStyleBackColor = true;
            // 
            // GroupBoxAutoSwitch
            // 
            this.GroupBoxAutoSwitch.Controls.Add(this.ComboBoxSwitchPeriod);
            this.GroupBoxAutoSwitch.Controls.Add(this.LabelSwitchPeriod);
            this.GroupBoxAutoSwitch.Controls.Add(this.CheckBoxAutoSwitch);
            this.GroupBoxAutoSwitch.Location = new System.Drawing.Point(376, 172);
            this.GroupBoxAutoSwitch.Name = "GroupBoxAutoSwitch";
            this.GroupBoxAutoSwitch.Size = new System.Drawing.Size(156, 80);
            this.GroupBoxAutoSwitch.TabIndex = 6;
            this.GroupBoxAutoSwitch.TabStop = false;
            this.GroupBoxAutoSwitch.Text = "自动切换";
            // 
            // ComboBoxSwitchPeriod
            // 
            this.ComboBoxSwitchPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxSwitchPeriod.Enabled = false;
            this.ComboBoxSwitchPeriod.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxSwitchPeriod.Location = new System.Drawing.Point(64, 47);
            this.ComboBoxSwitchPeriod.Name = "ComboBoxSwitchPeriod";
            this.ComboBoxSwitchPeriod.Size = new System.Drawing.Size(86, 23);
            this.ComboBoxSwitchPeriod.TabIndex = 8;
            // 
            // LabelSwitchPeriod
            // 
            this.LabelSwitchPeriod.AutoSize = true;
            this.LabelSwitchPeriod.Location = new System.Drawing.Point(3, 51);
            this.LabelSwitchPeriod.Name = "LabelSwitchPeriod";
            this.LabelSwitchPeriod.Size = new System.Drawing.Size(59, 15);
            this.LabelSwitchPeriod.TabIndex = 1;
            this.LabelSwitchPeriod.Text = "切换间隔";
            // 
            // CheckBoxAutoSwitch
            // 
            this.CheckBoxAutoSwitch.AutoSize = true;
            this.CheckBoxAutoSwitch.Location = new System.Drawing.Point(6, 24);
            this.CheckBoxAutoSwitch.Name = "CheckBoxAutoSwitch";
            this.CheckBoxAutoSwitch.Size = new System.Drawing.Size(104, 19);
            this.CheckBoxAutoSwitch.TabIndex = 0;
            this.CheckBoxAutoSwitch.Text = "启用自动切换";
            this.CheckBoxAutoSwitch.UseVisualStyleBackColor = true;
            this.CheckBoxAutoSwitch.CheckedChanged += new System.EventHandler(this.CheckBoxAutoSwitch_CheckedChanged);
            // 
            // ExamInfoManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(540, 308);
            this.Controls.Add(this.GroupBoxAutoSwitch);
            this.Controls.Add(this.ButtonB);
            this.Controls.Add(this.ButtonA);
            this.Controls.Add(this.GroupBoxEdit);
            this.Controls.Add(this.PanelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ExamInfoManager";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "管理考试信息 - 高考倒计时";
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            this.GroupBoxEdit.ResumeLayout(false);
            this.GroupBoxEdit.PerformLayout();
            this.GroupBoxAutoSwitch.ResumeLayout(false);
            this.GroupBoxAutoSwitch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ListViewEx ListViewMain;
        private System.Windows.Forms.ColumnHeader ColumnHeadExamName;
        private System.Windows.Forms.ColumnHeader ColumnHeadStart;
        private System.Windows.Forms.ColumnHeader ColumnHeadEnd;
        private System.Windows.Forms.Label LabelInfo;
        private System.Windows.Forms.GroupBox GroupBoxEdit;
        private System.Windows.Forms.Label LabelEnd;
        private System.Windows.Forms.DateTimePicker DTPEnd;
        private System.Windows.Forms.Label LabelStart;
        private System.Windows.Forms.DateTimePicker DTPStart;
        private System.Windows.Forms.Label LabelName;
        private System.Windows.Forms.TextBox TextBoxName;
        private System.Windows.Forms.GroupBox GroupBoxAutoSwitch;
        private System.Windows.Forms.Label LabelSwitchPeriod;
        private System.Windows.Forms.CheckBox CheckBoxAutoSwitch;
        private System.Windows.Forms.ComboBox ComboBoxSwitchPeriod;
        private System.Windows.Forms.Button ButtonAdd;
        private System.Windows.Forms.Button ButtonChange;
        //private System.Windows.Forms.Panel PanelMain;
        //private System.Windows.Forms.Button ButtonA;
        //private System.Windows.Forms.Button ButtonB;
    }
}