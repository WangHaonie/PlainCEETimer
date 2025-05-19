using PlainCEETimer.Modules;

namespace PlainCEETimer.Forms
{
    partial class AboutForm
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
            this.LabelInfo = new System.Windows.Forms.Label();
            this.ButtonOK = new PlainCEETimer.Controls.PlainButton();
            this.LabelLicense = new System.Windows.Forms.Label();
            this.PanelMain = new System.Windows.Forms.Panel();
            this.LinkTutorial = new PlainCEETimer.Controls.Hyperlink();
            this.LinkFeedback = new PlainCEETimer.Controls.Hyperlink();
            this.PicBoxLogo = new System.Windows.Forms.PictureBox();
            this.LinkGitHub = new PlainCEETimer.Controls.Hyperlink();
            this.PanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelInfo
            // 
            this.LabelInfo.AutoSize = true;
            this.LabelInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelInfo.Location = new System.Drawing.Point(40, 3);
            this.LabelInfo.Name = "LabelInfo";
            this.LabelInfo.Size = new System.Drawing.Size(0, 15);
            this.LabelInfo.TabIndex = 1;
            // 
            // ButtonClose
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonOK.Location = new System.Drawing.Point(130, 66);
            this.ButtonOK.Name = "ButtonClose";
            this.ButtonOK.Size = new System.Drawing.Size(75, 25);
            this.ButtonOK.TabIndex = 5;
            this.ButtonOK.Text = "确定(&O)";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // LabelLicense
            // 
            this.LabelLicense.AutoSize = true;
            this.LabelLicense.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelLicense.Location = new System.Drawing.Point(5, 34);
            this.LabelLicense.Name = "LabelLicense";
            this.LabelLicense.Size = new System.Drawing.Size(0, 15);
            this.LabelLicense.TabIndex = 7;
            // 
            // PanelMain
            // 
            this.PanelMain.AutoSize = true;
            this.PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PanelMain.Controls.Add(this.LinkTutorial);
            this.PanelMain.Controls.Add(this.LinkFeedback);
            this.PanelMain.Controls.Add(this.PicBoxLogo);
            this.PanelMain.Controls.Add(this.LinkGitHub);
            this.PanelMain.Controls.Add(this.LabelInfo);
            this.PanelMain.Controls.Add(this.ButtonOK);
            this.PanelMain.Controls.Add(this.LabelLicense);
            this.PanelMain.Location = new System.Drawing.Point(0, 0);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(208, 94);
            this.PanelMain.TabIndex = 8;
            // 
            // LinkTutorial
            // 
            this.LinkTutorial.ActiveLinkColor = System.Drawing.Color.Blue;
            this.LinkTutorial.AutoSize = true;
            this.LinkTutorial.HyperLink = "https://github.com/WangHaonie/PlainCEETimer/blob/main/.github/Manual.md";
            this.LinkTutorial.Location = new System.Drawing.Point(81, 71);
            this.LinkTutorial.Name = "LinkTutorial";
            this.LinkTutorial.Size = new System.Drawing.Size(33, 15);
            this.LinkTutorial.TabIndex = 11;
            this.LinkTutorial.TabStop = true;
            this.LinkTutorial.Text = "教程";
            this.LinkTutorial.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // LinkFeedback
            // 
            this.LinkFeedback.ActiveLinkColor = System.Drawing.Color.Blue;
            this.LinkFeedback.AutoSize = true;
            this.LinkFeedback.HyperLink = "https://github.com/WangHaonie/PlainCEETimer/issues/new/choose";
            this.LinkFeedback.Location = new System.Drawing.Point(49, 71);
            this.LinkFeedback.Name = "LinkFeedback";
            this.LinkFeedback.Size = new System.Drawing.Size(33, 15);
            this.LinkFeedback.TabIndex = 10;
            this.LinkFeedback.TabStop = true;
            this.LinkFeedback.Text = "反馈";
            this.LinkFeedback.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // PicBoxLogo
            // 
            this.PicBoxLogo.Cursor = System.Windows.Forms.Cursors.Help;
            this.PicBoxLogo.Location = new System.Drawing.Point(6, 3);
            this.PicBoxLogo.Name = "PicBoxLogo";
            this.PicBoxLogo.Size = new System.Drawing.Size(32, 32);
            this.PicBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicBoxLogo.TabIndex = 9;
            this.PicBoxLogo.TabStop = false;
            this.PicBoxLogo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PicBoxLogo_MouseClick);
            // 
            // LinkGitHub
            // 
            this.LinkGitHub.ActiveLinkColor = System.Drawing.Color.Blue;
            this.LinkGitHub.AutoSize = true;
            this.LinkGitHub.HyperLink = "https://github.com/WangHaonie/PlainCEETimer";
            this.LinkGitHub.Location = new System.Drawing.Point(5, 71);
            this.LinkGitHub.Name = "LinkGitHub";
            this.LinkGitHub.Size = new System.Drawing.Size(45, 15);
            this.LinkGitHub.TabIndex = 8;
            this.LinkGitHub.TabStop = true;
            this.LinkGitHub.Text = "GitHub";
            this.LinkGitHub.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonOK;
            this.ClientSize = new System.Drawing.Size(208, 94);
            this.Controls.Add(this.PanelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.Text = "关于";
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LabelInfo;
        private PlainCEETimer.Controls.PlainButton ButtonOK;
        private System.Windows.Forms.Label LabelLicense;
        private System.Windows.Forms.Panel PanelMain;
        private PlainCEETimer.Controls.Hyperlink LinkGitHub;
        private System.Windows.Forms.PictureBox PicBoxLogo;
        private PlainCEETimer.Controls.Hyperlink LinkFeedback;
        private PlainCEETimer.Controls.Hyperlink LinkTutorial;
    }
}