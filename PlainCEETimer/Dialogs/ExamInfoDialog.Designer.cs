namespace PlainCEETimer.Dialogs
{
    partial class ExamInfoDialog
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
            this.LabelCounter = new System.Windows.Forms.Label();
            this.LabelEnd = new System.Windows.Forms.Label();
            this.DTPEnd = new System.Windows.Forms.DateTimePicker();
            this.LabelStart = new System.Windows.Forms.Label();
            this.DTPStart = new System.Windows.Forms.DateTimePicker();
            this.LabelName = new System.Windows.Forms.Label();
            this.TextBoxName = new PlainCEETimer.Controls.PlainTextBox();
            //this.ButtonB = new PlainCEETimer.Controls.PlainButton();
            //this.ButtonA = new PlainCEETimer.Controls.PlainButton();
            //this.PanelMain = new System.Windows.Forms.Panel();
            this.PanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelCounter
            // 
            this.LabelCounter.AutoSize = true;
            this.LabelCounter.Location = new System.Drawing.Point(319, 6);
            this.LabelCounter.Name = "LabelCounter";
            this.LabelCounter.Size = new System.Drawing.Size(0, 15);
            this.LabelCounter.TabIndex = 15;
            // 
            // LabelEnd
            // 
            this.LabelEnd.AutoSize = true;
            this.LabelEnd.Location = new System.Drawing.Point(3, 62);
            this.LabelEnd.Name = "LabelEnd";
            this.LabelEnd.Size = new System.Drawing.Size(104, 15);
            this.LabelEnd.TabIndex = 21;
            this.LabelEnd.Text = "结束日期和时间: ";
            // 
            // DTPEnd
            // 
            this.DTPEnd.CustomFormat = "yyyy-MM-dd dddd HH:mm:ss";
            this.DTPEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTPEnd.Location = new System.Drawing.Point(109, 58);
            this.DTPEnd.Name = "DTPEnd";
            this.DTPEnd.Size = new System.Drawing.Size(246, 23);
            this.DTPEnd.TabIndex = 20;
            this.DTPEnd.ValueChanged += new System.EventHandler(DTP_ValueChanged);
            // 
            // LabelStart
            // 
            this.LabelStart.AutoSize = true;
            this.LabelStart.Location = new System.Drawing.Point(3, 35);
            this.LabelStart.Name = "LabelStart";
            this.LabelStart.Size = new System.Drawing.Size(104, 15);
            this.LabelStart.TabIndex = 19;
            this.LabelStart.Text = "开始日期和时间: ";
            // 
            // DTPStart
            // 
            this.DTPStart.CustomFormat = "yyyy-MM-dd dddd HH:mm:ss";
            this.DTPStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTPStart.Location = new System.Drawing.Point(109, 31);
            this.DTPStart.Name = "DTPStart";
            this.DTPStart.Size = new System.Drawing.Size(246, 23);
            this.DTPStart.TabIndex = 18;
            this.DTPStart.ValueChanged += new System.EventHandler(DTP_ValueChanged);
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Location = new System.Drawing.Point(3, 6);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(65, 15);
            this.LabelName.TabIndex = 17;
            this.LabelName.Text = "考试名称: ";
            // 
            // TextBoxName
            // 
            this.TextBoxName.Location = new System.Drawing.Point(69, 2);
            this.TextBoxName.Name = "TextBoxName";
            this.TextBoxName.Size = new System.Drawing.Size(248, 23);
            this.TextBoxName.TabIndex = 16;
            this.TextBoxName.TextChanged += new System.EventHandler(this.TextBoxName_TextChanged);
            // 
            // ButtonB
            // 
            this.ButtonB.Location = new System.Drawing.Point(283, 90);
            this.ButtonB.Name = "ButtonB";
            this.ButtonB.Size = new System.Drawing.Size(75, 23);
            this.ButtonB.TabIndex = 23;
            this.ButtonB.Text = "取消(&C)";
            this.ButtonB.UseVisualStyleBackColor = true;
            // 
            // ButtonA
            // 
            this.ButtonA.Enabled = false;
            this.ButtonA.Location = new System.Drawing.Point(202, 90);
            this.ButtonA.Name = "ButtonA";
            this.ButtonA.Size = new System.Drawing.Size(75, 23);
            this.ButtonA.TabIndex = 22;
            this.ButtonA.Text = "确定(&O)";
            this.ButtonA.UseVisualStyleBackColor = true;
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.LabelName);
            this.PanelMain.Controls.Add(this.TextBoxName);
            this.PanelMain.Controls.Add(this.DTPStart);
            this.PanelMain.Controls.Add(this.LabelCounter);
            this.PanelMain.Controls.Add(this.LabelEnd);
            this.PanelMain.Controls.Add(this.LabelStart);
            this.PanelMain.Controls.Add(this.DTPEnd);
            this.PanelMain.Location = new System.Drawing.Point(3, 3);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(362, 87);
            this.PanelMain.TabIndex = 24;
            // 
            // ExamInfoDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(367, 114);
            this.Controls.Add(this.PanelMain);
            this.Controls.Add(this.ButtonB);
            this.Controls.Add(this.ButtonA);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExamInfoDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "考试信息 - 高考倒计时";
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelCounter;
        //private PlainCEETimer.Controls.PlainButton ButtonB;
        //private PlainCEETimer.Controls.PlainButton ButtonA;
        //private System.Windows.Forms.Panel PanelMain;
        private System.Windows.Forms.Label LabelEnd;
        private System.Windows.Forms.DateTimePicker DTPEnd;
        private System.Windows.Forms.Label LabelStart;
        private System.Windows.Forms.DateTimePicker DTPStart;
        private System.Windows.Forms.Label LabelName;
        private PlainCEETimer.Controls.PlainTextBox TextBoxName;
    }
}