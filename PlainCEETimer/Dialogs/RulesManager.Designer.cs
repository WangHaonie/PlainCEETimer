namespace PlainCEETimer.Dialogs
{
    partial class RulesManager
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
            //this.ButtonB = new System.Windows.Forms.Button();
            //this.ButtonA = new System.Windows.Forms.Button();
            this.ListViewMain = new PlainCEETimer.Controls.ListViewEx();
            this.ColumnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderTick = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderFore = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderBack = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GroupBoxDetails = new System.Windows.Forms.GroupBox();
            this.ComboBoxRuleType = new System.Windows.Forms.ComboBox();
            this.ButtonChange = new System.Windows.Forms.Button();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.LinkReset = new System.Windows.Forms.LinkLabel();
            this.TextBoxCustomText = new System.Windows.Forms.TextBox();
            this.LabelCustomText = new System.Windows.Forms.Label();
            this.LabelColorPreview = new System.Windows.Forms.Label();
            this.LabelBack = new System.Windows.Forms.Label();
            this.LabelChar7 = new System.Windows.Forms.Label();
            this.LabelFore = new System.Windows.Forms.Label();
            this.LabelChar6 = new System.Windows.Forms.Label();
            this.NUDSeconds = new System.Windows.Forms.NumericUpDown();
            this.NUDMinutes = new System.Windows.Forms.NumericUpDown();
            this.NUDHours = new System.Windows.Forms.NumericUpDown();
            this.LabelChar5 = new System.Windows.Forms.Label();
            this.LabelChar4 = new System.Windows.Forms.Label();
            this.LabelChar3 = new System.Windows.Forms.Label();
            this.NUDDays = new System.Windows.Forms.NumericUpDown();
            this.LabelChar2 = new System.Windows.Forms.Label();
            this.LabelChar1 = new System.Windows.Forms.Label();
            this.GroupBoxWarning = new System.Windows.Forms.GroupBox();
            this.ButtonCustomText = new System.Windows.Forms.Button();
            this.LabelWarning = new System.Windows.Forms.Label();
            this.PanelMain.SuspendLayout();
            this.GroupBoxDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDDays)).BeginInit();
            this.GroupBoxWarning.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.ListViewMain);
            this.PanelMain.Location = new System.Drawing.Point(0, 0);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(637, 163);
            this.PanelMain.TabIndex = 0;
            // 
            // ListViewMain
            // 
            this.ListViewMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderType,
            this.ColumnHeaderTick,
            this.ColumnHeaderFore,
            this.ColumnHeaderBack,
            this.ColumnHeaderText});
            this.ListViewMain.FullRowSelect = true;
            this.ListViewMain.GridLines = true;
            this.ListViewMain.HideSelection = false;
            this.ListViewMain.Location = new System.Drawing.Point(6, 3);
            this.ListViewMain.Name = "ListViewMain";
            this.ListViewMain.Size = new System.Drawing.Size(626, 154);
            this.ListViewMain.TabIndex = 1;
            this.ListViewMain.UseCompatibleStateImageBehavior = false;
            this.ListViewMain.View = System.Windows.Forms.View.Details;
            // 
            // ColumnHeaderType
            // 
            this.ColumnHeaderType.Text = "类别";
            this.ColumnHeaderType.Width = 70;
            // 
            // ColumnHeaderTick
            // 
            this.ColumnHeaderTick.Text = "时刻";
            this.ColumnHeaderTick.Width = 135;
            // 
            // ColumnHeaderFore
            // 
            this.ColumnHeaderFore.Text = "文字颜色";
            this.ColumnHeaderFore.Width = 80;
            // 
            // ColumnHeaderBack
            // 
            this.ColumnHeaderBack.Text = "背景颜色";
            this.ColumnHeaderBack.Width = 80;
            // 
            // ColumnHeaderText
            // 
            this.ColumnHeaderText.Text = "自定义文本";
            this.ColumnHeaderText.Width = 250;
            // 
            // ButtonB
            // 
            this.ButtonB.Location = new System.Drawing.Point(557, 251);
            this.ButtonB.Name = "ButtonB";
            this.ButtonB.Size = new System.Drawing.Size(75, 23);
            this.ButtonB.TabIndex = 22;
            this.ButtonB.Text = "取消(&C)";
            this.ButtonB.UseVisualStyleBackColor = true;
            // 
            // ButtonA
            // 
            this.ButtonA.Enabled = false;
            this.ButtonA.Location = new System.Drawing.Point(476, 251);
            this.ButtonA.Name = "ButtonA";
            this.ButtonA.Size = new System.Drawing.Size(75, 23);
            this.ButtonA.TabIndex = 21;
            this.ButtonA.Text = "保存(&S)";
            this.ButtonA.UseVisualStyleBackColor = true;
            // 
            // GroupBoxDetails
            // 
            this.GroupBoxDetails.Controls.Add(this.ComboBoxRuleType);
            this.GroupBoxDetails.Controls.Add(this.ButtonChange);
            this.GroupBoxDetails.Controls.Add(this.ButtonAdd);
            this.GroupBoxDetails.Controls.Add(this.LinkReset);
            this.GroupBoxDetails.Controls.Add(this.TextBoxCustomText);
            this.GroupBoxDetails.Controls.Add(this.LabelCustomText);
            this.GroupBoxDetails.Controls.Add(this.LabelColorPreview);
            this.GroupBoxDetails.Controls.Add(this.LabelBack);
            this.GroupBoxDetails.Controls.Add(this.LabelChar7);
            this.GroupBoxDetails.Controls.Add(this.LabelFore);
            this.GroupBoxDetails.Controls.Add(this.LabelChar6);
            this.GroupBoxDetails.Controls.Add(this.NUDSeconds);
            this.GroupBoxDetails.Controls.Add(this.NUDMinutes);
            this.GroupBoxDetails.Controls.Add(this.NUDHours);
            this.GroupBoxDetails.Controls.Add(this.LabelChar5);
            this.GroupBoxDetails.Controls.Add(this.LabelChar4);
            this.GroupBoxDetails.Controls.Add(this.LabelChar3);
            this.GroupBoxDetails.Controls.Add(this.NUDDays);
            this.GroupBoxDetails.Controls.Add(this.LabelChar2);
            this.GroupBoxDetails.Controls.Add(this.LabelChar1);
            this.GroupBoxDetails.Location = new System.Drawing.Point(6, 164);
            this.GroupBoxDetails.Name = "GroupBoxDetails";
            this.GroupBoxDetails.Size = new System.Drawing.Size(456, 116);
            this.GroupBoxDetails.TabIndex = 1;
            this.GroupBoxDetails.TabStop = false;
            this.GroupBoxDetails.Text = "规则详情";
            // 
            // ComboBoxRuleType
            // 
            this.ComboBoxRuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxRuleType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxRuleType.Location = new System.Drawing.Point(77, 15);
            this.ComboBoxRuleType.Name = "ComboBoxRuleType";
            this.ComboBoxRuleType.Size = new System.Drawing.Size(82, 23);
            this.ComboBoxRuleType.TabIndex = 21;
            // 
            // ButtonChange
            // 
            this.ButtonChange.Enabled = false;
            this.ButtonChange.Location = new System.Drawing.Point(375, 87);
            this.ButtonChange.Name = "ButtonChange";
            this.ButtonChange.Size = new System.Drawing.Size(75, 23);
            this.ButtonChange.TabIndex = 20;
            this.ButtonChange.Text = "更改(&E)";
            this.ButtonChange.UseVisualStyleBackColor = true;
            this.ButtonChange.Click += new System.EventHandler(this.ButtonChange_Click);
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Location = new System.Drawing.Point(294, 87);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(75, 23);
            this.ButtonAdd.TabIndex = 19;
            this.ButtonAdd.Text = "添加(&A)";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // LinkReset
            // 
            this.LinkReset.AutoSize = true;
            this.LinkReset.Location = new System.Drawing.Point(412, 65);
            this.LinkReset.Name = "LinkReset";
            this.LinkReset.Size = new System.Drawing.Size(33, 15);
            this.LinkReset.TabIndex = 18;
            this.LinkReset.TabStop = true;
            this.LinkReset.Text = "重置";
            // 
            // TextBoxCustomText
            // 
            this.TextBoxCustomText.Location = new System.Drawing.Point(77, 60);
            this.TextBoxCustomText.Name = "TextBoxCustomText";
            this.TextBoxCustomText.Size = new System.Drawing.Size(333, 23);
            this.TextBoxCustomText.TabIndex = 17;
            // 
            // LabelCustomText
            // 
            this.LabelCustomText.AutoSize = true;
            this.LabelCustomText.Location = new System.Drawing.Point(3, 65);
            this.LabelCustomText.Name = "LabelCustomText";
            this.LabelCustomText.Size = new System.Drawing.Size(72, 15);
            this.LabelCustomText.TabIndex = 16;
            this.LabelCustomText.Text = "自定义文本";
            // 
            // LabelColorPreview
            // 
            this.LabelColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColorPreview.Location = new System.Drawing.Point(361, 41);
            this.LabelColorPreview.Name = "LabelColorPreview";
            this.LabelColorPreview.Size = new System.Drawing.Size(89, 16);
            this.LabelColorPreview.TabIndex = 15;
            this.LabelColorPreview.Text = "颜色效果预览";
            // 
            // LabelBack
            // 
            this.LabelBack.AutoSize = true;
            this.LabelBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelBack.Location = new System.Drawing.Point(203, 41);
            this.LabelBack.Name = "LabelBack";
            this.LabelBack.Size = new System.Drawing.Size(39, 17);
            this.LabelBack.TabIndex = 14;
            this.LabelBack.Text = "          ";
            // 
            // LabelChar7
            // 
            this.LabelChar7.AutoSize = true;
            this.LabelChar7.Location = new System.Drawing.Point(121, 42);
            this.LabelChar7.Name = "LabelChar7";
            this.LabelChar7.Size = new System.Drawing.Size(78, 15);
            this.LabelChar7.TabIndex = 13;
            this.LabelChar7.Text = ", 背景颜色为";
            // 
            // LabelFore
            // 
            this.LabelFore.AutoSize = true;
            this.LabelFore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelFore.Location = new System.Drawing.Point(77, 41);
            this.LabelFore.Name = "LabelFore";
            this.LabelFore.Size = new System.Drawing.Size(39, 17);
            this.LabelFore.TabIndex = 12;
            this.LabelFore.Text = "          ";
            // 
            // LabelChar6
            // 
            this.LabelChar6.AutoSize = true;
            this.LabelChar6.Location = new System.Drawing.Point(3, 42);
            this.LabelChar6.Name = "LabelChar6";
            this.LabelChar6.Size = new System.Drawing.Size(72, 15);
            this.LabelChar6.TabIndex = 10;
            this.LabelChar6.Text = "文字颜色为";
            // 
            // NUDSeconds
            // 
            this.NUDSeconds.Location = new System.Drawing.Point(370, 15);
            this.NUDSeconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUDSeconds.Name = "NUDSeconds";
            this.NUDSeconds.Size = new System.Drawing.Size(40, 23);
            this.NUDSeconds.TabIndex = 9;
            this.NUDSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUDMinutes
            // 
            this.NUDMinutes.Location = new System.Drawing.Point(306, 15);
            this.NUDMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUDMinutes.Name = "NUDMinutes";
            this.NUDMinutes.Size = new System.Drawing.Size(40, 23);
            this.NUDMinutes.TabIndex = 8;
            this.NUDMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUDHours
            // 
            this.NUDHours.Location = new System.Drawing.Point(242, 15);
            this.NUDHours.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.NUDHours.Name = "NUDHours";
            this.NUDHours.Size = new System.Drawing.Size(40, 23);
            this.NUDHours.TabIndex = 7;
            this.NUDHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LabelChar5
            // 
            this.LabelChar5.AutoSize = true;
            this.LabelChar5.Location = new System.Drawing.Point(412, 19);
            this.LabelChar5.Name = "LabelChar5";
            this.LabelChar5.Size = new System.Drawing.Size(39, 15);
            this.LabelChar5.TabIndex = 6;
            this.LabelChar5.Text = "秒 时,";
            // 
            // LabelChar4
            // 
            this.LabelChar4.AutoSize = true;
            this.LabelChar4.Location = new System.Drawing.Point(348, 19);
            this.LabelChar4.Name = "LabelChar4";
            this.LabelChar4.Size = new System.Drawing.Size(20, 15);
            this.LabelChar4.TabIndex = 5;
            this.LabelChar4.Text = "分";
            // 
            // LabelChar3
            // 
            this.LabelChar3.AutoSize = true;
            this.LabelChar3.Location = new System.Drawing.Point(284, 19);
            this.LabelChar3.Name = "LabelChar3";
            this.LabelChar3.Size = new System.Drawing.Size(20, 15);
            this.LabelChar3.TabIndex = 4;
            this.LabelChar3.Text = "时";
            // 
            // NUDDays
            // 
            this.NUDDays.Location = new System.Drawing.Point(165, 15);
            this.NUDDays.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NUDDays.Name = "NUDDays";
            this.NUDDays.Size = new System.Drawing.Size(53, 23);
            this.NUDDays.TabIndex = 3;
            this.NUDDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LabelChar2
            // 
            this.LabelChar2.AutoSize = true;
            this.LabelChar2.Location = new System.Drawing.Point(220, 19);
            this.LabelChar2.Name = "LabelChar2";
            this.LabelChar2.Size = new System.Drawing.Size(20, 15);
            this.LabelChar2.TabIndex = 2;
            this.LabelChar2.Text = "天";
            // 
            // LabelChar1
            // 
            this.LabelChar1.AutoSize = true;
            this.LabelChar1.Location = new System.Drawing.Point(3, 19);
            this.LabelChar1.Name = "LabelChar1";
            this.LabelChar1.Size = new System.Drawing.Size(72, 15);
            this.LabelChar1.TabIndex = 0;
            this.LabelChar1.Text = "当距离考试";
            // 
            // GroupBoxWarning
            // 
            this.GroupBoxWarning.Controls.Add(this.ButtonCustomText);
            this.GroupBoxWarning.Controls.Add(this.LabelWarning);
            this.GroupBoxWarning.Location = new System.Drawing.Point(468, 164);
            this.GroupBoxWarning.Name = "GroupBoxWarning";
            this.GroupBoxWarning.Size = new System.Drawing.Size(164, 81);
            this.GroupBoxWarning.TabIndex = 2;
            this.GroupBoxWarning.TabStop = false;
            this.GroupBoxWarning.Text = "全局自定义文本";
            // 
            // ButtonCustomText
            // 
            this.ButtonCustomText.AutoSize = true;
            this.ButtonCustomText.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonCustomText.Location = new System.Drawing.Point(73, 50);
            this.ButtonCustomText.Name = "ButtonCustomText";
            this.ButtonCustomText.Size = new System.Drawing.Size(85, 25);
            this.ButtonCustomText.TabIndex = 42;
            this.ButtonCustomText.Text = "全局设置(G)";
            this.ButtonCustomText.UseVisualStyleBackColor = true;
            this.ButtonCustomText.Click += new System.EventHandler(this.ButtonCustomText_Click);
            // 
            // LabelWarning
            // 
            this.LabelWarning.AutoSize = true;
            this.LabelWarning.Location = new System.Drawing.Point(3, 19);
            this.LabelWarning.Name = "LabelWarning";
            this.LabelWarning.Size = new System.Drawing.Size(247, 15);
            this.LabelWarning.TabIndex = 0;
            this.LabelWarning.Text = "若未添加规则, 默认显示全局设置的文本。";
            // 
            // RulesManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(640, 280);
            this.Controls.Add(this.ButtonB);
            this.Controls.Add(this.GroupBoxWarning);
            this.Controls.Add(this.ButtonA);
            this.Controls.Add(this.GroupBoxDetails);
            this.Controls.Add(this.PanelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RulesManager";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "规则管理器 - 高考倒计时";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RulesManager_KeyDown);
            this.PanelMain.ResumeLayout(false);
            this.GroupBoxDetails.ResumeLayout(false);
            this.GroupBoxDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDDays)).EndInit();
            this.GroupBoxWarning.ResumeLayout(false);
            this.GroupBoxWarning.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ListViewEx ListViewMain;
        private System.Windows.Forms.ColumnHeader ColumnHeaderType;
        private System.Windows.Forms.ColumnHeader ColumnHeaderTick;
        private System.Windows.Forms.ColumnHeader ColumnHeaderFore;
        private System.Windows.Forms.ColumnHeader ColumnHeaderBack;
        private System.Windows.Forms.ColumnHeader ColumnHeaderText;
        private System.Windows.Forms.GroupBox GroupBoxDetails;
        private System.Windows.Forms.NumericUpDown NUDDays;
        private System.Windows.Forms.Label LabelChar2;
        private System.Windows.Forms.Label LabelChar1;
        private System.Windows.Forms.Label LabelChar5;
        private System.Windows.Forms.Label LabelChar4;
        private System.Windows.Forms.Label LabelChar3;
        private System.Windows.Forms.NumericUpDown NUDHours;
        private System.Windows.Forms.NumericUpDown NUDMinutes;
        private System.Windows.Forms.NumericUpDown NUDSeconds;
        private System.Windows.Forms.Label LabelChar6;
        private System.Windows.Forms.Label LabelBack;
        private System.Windows.Forms.Label LabelChar7;
        private System.Windows.Forms.Label LabelFore;
        private System.Windows.Forms.LinkLabel LinkReset;
        private System.Windows.Forms.TextBox TextBoxCustomText;
        private System.Windows.Forms.Label LabelCustomText;
        private System.Windows.Forms.Label LabelColorPreview;
        private System.Windows.Forms.GroupBox GroupBoxWarning;
        private System.Windows.Forms.Button ButtonChange;
        private System.Windows.Forms.Button ButtonAdd;
        private System.Windows.Forms.ComboBox ComboBoxRuleType;
        //private System.Windows.Forms.Panel PanelMain;
        //private System.Windows.Forms.Button ButtonB;
        //private System.Windows.Forms.Button ButtonA;
        private System.Windows.Forms.Label LabelWarning;
        private System.Windows.Forms.Button ButtonCustomText;
    }
}