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
            this.ListViewMain = new PlainCEETimer.Controls.ListViewEx();
            //this.PanelMain = new System.Windows.Forms.Panel();
            //this.ButtonA = new System.Windows.Forms.Button();
            //this.ButtonB = new System.Windows.Forms.Button();
            this.ButtonOperation = new System.Windows.Forms.Button();
            this.PanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListViewMain
            // 
            this.ListViewMain.FullRowSelect = true;
            this.ListViewMain.GridLines = true;
            this.ListViewMain.Headers = new string[] {
        "考试名称",
        "开始日期和时间",
        "结束日期和时间"};
            this.ListViewMain.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListViewMain.HideSelection = false;
            this.ListViewMain.Location = new System.Drawing.Point(3, 2);
            this.ListViewMain.Name = "ListViewMain";
            this.ListViewMain.Size = new System.Drawing.Size(443, 139);
            this.ListViewMain.TabIndex = 0;
            this.ListViewMain.UseCompatibleStateImageBehavior = false;
            this.ListViewMain.View = System.Windows.Forms.View.Details;
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.ListViewMain);
            this.PanelMain.Location = new System.Drawing.Point(6, 3);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(453, 145);
            this.PanelMain.TabIndex = 2;
            // 
            // ButtonA
            // 
            this.ButtonA.Enabled = false;
            this.ButtonA.Location = new System.Drawing.Point(296, 149);
            this.ButtonA.Name = "ButtonA";
            this.ButtonA.Size = new System.Drawing.Size(75, 23);
            this.ButtonA.TabIndex = 4;
            this.ButtonA.Text = "保存(&S)";
            this.ButtonA.UseVisualStyleBackColor = true;
            // 
            // ButtonB
            // 
            this.ButtonB.Location = new System.Drawing.Point(377, 149);
            this.ButtonB.Name = "ButtonB";
            this.ButtonB.Size = new System.Drawing.Size(75, 23);
            this.ButtonB.TabIndex = 5;
            this.ButtonB.Text = "取消(&C)";
            this.ButtonB.UseVisualStyleBackColor = true;
            // 
            // ButtonOperation
            // 
            this.ButtonOperation.Location = new System.Drawing.Point(9, 149);
            this.ButtonOperation.Name = "ButtonOperation";
            this.ButtonOperation.Size = new System.Drawing.Size(75, 23);
            this.ButtonOperation.TabIndex = 6;
            this.ButtonOperation.Text = "操作(&O) ▼";
            this.ButtonOperation.UseVisualStyleBackColor = true;
            this.ButtonOperation.Click += new System.EventHandler(this.ButtonOperation_Click);
            // 
            // ExamInfoManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(460, 176);
            this.Controls.Add(this.ButtonOperation);
            this.Controls.Add(this.ButtonB);
            this.Controls.Add(this.ButtonA);
            this.Controls.Add(this.PanelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExamInfoManager";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "管理考试信息 - 高考倒计时";
            this.PanelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ListViewEx ListViewMain;
        //private System.Windows.Forms.Panel PanelMain;
        //private System.Windows.Forms.Button ButtonA;
        //private System.Windows.Forms.Button ButtonB;
        private System.Windows.Forms.Button ButtonOperation;
    }
}