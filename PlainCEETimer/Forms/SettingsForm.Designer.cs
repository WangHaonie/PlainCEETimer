namespace PlainCEETimer.Forms
{
    partial class SettingsForm
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
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonSyncTime = new System.Windows.Forms.Button();
            this.ButtonRestart = new System.Windows.Forms.Button();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.TabControlMain = new System.Windows.Forms.TabControl();
            this.TabPageGeneral = new System.Windows.Forms.TabPage();
            this.GBoxExamInfo = new System.Windows.Forms.GroupBox();
            this.LabelExamInfoWarning = new System.Windows.Forms.Label();
            this.ButtonExamInfo = new System.Windows.Forms.Button();
            this.LabelExamInfo = new System.Windows.Forms.Label();
            this.GBoxOthers = new System.Windows.Forms.GroupBox();
            this.CheckBoxTrayText = new System.Windows.Forms.CheckBox();
            this.CheckBoxWCCMS = new System.Windows.Forms.CheckBox();
            this.CheckBoxMemClean = new System.Windows.Forms.CheckBox();
            this.CheckBoxTrayIcon = new System.Windows.Forms.CheckBox();
            this.CheckBoxUniTopMost = new System.Windows.Forms.CheckBox();
            this.CheckBoxTopMost = new System.Windows.Forms.CheckBox();
            this.CheckBoxStartup = new System.Windows.Forms.CheckBox();
            this.TabPageDisplay = new System.Windows.Forms.TabPage();
            this.GBoxPptsvc = new System.Windows.Forms.GroupBox();
            this.CheckBoxPptSvc = new System.Windows.Forms.CheckBox();
            this.LabelPptsvc = new System.Windows.Forms.Label();
            this.GBoxContent = new System.Windows.Forms.GroupBox();
            this.LabelCountdownEnd = new System.Windows.Forms.Label();
            this.ComboBoxCountdownEnd = new System.Windows.Forms.ComboBox();
            this.CheckBoxRulesMan = new System.Windows.Forms.CheckBox();
            this.ButtonRulesMan = new System.Windows.Forms.Button();
            this.ComboBoxShowXOnly = new System.Windows.Forms.ComboBox();
            this.CheckBoxCeiling = new System.Windows.Forms.CheckBox();
            this.CheckBoxShowXOnly = new System.Windows.Forms.CheckBox();
            this.GBoxDraggable = new System.Windows.Forms.GroupBox();
            this.ComboBoxPosition = new System.Windows.Forms.ComboBox();
            this.CheckBoxDraggable = new System.Windows.Forms.CheckBox();
            this.LabelChar1 = new System.Windows.Forms.Label();
            this.LabelScreens = new System.Windows.Forms.Label();
            this.ComboBoxScreens = new System.Windows.Forms.ComboBox();
            this.TabPageAppearance = new System.Windows.Forms.TabPage();
            this.GBoxColors = new System.Windows.Forms.GroupBox();
            this.LabelPreviewColor4 = new System.Windows.Forms.Label();
            this.LabelColor41 = new System.Windows.Forms.Label();
            this.LabelColor42 = new System.Windows.Forms.Label();
            this.LabelLine05 = new System.Windows.Forms.Label();
            this.LabelPreviewColor1 = new System.Windows.Forms.Label();
            this.LabelPreviewColor2 = new System.Windows.Forms.Label();
            this.LabelPreviewColor3 = new System.Windows.Forms.Label();
            this.LabelLine01 = new System.Windows.Forms.Label();
            this.ButtonDefaultColor = new System.Windows.Forms.Button();
            this.LabelColor31 = new System.Windows.Forms.Label();
            this.LabelColor32 = new System.Windows.Forms.Label();
            this.LabelColor21 = new System.Windows.Forms.Label();
            this.LabelColor22 = new System.Windows.Forms.Label();
            this.LabelColor11 = new System.Windows.Forms.Label();
            this.LabelColor12 = new System.Windows.Forms.Label();
            this.LabelLine04 = new System.Windows.Forms.Label();
            this.LabelLine03 = new System.Windows.Forms.Label();
            this.LabelLine02 = new System.Windows.Forms.Label();
            this.GBoxFont = new System.Windows.Forms.GroupBox();
            this.ButtonDefaultFont = new System.Windows.Forms.Button();
            this.ButtonFont = new System.Windows.Forms.Button();
            this.LabelFont = new System.Windows.Forms.Label();
            this.TabPageTools = new System.Windows.Forms.TabPage();
            this.GBoxRestart = new System.Windows.Forms.GroupBox();
            this.LabelRestart = new System.Windows.Forms.Label();
            this.GBoxSyncTime = new System.Windows.Forms.GroupBox();
            this.ComboBoxNtpServers = new System.Windows.Forms.ComboBox();
            this.LabelSyncTime = new System.Windows.Forms.Label();
            this.TabControlMain.SuspendLayout();
            this.TabPageGeneral.SuspendLayout();
            this.GBoxExamInfo.SuspendLayout();
            this.GBoxOthers.SuspendLayout();
            this.TabPageDisplay.SuspendLayout();
            this.GBoxPptsvc.SuspendLayout();
            this.GBoxContent.SuspendLayout();
            this.GBoxDraggable.SuspendLayout();
            this.TabPageAppearance.SuspendLayout();
            this.GBoxColors.SuspendLayout();
            this.GBoxFont.SuspendLayout();
            this.TabPageTools.SuspendLayout();
            this.GBoxRestart.SuspendLayout();
            this.GBoxSyncTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(276, 296);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 25);
            this.ButtonCancel.TabIndex = 17;
            this.ButtonCancel.Text = "取消(&C)";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonSyncTime
            // 
            this.ButtonSyncTime.AutoSize = true;
            this.ButtonSyncTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonSyncTime.Location = new System.Drawing.Point(145, 54);
            this.ButtonSyncTime.Name = "ButtonSyncTime";
            this.ButtonSyncTime.Size = new System.Drawing.Size(84, 25);
            this.ButtonSyncTime.TabIndex = 19;
            this.ButtonSyncTime.Text = "立即同步(&Y)";
            this.ButtonSyncTime.UseVisualStyleBackColor = true;
            this.ButtonSyncTime.Click += new System.EventHandler(this.ButtonSyncTime_Click);
            // 
            // ButtonRestart
            // 
            this.ButtonRestart.AutoSize = true;
            this.ButtonRestart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonRestart.Location = new System.Drawing.Point(9, 54);
            this.ButtonRestart.Name = "ButtonRestart";
            this.ButtonRestart.Size = new System.Drawing.Size(6, 6);
            this.ButtonRestart.TabIndex = 36;
            this.ButtonRestart.UseVisualStyleBackColor = true;
            this.ButtonRestart.Click += new System.EventHandler(this.ButtonRestart_Click);
            this.ButtonRestart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonRestart_MouseDown);
            // 
            // ButtonSave
            // 
            this.ButtonSave.Enabled = false;
            this.ButtonSave.Location = new System.Drawing.Point(195, 296);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(75, 25);
            this.ButtonSave.TabIndex = 16;
            this.ButtonSave.Text = "保存(&S)";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // TabControlMain
            // 
            this.TabControlMain.Controls.Add(this.TabPageGeneral);
            this.TabControlMain.Controls.Add(this.TabPageDisplay);
            this.TabControlMain.Controls.Add(this.TabPageAppearance);
            this.TabControlMain.Controls.Add(this.TabPageTools);
            this.TabControlMain.Location = new System.Drawing.Point(5, 4);
            this.TabControlMain.Name = "TabControlMain";
            this.TabControlMain.SelectedIndex = 0;
            this.TabControlMain.Size = new System.Drawing.Size(347, 289);
            this.TabControlMain.TabIndex = 40;
            // 
            // TabPageGeneral
            // 
            this.TabPageGeneral.Controls.Add(this.GBoxExamInfo);
            this.TabPageGeneral.Controls.Add(this.GBoxOthers);
            this.TabPageGeneral.Location = new System.Drawing.Point(4, 24);
            this.TabPageGeneral.Name = "TabPageGeneral";
            this.TabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageGeneral.Size = new System.Drawing.Size(339, 261);
            this.TabPageGeneral.TabIndex = 0;
            this.TabPageGeneral.Text = "基本";
            this.TabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // GBoxExamInfo
            // 
            this.GBoxExamInfo.Controls.Add(this.LabelExamInfoWarning);
            this.GBoxExamInfo.Controls.Add(this.ButtonExamInfo);
            this.GBoxExamInfo.Controls.Add(this.LabelExamInfo);
            this.GBoxExamInfo.Location = new System.Drawing.Point(7, 5);
            this.GBoxExamInfo.Name = "GBoxExamInfo";
            this.GBoxExamInfo.Size = new System.Drawing.Size(323, 96);
            this.GBoxExamInfo.TabIndex = 45;
            this.GBoxExamInfo.TabStop = false;
            this.GBoxExamInfo.Text = "当前考试信息";
            // 
            // LabelExamInfoWarning
            // 
            this.LabelExamInfoWarning.AutoSize = true;
            this.LabelExamInfoWarning.ForeColor = System.Drawing.Color.Red;
            this.LabelExamInfoWarning.Location = new System.Drawing.Point(90, 71);
            this.LabelExamInfoWarning.Name = "LabelExamInfoWarning";
            this.LabelExamInfoWarning.Size = new System.Drawing.Size(135, 15);
            this.LabelExamInfoWarning.TabIndex = 4;
            this.LabelExamInfoWarning.Text = "*修改成功, 保存后生效";
            this.LabelExamInfoWarning.Visible = false;
            // 
            // ButtonExamInfo
            // 
            this.ButtonExamInfo.Location = new System.Drawing.Point(9, 67);
            this.ButtonExamInfo.Name = "ButtonExamInfo";
            this.ButtonExamInfo.Size = new System.Drawing.Size(75, 23);
            this.ButtonExamInfo.TabIndex = 3;
            this.ButtonExamInfo.Text = "管理(&G)";
            this.ButtonExamInfo.UseVisualStyleBackColor = true;
            this.ButtonExamInfo.Click += new System.EventHandler(this.ButtonExamInfo_Click);
            // 
            // LabelExamInfo
            // 
            this.LabelExamInfo.AutoSize = true;
            this.LabelExamInfo.Location = new System.Drawing.Point(6, 19);
            this.LabelExamInfo.Name = "LabelExamInfo";
            this.LabelExamInfo.Size = new System.Drawing.Size(0, 15);
            this.LabelExamInfo.TabIndex = 0;
            // 
            // GBoxOthers
            // 
            this.GBoxOthers.Controls.Add(this.CheckBoxTrayText);
            this.GBoxOthers.Controls.Add(this.CheckBoxWCCMS);
            this.GBoxOthers.Controls.Add(this.CheckBoxMemClean);
            this.GBoxOthers.Controls.Add(this.CheckBoxTrayIcon);
            this.GBoxOthers.Controls.Add(this.CheckBoxUniTopMost);
            this.GBoxOthers.Controls.Add(this.CheckBoxTopMost);
            this.GBoxOthers.Controls.Add(this.CheckBoxStartup);
            this.GBoxOthers.Location = new System.Drawing.Point(7, 102);
            this.GBoxOthers.Name = "GBoxOthers";
            this.GBoxOthers.Size = new System.Drawing.Size(323, 149);
            this.GBoxOthers.TabIndex = 44;
            this.GBoxOthers.TabStop = false;
            this.GBoxOthers.Text = "其他";
            // 
            // CheckBoxTrayText
            // 
            this.CheckBoxTrayText.AutoSize = true;
            this.CheckBoxTrayText.Enabled = false;
            this.CheckBoxTrayText.Location = new System.Drawing.Point(9, 97);
            this.CheckBoxTrayText.Name = "CheckBoxTrayText";
            this.CheckBoxTrayText.Size = new System.Drawing.Size(277, 19);
            this.CheckBoxTrayText.TabIndex = 1;
            this.CheckBoxTrayText.Text = "鼠标悬停在通知图标上时显示倒计时内容(&N)";
            this.CheckBoxTrayText.UseVisualStyleBackColor = true;
            this.CheckBoxTrayText.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxWCCMS
            // 
            this.CheckBoxWCCMS.AutoSize = true;
            this.CheckBoxWCCMS.Location = new System.Drawing.Point(9, 122);
            this.CheckBoxWCCMS.Name = "CheckBoxWCCMS";
            this.CheckBoxWCCMS.Size = new System.Drawing.Size(228, 19);
            this.CheckBoxWCCMS.TabIndex = 19;
            this.CheckBoxWCCMS.Text = "使用 Windows 经典右键菜单样式(&Q)";
            this.CheckBoxWCCMS.UseVisualStyleBackColor = true;
            this.CheckBoxWCCMS.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxMemClean
            // 
            this.CheckBoxMemClean.AutoSize = true;
            this.CheckBoxMemClean.Location = new System.Drawing.Point(157, 22);
            this.CheckBoxMemClean.Name = "CheckBoxMemClean";
            this.CheckBoxMemClean.Size = new System.Drawing.Size(149, 19);
            this.CheckBoxMemClean.TabIndex = 2;
            this.CheckBoxMemClean.Text = "自动清理程序内存(&M)";
            this.CheckBoxMemClean.UseVisualStyleBackColor = true;
            this.CheckBoxMemClean.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxTrayIcon
            // 
            this.CheckBoxTrayIcon.AutoSize = true;
            this.CheckBoxTrayIcon.Location = new System.Drawing.Point(9, 72);
            this.CheckBoxTrayIcon.Name = "CheckBoxTrayIcon";
            this.CheckBoxTrayIcon.Size = new System.Drawing.Size(180, 19);
            this.CheckBoxTrayIcon.TabIndex = 0;
            this.CheckBoxTrayIcon.Text = "在托盘区域显示通知图标(&I)";
            this.CheckBoxTrayIcon.UseVisualStyleBackColor = true;
            this.CheckBoxTrayIcon.CheckedChanged += new System.EventHandler(this.CheckBoxTrayIcon_CheckedChanged);
            // 
            // CheckBoxUniTopMost
            // 
            this.CheckBoxUniTopMost.AutoSize = true;
            this.CheckBoxUniTopMost.Location = new System.Drawing.Point(157, 47);
            this.CheckBoxUniTopMost.Name = "CheckBoxUniTopMost";
            this.CheckBoxUniTopMost.Size = new System.Drawing.Size(120, 19);
            this.CheckBoxUniTopMost.TabIndex = 4;
            this.CheckBoxUniTopMost.Text = "顶置所有窗口(&U)";
            this.CheckBoxUniTopMost.UseVisualStyleBackColor = true;
            this.CheckBoxUniTopMost.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxTopMost
            // 
            this.CheckBoxTopMost.AutoSize = true;
            this.CheckBoxTopMost.Checked = true;
            this.CheckBoxTopMost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxTopMost.Location = new System.Drawing.Point(9, 47);
            this.CheckBoxTopMost.Name = "CheckBoxTopMost";
            this.CheckBoxTopMost.Size = new System.Drawing.Size(132, 19);
            this.CheckBoxTopMost.TabIndex = 0;
            this.CheckBoxTopMost.Text = "顶置倒计时窗口(&T)";
            this.CheckBoxTopMost.UseVisualStyleBackColor = true;
            this.CheckBoxTopMost.CheckedChanged += new System.EventHandler(this.CheckBoxTopMost_CheckedChanged);
            // 
            // CheckBoxStartup
            // 
            this.CheckBoxStartup.AutoSize = true;
            this.CheckBoxStartup.Location = new System.Drawing.Point(9, 22);
            this.CheckBoxStartup.Name = "CheckBoxStartup";
            this.CheckBoxStartup.Size = new System.Drawing.Size(119, 19);
            this.CheckBoxStartup.TabIndex = 18;
            this.CheckBoxStartup.Text = "开机自动启动(&B)";
            this.CheckBoxStartup.UseVisualStyleBackColor = true;
            this.CheckBoxStartup.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // TabPageDisplay
            // 
            this.TabPageDisplay.Controls.Add(this.GBoxPptsvc);
            this.TabPageDisplay.Controls.Add(this.GBoxContent);
            this.TabPageDisplay.Controls.Add(this.GBoxDraggable);
            this.TabPageDisplay.Location = new System.Drawing.Point(4, 24);
            this.TabPageDisplay.Name = "TabPageDisplay";
            this.TabPageDisplay.Size = new System.Drawing.Size(339, 261);
            this.TabPageDisplay.TabIndex = 3;
            this.TabPageDisplay.Text = "显示";
            this.TabPageDisplay.UseVisualStyleBackColor = true;
            // 
            // GBoxPptsvc
            // 
            this.GBoxPptsvc.Controls.Add(this.CheckBoxPptSvc);
            this.GBoxPptsvc.Controls.Add(this.LabelPptsvc);
            this.GBoxPptsvc.Location = new System.Drawing.Point(7, 178);
            this.GBoxPptsvc.Name = "GBoxPptsvc";
            this.GBoxPptsvc.Size = new System.Drawing.Size(323, 78);
            this.GBoxPptsvc.TabIndex = 0;
            this.GBoxPptsvc.TabStop = false;
            this.GBoxPptsvc.Text = "兼容希沃PPT小工具";
            // 
            // CheckBoxPptSvc
            // 
            this.CheckBoxPptSvc.AutoSize = true;
            this.CheckBoxPptSvc.Location = new System.Drawing.Point(9, 53);
            this.CheckBoxPptSvc.Name = "CheckBoxPptSvc";
            this.CheckBoxPptSvc.Size = new System.Drawing.Size(15, 14);
            this.CheckBoxPptSvc.TabIndex = 3;
            this.CheckBoxPptSvc.UseVisualStyleBackColor = true;
            this.CheckBoxPptSvc.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // LabelPptsvc
            // 
            this.LabelPptsvc.AutoSize = true;
            this.LabelPptsvc.Location = new System.Drawing.Point(6, 19);
            this.LabelPptsvc.Name = "LabelPptsvc";
            this.LabelPptsvc.Size = new System.Drawing.Size(587, 15);
            this.LabelPptsvc.TabIndex = 0;
            this.LabelPptsvc.Text = "用于解决个别机型在内置白板打开后底部工具栏消失的情况。(仅倒计时恰好在左上角且顶置才会出现)";
            // 
            // GBoxContent
            // 
            this.GBoxContent.Controls.Add(this.LabelCountdownEnd);
            this.GBoxContent.Controls.Add(this.ComboBoxCountdownEnd);
            this.GBoxContent.Controls.Add(this.CheckBoxRulesMan);
            this.GBoxContent.Controls.Add(this.ButtonRulesMan);
            this.GBoxContent.Controls.Add(this.ComboBoxShowXOnly);
            this.GBoxContent.Controls.Add(this.CheckBoxCeiling);
            this.GBoxContent.Controls.Add(this.CheckBoxShowXOnly);
            this.GBoxContent.Location = new System.Drawing.Point(7, 5);
            this.GBoxContent.Name = "GBoxContent";
            this.GBoxContent.Size = new System.Drawing.Size(323, 98);
            this.GBoxContent.TabIndex = 1;
            this.GBoxContent.TabStop = false;
            this.GBoxContent.Text = "倒计时内容";
            // 
            // LabelCountdownEnd
            // 
            this.LabelCountdownEnd.AutoSize = true;
            this.LabelCountdownEnd.Location = new System.Drawing.Point(6, 47);
            this.LabelCountdownEnd.Name = "LabelCountdownEnd";
            this.LabelCountdownEnd.Size = new System.Drawing.Size(130, 15);
            this.LabelCountdownEnd.TabIndex = 45;
            this.LabelCountdownEnd.Text = "当倒计时结束时, 显示";
            // 
            // ComboBoxCountdownEnd
            // 
            this.ComboBoxCountdownEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCountdownEnd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxCountdownEnd.Location = new System.Drawing.Point(138, 43);
            this.ComboBoxCountdownEnd.MaxDropDownItems = 1;
            this.ComboBoxCountdownEnd.Name = "ComboBoxCountdownEnd";
            this.ComboBoxCountdownEnd.Size = new System.Drawing.Size(178, 23);
            this.ComboBoxCountdownEnd.TabIndex = 44;
            this.ComboBoxCountdownEnd.DropDown += new System.EventHandler(this.ComboBoxes_DropDown);
            this.ComboBoxCountdownEnd.SelectedIndexChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxRulesMan
            // 
            this.CheckBoxRulesMan.AutoSize = true;
            this.CheckBoxRulesMan.Location = new System.Drawing.Point(9, 74);
            this.CheckBoxRulesMan.Name = "CheckBoxRulesMan";
            this.CheckBoxRulesMan.Size = new System.Drawing.Size(198, 19);
            this.CheckBoxRulesMan.TabIndex = 42;
            this.CheckBoxRulesMan.Text = "自定义不同时刻的颜色和内容:";
            this.CheckBoxRulesMan.UseVisualStyleBackColor = true;
            this.CheckBoxRulesMan.CheckedChanged += new System.EventHandler(this.CheckBoxRulesMan_CheckedChanged);
            // 
            // ButtonRulesMan
            // 
            this.ButtonRulesMan.AutoSize = true;
            this.ButtonRulesMan.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonRulesMan.Enabled = false;
            this.ButtonRulesMan.Location = new System.Drawing.Point(219, 70);
            this.ButtonRulesMan.Name = "ButtonRulesMan";
            this.ButtonRulesMan.Size = new System.Drawing.Size(97, 25);
            this.ButtonRulesMan.TabIndex = 41;
            this.ButtonRulesMan.Text = "规则管理器(&R)";
            this.ButtonRulesMan.UseVisualStyleBackColor = true;
            this.ButtonRulesMan.Click += new System.EventHandler(this.ButtonRulesMan_Click);
            // 
            // ComboBoxShowXOnly
            // 
            this.ComboBoxShowXOnly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxShowXOnly.Enabled = false;
            this.ComboBoxShowXOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxShowXOnly.Location = new System.Drawing.Point(72, 19);
            this.ComboBoxShowXOnly.MaxDropDownItems = 1;
            this.ComboBoxShowXOnly.Name = "ComboBoxShowXOnly";
            this.ComboBoxShowXOnly.Size = new System.Drawing.Size(38, 23);
            this.ComboBoxShowXOnly.TabIndex = 5;
            this.ComboBoxShowXOnly.DropDown += new System.EventHandler(this.ComboBoxes_DropDown);
            this.ComboBoxShowXOnly.SelectedIndexChanged += new System.EventHandler(this.ComboBoxShowXOnly_SelectedIndexChanged);
            // 
            // CheckBoxCeiling
            // 
            this.CheckBoxCeiling.AutoSize = true;
            this.CheckBoxCeiling.Enabled = false;
            this.CheckBoxCeiling.Location = new System.Drawing.Point(117, 22);
            this.CheckBoxCeiling.Name = "CheckBoxCeiling";
            this.CheckBoxCeiling.Size = new System.Drawing.Size(160, 19);
            this.CheckBoxCeiling.TabIndex = 1;
            this.CheckBoxCeiling.Text = "不足一天按整天计算(&N)";
            this.CheckBoxCeiling.UseVisualStyleBackColor = true;
            this.CheckBoxCeiling.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxShowXOnly
            // 
            this.CheckBoxShowXOnly.AutoSize = true;
            this.CheckBoxShowXOnly.Location = new System.Drawing.Point(9, 22);
            this.CheckBoxShowXOnly.Name = "CheckBoxShowXOnly";
            this.CheckBoxShowXOnly.Size = new System.Drawing.Size(65, 19);
            this.CheckBoxShowXOnly.TabIndex = 0;
            this.CheckBoxShowXOnly.Text = "只显示";
            this.CheckBoxShowXOnly.UseVisualStyleBackColor = true;
            this.CheckBoxShowXOnly.CheckedChanged += new System.EventHandler(this.CheckBoxShowXOnly_CheckedChanged);
            // 
            // GBoxDraggable
            // 
            this.GBoxDraggable.Controls.Add(this.ComboBoxPosition);
            this.GBoxDraggable.Controls.Add(this.CheckBoxDraggable);
            this.GBoxDraggable.Controls.Add(this.LabelChar1);
            this.GBoxDraggable.Controls.Add(this.LabelScreens);
            this.GBoxDraggable.Controls.Add(this.ComboBoxScreens);
            this.GBoxDraggable.Location = new System.Drawing.Point(7, 105);
            this.GBoxDraggable.Name = "GBoxDraggable";
            this.GBoxDraggable.Size = new System.Drawing.Size(323, 71);
            this.GBoxDraggable.TabIndex = 4;
            this.GBoxDraggable.TabStop = false;
            this.GBoxDraggable.Text = "多显示器与拖动";
            // 
            // ComboBoxPosition
            // 
            this.ComboBoxPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPosition.Enabled = false;
            this.ComboBoxPosition.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxPosition.Location = new System.Drawing.Point(232, 17);
            this.ComboBoxPosition.Name = "ComboBoxPosition";
            this.ComboBoxPosition.Size = new System.Drawing.Size(84, 23);
            this.ComboBoxPosition.TabIndex = 7;
            this.ComboBoxPosition.DropDown += new System.EventHandler(this.ComboBoxes_DropDown);
            this.ComboBoxPosition.SelectedIndexChanged += new System.EventHandler(this.ChangePptsvcStyle);
            // 
            // CheckBoxDraggable
            // 
            this.CheckBoxDraggable.AutoSize = true;
            this.CheckBoxDraggable.Location = new System.Drawing.Point(9, 45);
            this.CheckBoxDraggable.Name = "CheckBoxDraggable";
            this.CheckBoxDraggable.Size = new System.Drawing.Size(159, 19);
            this.CheckBoxDraggable.TabIndex = 3;
            this.CheckBoxDraggable.Text = "允许拖动倒计时窗口(&D)";
            this.CheckBoxDraggable.UseVisualStyleBackColor = true;
            this.CheckBoxDraggable.CheckedChanged += new System.EventHandler(this.CheckBoxDraggable_CheckedChanged);
            // 
            // LabelChar1
            // 
            this.LabelChar1.AutoSize = true;
            this.LabelChar1.Location = new System.Drawing.Point(193, 21);
            this.LabelChar1.Name = "LabelChar1";
            this.LabelChar1.Size = new System.Drawing.Size(33, 15);
            this.LabelChar1.TabIndex = 6;
            this.LabelChar1.Text = "位置";
            // 
            // LabelScreens
            // 
            this.LabelScreens.AutoSize = true;
            this.LabelScreens.Location = new System.Drawing.Point(6, 21);
            this.LabelScreens.Name = "LabelScreens";
            this.LabelScreens.Size = new System.Drawing.Size(72, 15);
            this.LabelScreens.TabIndex = 5;
            this.LabelScreens.Text = "固定在屏幕";
            // 
            // ComboBoxScreens
            // 
            this.ComboBoxScreens.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxScreens.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxScreens.Location = new System.Drawing.Point(80, 17);
            this.ComboBoxScreens.MaxDropDownItems = 1;
            this.ComboBoxScreens.Name = "ComboBoxScreens";
            this.ComboBoxScreens.Size = new System.Drawing.Size(107, 23);
            this.ComboBoxScreens.TabIndex = 4;
            this.ComboBoxScreens.DropDown += new System.EventHandler(this.ComboBoxes_DropDown);
            this.ComboBoxScreens.SelectedIndexChanged += new System.EventHandler(this.ComboBoxScreens_SelectedIndexChanged);
            // 
            // TabPageAppearance
            // 
            this.TabPageAppearance.Controls.Add(this.GBoxColors);
            this.TabPageAppearance.Controls.Add(this.GBoxFont);
            this.TabPageAppearance.Location = new System.Drawing.Point(4, 24);
            this.TabPageAppearance.Name = "TabPageAppearance";
            this.TabPageAppearance.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageAppearance.Size = new System.Drawing.Size(339, 261);
            this.TabPageAppearance.TabIndex = 4;
            this.TabPageAppearance.Text = "外观";
            this.TabPageAppearance.UseVisualStyleBackColor = true;
            // 
            // GBoxColors
            // 
            this.GBoxColors.Controls.Add(this.LabelPreviewColor4);
            this.GBoxColors.Controls.Add(this.LabelColor41);
            this.GBoxColors.Controls.Add(this.LabelColor42);
            this.GBoxColors.Controls.Add(this.LabelLine05);
            this.GBoxColors.Controls.Add(this.LabelPreviewColor1);
            this.GBoxColors.Controls.Add(this.LabelPreviewColor2);
            this.GBoxColors.Controls.Add(this.LabelPreviewColor3);
            this.GBoxColors.Controls.Add(this.LabelLine01);
            this.GBoxColors.Controls.Add(this.ButtonDefaultColor);
            this.GBoxColors.Controls.Add(this.LabelColor31);
            this.GBoxColors.Controls.Add(this.LabelColor32);
            this.GBoxColors.Controls.Add(this.LabelColor21);
            this.GBoxColors.Controls.Add(this.LabelColor22);
            this.GBoxColors.Controls.Add(this.LabelColor11);
            this.GBoxColors.Controls.Add(this.LabelColor12);
            this.GBoxColors.Controls.Add(this.LabelLine04);
            this.GBoxColors.Controls.Add(this.LabelLine03);
            this.GBoxColors.Controls.Add(this.LabelLine02);
            this.GBoxColors.Location = new System.Drawing.Point(7, 79);
            this.GBoxColors.Name = "GBoxColors";
            this.GBoxColors.Size = new System.Drawing.Size(323, 168);
            this.GBoxColors.TabIndex = 0;
            this.GBoxColors.TabStop = false;
            this.GBoxColors.Text = "字体颜色";
            // 
            // LabelPreviewColor4
            // 
            this.LabelPreviewColor4.AutoSize = true;
            this.LabelPreviewColor4.Location = new System.Drawing.Point(200, 118);
            this.LabelPreviewColor4.Name = "LabelPreviewColor4";
            this.LabelPreviewColor4.Size = new System.Drawing.Size(59, 15);
            this.LabelPreviewColor4.TabIndex = 19;
            this.LabelPreviewColor4.Text = "欢迎使用";
            // 
            // LabelColor41
            // 
            this.LabelColor41.AutoSize = true;
            this.LabelColor41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor41.Location = new System.Drawing.Point(110, 118);
            this.LabelColor41.Name = "LabelColor41";
            this.LabelColor41.Size = new System.Drawing.Size(39, 17);
            this.LabelColor41.TabIndex = 18;
            this.LabelColor41.Text = "          ";
            // 
            // LabelColor42
            // 
            this.LabelColor42.AutoSize = true;
            this.LabelColor42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor42.Location = new System.Drawing.Point(155, 118);
            this.LabelColor42.Name = "LabelColor42";
            this.LabelColor42.Size = new System.Drawing.Size(39, 17);
            this.LabelColor42.TabIndex = 17;
            this.LabelColor42.Text = "          ";
            // 
            // LabelLine05
            // 
            this.LabelLine05.AutoSize = true;
            this.LabelLine05.Location = new System.Drawing.Point(6, 119);
            this.LabelLine05.Name = "LabelLine05";
            this.LabelLine05.Size = new System.Drawing.Size(99, 15);
            this.LabelLine05.TabIndex = 16;
            this.LabelLine05.Text = "[4]程序欢迎界面";
            // 
            // LabelPreviewColor1
            // 
            this.LabelPreviewColor1.AutoSize = true;
            this.LabelPreviewColor1.Location = new System.Drawing.Point(200, 53);
            this.LabelPreviewColor1.Name = "LabelPreviewColor1";
            this.LabelPreviewColor1.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor1.TabIndex = 15;
            // 
            // LabelPreviewColor2
            // 
            this.LabelPreviewColor2.AutoSize = true;
            this.LabelPreviewColor2.Location = new System.Drawing.Point(200, 75);
            this.LabelPreviewColor2.Name = "LabelPreviewColor2";
            this.LabelPreviewColor2.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor2.TabIndex = 14;
            // 
            // LabelPreviewColor3
            // 
            this.LabelPreviewColor3.AutoSize = true;
            this.LabelPreviewColor3.Location = new System.Drawing.Point(200, 97);
            this.LabelPreviewColor3.Name = "LabelPreviewColor3";
            this.LabelPreviewColor3.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor3.TabIndex = 13;
            // 
            // LabelLine01
            // 
            this.LabelLine01.AutoSize = true;
            this.LabelLine01.Location = new System.Drawing.Point(6, 19);
            this.LabelLine01.Name = "LabelLine01";
            this.LabelLine01.Size = new System.Drawing.Size(527, 15);
            this.LabelLine01.TabIndex = 12;
            this.LabelLine01.Text = "请点击色块来选择文字、背景颜色。将一个色块拖放到其它色块上可快速应用相同的颜色。";
            // 
            // ButtonDefaultColor
            // 
            this.ButtonDefaultColor.AutoSize = true;
            this.ButtonDefaultColor.Location = new System.Drawing.Point(9, 137);
            this.ButtonDefaultColor.Name = "ButtonDefaultColor";
            this.ButtonDefaultColor.Size = new System.Drawing.Size(88, 25);
            this.ButtonDefaultColor.TabIndex = 11;
            this.ButtonDefaultColor.Text = "恢复默认(&M)";
            this.ButtonDefaultColor.UseVisualStyleBackColor = true;
            this.ButtonDefaultColor.Click += new System.EventHandler(this.ButtonDefaultColor_Click);
            // 
            // LabelColor31
            // 
            this.LabelColor31.AutoSize = true;
            this.LabelColor31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor31.Location = new System.Drawing.Point(110, 96);
            this.LabelColor31.Name = "LabelColor31";
            this.LabelColor31.Size = new System.Drawing.Size(39, 17);
            this.LabelColor31.TabIndex = 9;
            this.LabelColor31.Text = "          ";
            // 
            // LabelColor32
            // 
            this.LabelColor32.AutoSize = true;
            this.LabelColor32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor32.Location = new System.Drawing.Point(155, 96);
            this.LabelColor32.Name = "LabelColor32";
            this.LabelColor32.Size = new System.Drawing.Size(39, 17);
            this.LabelColor32.TabIndex = 8;
            this.LabelColor32.Text = "          ";
            // 
            // LabelColor21
            // 
            this.LabelColor21.AutoSize = true;
            this.LabelColor21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor21.Location = new System.Drawing.Point(110, 74);
            this.LabelColor21.Name = "LabelColor21";
            this.LabelColor21.Size = new System.Drawing.Size(39, 17);
            this.LabelColor21.TabIndex = 7;
            this.LabelColor21.Text = "          ";
            // 
            // LabelColor22
            // 
            this.LabelColor22.AutoSize = true;
            this.LabelColor22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor22.Location = new System.Drawing.Point(155, 74);
            this.LabelColor22.Name = "LabelColor22";
            this.LabelColor22.Size = new System.Drawing.Size(39, 17);
            this.LabelColor22.TabIndex = 6;
            this.LabelColor22.Text = "          ";
            // 
            // LabelColor11
            // 
            this.LabelColor11.AutoSize = true;
            this.LabelColor11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor11.Location = new System.Drawing.Point(110, 52);
            this.LabelColor11.Name = "LabelColor11";
            this.LabelColor11.Size = new System.Drawing.Size(39, 17);
            this.LabelColor11.TabIndex = 5;
            this.LabelColor11.Text = "          ";
            // 
            // LabelColor12
            // 
            this.LabelColor12.AutoSize = true;
            this.LabelColor12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor12.Location = new System.Drawing.Point(155, 52);
            this.LabelColor12.Name = "LabelColor12";
            this.LabelColor12.Size = new System.Drawing.Size(39, 17);
            this.LabelColor12.TabIndex = 0;
            this.LabelColor12.Text = "          ";
            // 
            // LabelLine04
            // 
            this.LabelLine04.AutoSize = true;
            this.LabelLine04.Location = new System.Drawing.Point(6, 97);
            this.LabelLine04.Name = "LabelLine04";
            this.LabelLine04.Size = new System.Drawing.Size(99, 15);
            this.LabelLine04.TabIndex = 4;
            this.LabelLine04.Text = "[3]考试已结束时";
            // 
            // LabelLine03
            // 
            this.LabelLine03.AutoSize = true;
            this.LabelLine03.Location = new System.Drawing.Point(6, 75);
            this.LabelLine03.Name = "LabelLine03";
            this.LabelLine03.Size = new System.Drawing.Size(99, 15);
            this.LabelLine03.TabIndex = 3;
            this.LabelLine03.Text = "[2]考试已开始时";
            // 
            // LabelLine02
            // 
            this.LabelLine02.AutoSize = true;
            this.LabelLine02.Location = new System.Drawing.Point(6, 53);
            this.LabelLine02.Name = "LabelLine02";
            this.LabelLine02.Size = new System.Drawing.Size(99, 15);
            this.LabelLine02.TabIndex = 2;
            this.LabelLine02.Text = "[1]考试未开始时";
            // 
            // GBoxFont
            // 
            this.GBoxFont.Controls.Add(this.ButtonDefaultFont);
            this.GBoxFont.Controls.Add(this.ButtonFont);
            this.GBoxFont.Controls.Add(this.LabelFont);
            this.GBoxFont.Location = new System.Drawing.Point(7, 5);
            this.GBoxFont.Name = "GBoxFont";
            this.GBoxFont.Size = new System.Drawing.Size(323, 68);
            this.GBoxFont.TabIndex = 0;
            this.GBoxFont.TabStop = false;
            this.GBoxFont.Text = "字体和大小";
            // 
            // ButtonDefaultFont
            // 
            this.ButtonDefaultFont.AutoSize = true;
            this.ButtonDefaultFont.Location = new System.Drawing.Point(98, 37);
            this.ButtonDefaultFont.Name = "ButtonDefaultFont";
            this.ButtonDefaultFont.Size = new System.Drawing.Size(86, 25);
            this.ButtonDefaultFont.TabIndex = 4;
            this.ButtonDefaultFont.Text = "恢复默认(&H)";
            this.ButtonDefaultFont.UseVisualStyleBackColor = true;
            this.ButtonDefaultFont.Click += new System.EventHandler(this.ButtonDefaultFont_Click);
            // 
            // ButtonFont
            // 
            this.ButtonFont.AutoSize = true;
            this.ButtonFont.Location = new System.Drawing.Point(9, 37);
            this.ButtonFont.Name = "ButtonFont";
            this.ButtonFont.Size = new System.Drawing.Size(83, 25);
            this.ButtonFont.TabIndex = 2;
            this.ButtonFont.Text = "选择字体(&F)";
            this.ButtonFont.UseVisualStyleBackColor = true;
            this.ButtonFont.Click += new System.EventHandler(this.ButtonFont_Click);
            // 
            // LabelFont
            // 
            this.LabelFont.AutoSize = true;
            this.LabelFont.Location = new System.Drawing.Point(6, 19);
            this.LabelFont.Name = "LabelFont";
            this.LabelFont.Size = new System.Drawing.Size(0, 15);
            this.LabelFont.TabIndex = 1;
            // 
            // TabPageTools
            // 
            this.TabPageTools.Controls.Add(this.GBoxRestart);
            this.TabPageTools.Controls.Add(this.GBoxSyncTime);
            this.TabPageTools.Location = new System.Drawing.Point(4, 24);
            this.TabPageTools.Name = "TabPageTools";
            this.TabPageTools.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTools.Size = new System.Drawing.Size(339, 261);
            this.TabPageTools.TabIndex = 1;
            this.TabPageTools.Text = "工具";
            this.TabPageTools.UseVisualStyleBackColor = true;
            // 
            // GBoxRestart
            // 
            this.GBoxRestart.Controls.Add(this.LabelRestart);
            this.GBoxRestart.Controls.Add(this.ButtonRestart);
            this.GBoxRestart.Location = new System.Drawing.Point(7, 94);
            this.GBoxRestart.Name = "GBoxRestart";
            this.GBoxRestart.Size = new System.Drawing.Size(323, 83);
            this.GBoxRestart.TabIndex = 45;
            this.GBoxRestart.TabStop = false;
            // 
            // LabelRestart
            // 
            this.LabelRestart.AutoSize = true;
            this.LabelRestart.Location = new System.Drawing.Point(6, 19);
            this.LabelRestart.Name = "LabelRestart";
            this.LabelRestart.Size = new System.Drawing.Size(0, 15);
            this.LabelRestart.TabIndex = 37;
            // 
            // GBoxSyncTime
            // 
            this.GBoxSyncTime.Controls.Add(this.ComboBoxNtpServers);
            this.GBoxSyncTime.Controls.Add(this.LabelSyncTime);
            this.GBoxSyncTime.Controls.Add(this.ButtonSyncTime);
            this.GBoxSyncTime.Location = new System.Drawing.Point(7, 5);
            this.GBoxSyncTime.Name = "GBoxSyncTime";
            this.GBoxSyncTime.Size = new System.Drawing.Size(323, 83);
            this.GBoxSyncTime.TabIndex = 44;
            this.GBoxSyncTime.TabStop = false;
            this.GBoxSyncTime.Text = "同步网络时钟";
            // 
            // ComboBoxNtpServers
            // 
            this.ComboBoxNtpServers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxNtpServers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxNtpServers.Location = new System.Drawing.Point(9, 53);
            this.ComboBoxNtpServers.MaxDropDownItems = 1;
            this.ComboBoxNtpServers.Name = "ComboBoxNtpServers";
            this.ComboBoxNtpServers.Size = new System.Drawing.Size(130, 23);
            this.ComboBoxNtpServers.TabIndex = 21;
            this.ComboBoxNtpServers.DropDown += new System.EventHandler(this.ComboBoxes_DropDown);
            this.ComboBoxNtpServers.SelectedIndexChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // LabelSyncTime
            // 
            this.LabelSyncTime.AutoSize = true;
            this.LabelSyncTime.Location = new System.Drawing.Point(6, 19);
            this.LabelSyncTime.Name = "LabelSyncTime";
            this.LabelSyncTime.Size = new System.Drawing.Size(517, 15);
            this.LabelSyncTime.TabIndex = 20;
            this.LabelSyncTime.Text = "设置默认 NTP 服务器并同步系统时间。此项将自动启动 Windows Time 服务, 请谨慎操作。";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.ButtonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(359, 327);
            this.Controls.Add(this.TabControlMain);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.ButtonCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置 - 高考倒计时";
            this.TabControlMain.ResumeLayout(false);
            this.TabPageGeneral.ResumeLayout(false);
            this.GBoxExamInfo.ResumeLayout(false);
            this.GBoxExamInfo.PerformLayout();
            this.GBoxOthers.ResumeLayout(false);
            this.GBoxOthers.PerformLayout();
            this.TabPageDisplay.ResumeLayout(false);
            this.GBoxPptsvc.ResumeLayout(false);
            this.GBoxPptsvc.PerformLayout();
            this.GBoxContent.ResumeLayout(false);
            this.GBoxContent.PerformLayout();
            this.GBoxDraggable.ResumeLayout(false);
            this.GBoxDraggable.PerformLayout();
            this.TabPageAppearance.ResumeLayout(false);
            this.GBoxColors.ResumeLayout(false);
            this.GBoxColors.PerformLayout();
            this.GBoxFont.ResumeLayout(false);
            this.GBoxFont.PerformLayout();
            this.TabPageTools.ResumeLayout(false);
            this.GBoxRestart.ResumeLayout(false);
            this.GBoxRestart.PerformLayout();
            this.GBoxSyncTime.ResumeLayout(false);
            this.GBoxSyncTime.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonSyncTime;
        private System.Windows.Forms.Button ButtonRestart;
        private System.Windows.Forms.Button ButtonSave;
        private System.Windows.Forms.TabControl TabControlMain;
        private System.Windows.Forms.TabPage TabPageTools;
        private System.Windows.Forms.GroupBox GBoxRestart;
        private System.Windows.Forms.GroupBox GBoxSyncTime;
        private System.Windows.Forms.Label LabelRestart;
        private System.Windows.Forms.Label LabelSyncTime;
        private System.Windows.Forms.TabPage TabPageDisplay;
        private System.Windows.Forms.GroupBox GBoxFont;
        private System.Windows.Forms.Label LabelFont;
        private System.Windows.Forms.Button ButtonFont;
        private System.Windows.Forms.Button ButtonDefaultFont;
        private System.Windows.Forms.GroupBox GBoxContent;
        private System.Windows.Forms.CheckBox CheckBoxShowXOnly;
        private System.Windows.Forms.CheckBox CheckBoxCeiling;
        private System.Windows.Forms.GroupBox GBoxDraggable;
        private System.Windows.Forms.CheckBox CheckBoxDraggable;
        private System.Windows.Forms.GroupBox GBoxPptsvc;
        private System.Windows.Forms.Label LabelPptsvc;
        private System.Windows.Forms.CheckBox CheckBoxPptSvc;
        private System.Windows.Forms.Label LabelScreens;
        private System.Windows.Forms.ComboBox ComboBoxScreens;
        private System.Windows.Forms.ComboBox ComboBoxShowXOnly;
        private System.Windows.Forms.TabPage TabPageAppearance;
        private System.Windows.Forms.GroupBox GBoxColors;
        private System.Windows.Forms.Label LabelColor31;
        private System.Windows.Forms.Label LabelColor32;
        private System.Windows.Forms.Label LabelColor21;
        private System.Windows.Forms.Label LabelColor22;
        private System.Windows.Forms.Label LabelColor11;
        private System.Windows.Forms.Label LabelColor12;
        private System.Windows.Forms.Label LabelLine04;
        private System.Windows.Forms.Label LabelLine03;
        private System.Windows.Forms.Label LabelLine02;
        private System.Windows.Forms.Button ButtonDefaultColor;
        private System.Windows.Forms.Label LabelLine01;
        private System.Windows.Forms.Label LabelPreviewColor1;
        private System.Windows.Forms.Label LabelPreviewColor2;
        private System.Windows.Forms.Label LabelPreviewColor3;
        private System.Windows.Forms.Label LabelPreviewColor4;
        private System.Windows.Forms.Label LabelColor41;
        private System.Windows.Forms.Label LabelColor42;
        private System.Windows.Forms.Label LabelLine05;
        private System.Windows.Forms.ComboBox ComboBoxPosition;
        private System.Windows.Forms.Label LabelChar1;
        private System.Windows.Forms.Button ButtonRulesMan;
        private System.Windows.Forms.CheckBox CheckBoxRulesMan;
        private System.Windows.Forms.ComboBox ComboBoxNtpServers;
        private System.Windows.Forms.ComboBox ComboBoxCountdownEnd;
        private System.Windows.Forms.Label LabelCountdownEnd;
        private System.Windows.Forms.CheckBox CheckBoxTrayText;
        private System.Windows.Forms.CheckBox CheckBoxTrayIcon;
        private System.Windows.Forms.TabPage TabPageGeneral;
        private System.Windows.Forms.GroupBox GBoxExamInfo;
        private System.Windows.Forms.Button ButtonExamInfo;
        private System.Windows.Forms.Label LabelExamInfo;
        private System.Windows.Forms.GroupBox GBoxOthers;
        private System.Windows.Forms.CheckBox CheckBoxMemClean;
        private System.Windows.Forms.CheckBox CheckBoxUniTopMost;
        private System.Windows.Forms.CheckBox CheckBoxTopMost;
        private System.Windows.Forms.CheckBox CheckBoxStartup;
        private System.Windows.Forms.CheckBox CheckBoxWCCMS;
        private System.Windows.Forms.Label LabelExamInfoWarning;
    }
}