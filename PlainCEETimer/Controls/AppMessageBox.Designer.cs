namespace PlainCEETimer.Controls
{
    partial class AppMessageBox
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
            this.PicBoxIcon = new System.Windows.Forms.PictureBox();
            this.LabelMessage = new System.Windows.Forms.Label();
            this.PanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.LabelMessage);
            this.PanelMain.Controls.Add(this.PicBoxIcon);
            this.PanelMain.Location = new System.Drawing.Point(0, 0);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(168, 40);
            this.PanelMain.TabIndex = 0;
            // 
            // PicBoxIcon
            // 
            this.PicBoxIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PicBoxIcon.Location = new System.Drawing.Point(10, 3);
            this.PicBoxIcon.Name = "PicBoxIcon";
            this.PicBoxIcon.Size = new System.Drawing.Size(32, 32);
            this.PicBoxIcon.TabIndex = 0;
            this.PicBoxIcon.TabStop = false;
            // 
            // LabelMessage
            // 
            this.LabelMessage.AutoSize = true;
            this.LabelMessage.Location = new System.Drawing.Point(48, 3);
            this.LabelMessage.Name = "LabelMessage";
            this.LabelMessage.Size = new System.Drawing.Size(0, 15);
            this.LabelMessage.TabIndex = 1;
            this.LabelMessage.Text = "Message";
            // 
            // ButtonA
            // 
            this.ButtonA.Location = new System.Drawing.Point(12, 41);
            this.ButtonA.Name = "ButtonA";
            this.ButtonA.Size = new System.Drawing.Size(75, 23);
            this.ButtonA.TabIndex = 1;
            this.ButtonA.UseVisualStyleBackColor = true;
            // 
            // ButtonB
            // 
            this.ButtonB.Location = new System.Drawing.Point(93, 41);
            this.ButtonB.Name = "ButtonB";
            this.ButtonB.Size = new System.Drawing.Size(75, 23);
            this.ButtonB.TabIndex = 2;
            this.ButtonB.UseVisualStyleBackColor = true;
            // 
            // AppMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(168, 66);
            this.Controls.Add(this.ButtonB);
            this.Controls.Add(this.ButtonA);
            this.Controls.Add(this.PanelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppMessageBox";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AppMessageBox";
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PicBoxIcon;
        private System.Windows.Forms.Label LabelMessage;
        //private System.Windows.Forms.Panel PanelMain;
        //private System.Windows.Forms.Button ButtonA;
        //private System.Windows.Forms.Button ButtonB;
    }
}