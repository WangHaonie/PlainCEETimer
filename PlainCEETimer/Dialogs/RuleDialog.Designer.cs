namespace PlainCEETimer.Dialogs
{
    partial class RuleDialog
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
            this.ComboBoxRuleType = new PlainCEETimer.Controls.ComboBoxEx();
            this.LinkResetColor = new PlainCEETimer.Controls.PlainLinkLabel();
            this.LinkResetText = new PlainCEETimer.Controls.PlainLinkLabel();
            this.TextBoxCustomText = new PlainCEETimer.Controls.PlainTextBox();
            this.LabelCustomText = new System.Windows.Forms.Label();
            this.LabelColorPreview = new System.Windows.Forms.Label();
            this.LabelBack = new System.Windows.Forms.Label();
            this.LabelChar7 = new System.Windows.Forms.Label();
            this.LabelFore = new System.Windows.Forms.Label();
            this.LabelChar6 = new System.Windows.Forms.Label();
            this.NUDSeconds = new PlainCEETimer.Controls.PlainNumericUpDown();
            this.NUDMinutes = new PlainCEETimer.Controls.PlainNumericUpDown();
            this.NUDHours = new PlainCEETimer.Controls.PlainNumericUpDown();
            this.LabelChar5 = new System.Windows.Forms.Label();
            this.LabelChar4 = new System.Windows.Forms.Label();
            this.LabelChar3 = new System.Windows.Forms.Label();
            this.NUDDays = new PlainCEETimer.Controls.PlainNumericUpDown();
            this.LabelChar2 = new System.Windows.Forms.Label();
            this.LabelChar1 = new System.Windows.Forms.Label();
            //this.PanelMain = new System.Windows.Forms.Panel();
            //this.ButtonB = new PlainCEETimer.Controls.PlainButton();
            //this.ButtonA = new PlainCEETimer.Controls.PlainButton();
            this.PanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDDays)).BeginInit();
            this.SuspendLayout();
            // 
            // ComboBoxRuleType
            // 
            this.ComboBoxRuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxRuleType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxRuleType.Location = new System.Drawing.Point(77, 2);
            this.ComboBoxRuleType.Name = "ComboBoxRuleType";
            this.ComboBoxRuleType.Size = new System.Drawing.Size(82, 23);
            this.ComboBoxRuleType.TabIndex = 41;
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.LinkResetColor);
            this.PanelMain.Controls.Add(this.ComboBoxRuleType);
            this.PanelMain.Controls.Add(this.LinkResetText);
            this.PanelMain.Controls.Add(this.TextBoxCustomText);
            this.PanelMain.Controls.Add(this.LabelCustomText);
            this.PanelMain.Controls.Add(this.LabelColorPreview);
            this.PanelMain.Controls.Add(this.LabelBack);
            this.PanelMain.Controls.Add(this.LabelChar7);
            this.PanelMain.Controls.Add(this.LabelFore);
            this.PanelMain.Controls.Add(this.LabelChar6);
            this.PanelMain.Controls.Add(this.NUDSeconds);
            this.PanelMain.Controls.Add(this.NUDMinutes);
            this.PanelMain.Controls.Add(this.NUDHours);
            this.PanelMain.Controls.Add(this.LabelChar5);
            this.PanelMain.Controls.Add(this.LabelChar4);
            this.PanelMain.Controls.Add(this.LabelChar3);
            this.PanelMain.Controls.Add(this.NUDDays);
            this.PanelMain.Controls.Add(this.LabelChar2);
            this.PanelMain.Controls.Add(this.LabelChar1);
            this.PanelMain.Location = new System.Drawing.Point(0, 0);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(454, 76);
            this.PanelMain.TabIndex = 0;
            // 
            // LinkResetColor
            // 
            this.LinkResetColor.AutoSize = true;
            this.LinkResetColor.Location = new System.Drawing.Point(412, 29);
            this.LinkResetColor.Name = "LinkResetColor";
            this.LinkResetColor.Size = new System.Drawing.Size(33, 15);
            this.LinkResetColor.TabIndex = 42;
            this.LinkResetColor.TabStop = true;
            this.LinkResetColor.Text = "重置";
            this.LinkResetColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkReset_LinkClicked);
            // 
            // LinkResetText
            // 
            this.LinkResetText.AutoSize = true;
            this.LinkResetText.Location = new System.Drawing.Point(412, 53);
            this.LinkResetText.Name = "LinkResetText";
            this.LinkResetText.Size = new System.Drawing.Size(33, 15);
            this.LinkResetText.TabIndex = 38;
            this.LinkResetText.TabStop = true;
            this.LinkResetText.Text = "重置";
            this.LinkResetText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkReset_LinkClicked);
            // 
            // TextBoxCustomText
            // 
            this.TextBoxCustomText.Location = new System.Drawing.Point(77, 50);
            this.TextBoxCustomText.Name = "TextBoxCustomText";
            this.TextBoxCustomText.Size = new System.Drawing.Size(333, 23);
            this.TextBoxCustomText.TabIndex = 37;
            // 
            // LabelCustomText
            // 
            this.LabelCustomText.AutoSize = true;
            this.LabelCustomText.Location = new System.Drawing.Point(3, 53);
            this.LabelCustomText.Name = "LabelCustomText";
            this.LabelCustomText.Size = new System.Drawing.Size(72, 15);
            this.LabelCustomText.TabIndex = 36;
            this.LabelCustomText.Text = "自定义文本";
            // 
            // LabelColorPreview
            // 
            this.LabelColorPreview.AutoSize = true;
            this.LabelColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColorPreview.Location = new System.Drawing.Point(323, 29);
            this.LabelColorPreview.Name = "LabelColorPreview";
            this.LabelColorPreview.Size = new System.Drawing.Size(87, 17);
            this.LabelColorPreview.TabIndex = 35;
            this.LabelColorPreview.Text = "颜色效果预览";
            // 
            // LabelBack
            // 
            this.LabelBack.AutoSize = true;
            this.LabelBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelBack.Location = new System.Drawing.Point(198, 29);
            this.LabelBack.Name = "LabelBack";
            this.LabelBack.Size = new System.Drawing.Size(39, 17);
            this.LabelBack.TabIndex = 34;
            this.LabelBack.Text = "          ";
            // 
            // LabelChar7
            // 
            this.LabelChar7.AutoSize = true;
            this.LabelChar7.Location = new System.Drawing.Point(118, 29);
            this.LabelChar7.Name = "LabelChar7";
            this.LabelChar7.Size = new System.Drawing.Size(78, 15);
            this.LabelChar7.TabIndex = 33;
            this.LabelChar7.Text = ", 背景颜色为";
            // 
            // LabelFore
            // 
            this.LabelFore.AutoSize = true;
            this.LabelFore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelFore.Location = new System.Drawing.Point(77, 29);
            this.LabelFore.Name = "LabelFore";
            this.LabelFore.Size = new System.Drawing.Size(39, 17);
            this.LabelFore.TabIndex = 32;
            this.LabelFore.Text = "          ";
            // 
            // LabelChar6
            // 
            this.LabelChar6.AutoSize = true;
            this.LabelChar6.Location = new System.Drawing.Point(3, 29);
            this.LabelChar6.Name = "LabelChar6";
            this.LabelChar6.Size = new System.Drawing.Size(72, 15);
            this.LabelChar6.TabIndex = 31;
            this.LabelChar6.Text = "文字颜色为";
            // 
            // NUDSeconds
            // 
            this.NUDSeconds.Location = new System.Drawing.Point(370, 2);
            this.NUDSeconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUDSeconds.Name = "NUDSeconds";
            this.NUDSeconds.Size = new System.Drawing.Size(40, 23);
            this.NUDSeconds.TabIndex = 30;
            this.NUDSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUDSeconds.TextChanged += new System.EventHandler((_, _) => this.UserChanged());
            // 
            // NUDMinutes
            // 
            this.NUDMinutes.Location = new System.Drawing.Point(306, 2);
            this.NUDMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUDMinutes.Name = "NUDMinutes";
            this.NUDMinutes.Size = new System.Drawing.Size(40, 23);
            this.NUDMinutes.TabIndex = 29;
            this.NUDMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUDMinutes.TextChanged += new System.EventHandler((_, _) => this.UserChanged());
            // 
            // NUDHours
            // 
            this.NUDHours.Location = new System.Drawing.Point(242, 2);
            this.NUDHours.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.NUDHours.Name = "NUDHours";
            this.NUDHours.Size = new System.Drawing.Size(40, 23);
            this.NUDHours.TabIndex = 28;
            this.NUDHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUDHours.TextChanged += new System.EventHandler((_, _) => this.UserChanged());
            // 
            // LabelChar5
            // 
            this.LabelChar5.AutoSize = true;
            this.LabelChar5.Location = new System.Drawing.Point(412, 6);
            this.LabelChar5.Name = "LabelChar5";
            this.LabelChar5.Size = new System.Drawing.Size(39, 15);
            this.LabelChar5.TabIndex = 27;
            this.LabelChar5.Text = "秒 时,";
            // 
            // LabelChar4
            // 
            this.LabelChar4.AutoSize = true;
            this.LabelChar4.Location = new System.Drawing.Point(348, 6);
            this.LabelChar4.Name = "LabelChar4";
            this.LabelChar4.Size = new System.Drawing.Size(20, 15);
            this.LabelChar4.TabIndex = 26;
            this.LabelChar4.Text = "分";
            // 
            // LabelChar3
            // 
            this.LabelChar3.AutoSize = true;
            this.LabelChar3.Location = new System.Drawing.Point(284, 6);
            this.LabelChar3.Name = "LabelChar3";
            this.LabelChar3.Size = new System.Drawing.Size(20, 15);
            this.LabelChar3.TabIndex = 25;
            this.LabelChar3.Text = "时";
            // 
            // NUDDays
            // 
            this.NUDDays.Location = new System.Drawing.Point(165, 2);
            this.NUDDays.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NUDDays.Name = "NUDDays";
            this.NUDDays.Size = new System.Drawing.Size(53, 23);
            this.NUDDays.TabIndex = 24;
            this.NUDDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUDDays.TextChanged += new System.EventHandler((_, _) => this.UserChanged());
            // 
            // LabelChar2
            // 
            this.LabelChar2.AutoSize = true;
            this.LabelChar2.Location = new System.Drawing.Point(220, 6);
            this.LabelChar2.Name = "LabelChar2";
            this.LabelChar2.Size = new System.Drawing.Size(20, 15);
            this.LabelChar2.TabIndex = 23;
            this.LabelChar2.Text = "天";
            // 
            // LabelChar1
            // 
            this.LabelChar1.AutoSize = true;
            this.LabelChar1.Location = new System.Drawing.Point(3, 6);
            this.LabelChar1.Name = "LabelChar1";
            this.LabelChar1.Size = new System.Drawing.Size(72, 15);
            this.LabelChar1.TabIndex = 22;
            this.LabelChar1.Text = "当距离考试";
            // 
            // ButtonB
            // 
            this.ButtonB.Location = new System.Drawing.Point(379, 77);
            this.ButtonB.Name = "ButtonB";
            this.ButtonB.Size = new System.Drawing.Size(75, 23);
            this.ButtonB.TabIndex = 40;
            this.ButtonB.Text = "取消(&C)";
            this.ButtonB.UseVisualStyleBackColor = true;
            // 
            // ButtonA
            // 
            this.ButtonA.Enabled = false;
            this.ButtonA.Location = new System.Drawing.Point(298, 77);
            this.ButtonA.Name = "ButtonA";
            this.ButtonA.Size = new System.Drawing.Size(75, 23);
            this.ButtonA.TabIndex = 39;
            this.ButtonA.Text = "确定(&O)";
            this.ButtonA.UseVisualStyleBackColor = true;
            // 
            // RuleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(462, 102);
            this.Controls.Add(this.PanelMain);
            this.Controls.Add(this.ButtonB);
            this.Controls.Add(this.ButtonA);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RuleDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "自定义规则 - 高考倒计时";
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDDays)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PlainCEETimer.Controls.PlainLinkLabel LinkResetColor;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxRuleType;
        private PlainCEETimer.Controls.PlainLinkLabel LinkResetText;
        private PlainCEETimer.Controls.PlainTextBox TextBoxCustomText;
        private System.Windows.Forms.Label LabelCustomText;
        private System.Windows.Forms.Label LabelColorPreview;
        private System.Windows.Forms.Label LabelBack;
        private System.Windows.Forms.Label LabelChar7;
        private System.Windows.Forms.Label LabelFore;
        private System.Windows.Forms.Label LabelChar6;
        private PlainCEETimer.Controls.PlainNumericUpDown NUDSeconds;
        private PlainCEETimer.Controls.PlainNumericUpDown NUDMinutes;
        private PlainCEETimer.Controls.PlainNumericUpDown NUDHours;
        private System.Windows.Forms.Label LabelChar5;
        private System.Windows.Forms.Label LabelChar4;
        private System.Windows.Forms.Label LabelChar3;
        private PlainCEETimer.Controls.PlainNumericUpDown NUDDays;
        private System.Windows.Forms.Label LabelChar2;
        private System.Windows.Forms.Label LabelChar1;
        //private System.Windows.Forms.Panel PanelMain;
        //private PlainCEETimer.Controls.PlainButton ButtonB;
        //private PlainCEETimer.Controls.PlainButton ButtonA;
    }
}