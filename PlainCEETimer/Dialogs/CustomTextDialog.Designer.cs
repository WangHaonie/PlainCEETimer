namespace PlainCEETimer.Dialogs
{
    partial class CustomTextDialog
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
            this.LabelP3 = new System.Windows.Forms.Label();
            this.TextBoxP3 = new PlainCEETimer.Controls.PlainTextBox();
            this.LabelP2 = new System.Windows.Forms.Label();
            this.TextBoxP2 = new PlainCEETimer.Controls.PlainTextBox();
            this.LabelP1 = new System.Windows.Forms.Label();
            this.TextBoxP1 = new PlainCEETimer.Controls.PlainTextBox();
            this.LabelInfo = new System.Windows.Forms.Label();
            //this.PanelMain = new System.Windows.Forms.Panel();
            //this.ButtonA = new PlainCEETimer.Controls.PlainButton();
            //this.ButtonB = new PlainCEETimer.Controls.PlainButton();
            this.ButtonReset = new PlainCEETimer.Controls.PlainButton();
            this.PanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelP3
            // 
            this.LabelP3.AutoSize = true;
            this.LabelP3.Location = new System.Drawing.Point(3, 107);
            this.LabelP3.Name = "LabelP3";
            this.LabelP3.Size = new System.Drawing.Size(72, 15);
            this.LabelP3.TabIndex = 9;
            this.LabelP3.Text = "考试已结束";
            // 
            // TextBoxP3
            // 
            this.TextBoxP3.Location = new System.Drawing.Point(76, 103);
            this.TextBoxP3.Name = "TextBoxP3";
            this.TextBoxP3.Size = new System.Drawing.Size(304, 23);
            this.TextBoxP3.TabIndex = 8;
            // 
            // LabelP2
            // 
            this.LabelP2.AutoSize = true;
            this.LabelP2.Location = new System.Drawing.Point(3, 81);
            this.LabelP2.Name = "LabelP2";
            this.LabelP2.Size = new System.Drawing.Size(72, 15);
            this.LabelP2.TabIndex = 7;
            this.LabelP2.Text = "考试已开始";
            // 
            // TextBoxP2
            // 
            this.TextBoxP2.Location = new System.Drawing.Point(76, 77);
            this.TextBoxP2.Name = "TextBoxP2";
            this.TextBoxP2.Size = new System.Drawing.Size(304, 23);
            this.TextBoxP2.TabIndex = 6;
            // 
            // LabelP1
            // 
            this.LabelP1.AutoSize = true;
            this.LabelP1.Location = new System.Drawing.Point(3, 55);
            this.LabelP1.Name = "LabelP1";
            this.LabelP1.Size = new System.Drawing.Size(72, 15);
            this.LabelP1.TabIndex = 5;
            this.LabelP1.Text = "考试未开始";
            // 
            // TextBoxP1
            // 
            this.TextBoxP1.Location = new System.Drawing.Point(76, 51);
            this.TextBoxP1.Name = "TextBoxP1";
            this.TextBoxP1.Size = new System.Drawing.Size(304, 23);
            this.TextBoxP1.TabIndex = 4;
            // 
            // LabelInfo
            // 
            this.LabelInfo.AutoSize = true;
            this.LabelInfo.Location = new System.Drawing.Point(3, 3);
            this.LabelInfo.Name = "LabelInfo";
            this.LabelInfo.Size = new System.Drawing.Size(0, 15);
            this.LabelInfo.TabIndex = 0;
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.LabelP3);
            this.PanelMain.Controls.Add(this.TextBoxP3);
            this.PanelMain.Controls.Add(this.LabelP2);
            this.PanelMain.Controls.Add(this.TextBoxP2);
            this.PanelMain.Controls.Add(this.LabelP1);
            this.PanelMain.Controls.Add(this.TextBoxP1);
            this.PanelMain.Controls.Add(this.LabelInfo);
            this.PanelMain.Location = new System.Drawing.Point(0, 0);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(382, 128);
            this.PanelMain.TabIndex = 0;
            // 
            // ButtonA
            // 
            this.ButtonA.Enabled = false;
            this.ButtonA.Location = new System.Drawing.Point(227, 128);
            this.ButtonA.Name = "ButtonA";
            this.ButtonA.Size = new System.Drawing.Size(75, 23);
            this.ButtonA.TabIndex = 2;
            this.ButtonA.Text = "确定(O)";
            this.ButtonA.UseVisualStyleBackColor = true;
            // 
            // ButtonB
            // 
            this.ButtonB.Location = new System.Drawing.Point(308, 128);
            this.ButtonB.Name = "ButtonB";
            this.ButtonB.Size = new System.Drawing.Size(75, 23);
            this.ButtonB.TabIndex = 3;
            this.ButtonB.Text = "取消(&C)";
            this.ButtonB.UseVisualStyleBackColor = true;
            // 
            // ButtonReset
            // 
            this.ButtonReset.Location = new System.Drawing.Point(3, 128);
            this.ButtonReset.Name = "ButtonReset";
            this.ButtonReset.Size = new System.Drawing.Size(75, 23);
            this.ButtonReset.TabIndex = 1;
            this.ButtonReset.Text = "重置(R)";
            this.ButtonReset.UseVisualStyleBackColor = true;
            this.ButtonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // CustomTextDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(385, 152);
            this.Controls.Add(this.ButtonB);
            this.Controls.Add(this.ButtonA);
            this.Controls.Add(this.ButtonReset);
            this.Controls.Add(this.PanelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomTextDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "全局自定义文本 - 高考倒计时";
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Panel PanelMain;
        //private PlainCEETimer.Controls.PlainButton ButtonA;
        //private PlainCEETimer.Controls.PlainButton ButtonB;
        private System.Windows.Forms.Label LabelInfo;
        private System.Windows.Forms.Label LabelP3;
        private PlainCEETimer.Controls.PlainTextBox TextBoxP3;
        private System.Windows.Forms.Label LabelP2;
        private PlainCEETimer.Controls.PlainTextBox TextBoxP2;
        private System.Windows.Forms.Label LabelP1;
        private PlainCEETimer.Controls.PlainTextBox TextBoxP1;
        private PlainCEETimer.Controls.PlainButton ButtonReset;
    }
}