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
            this.GBoxExamInfo = new System.Windows.Forms.GroupBox();
            this.ComboBoxAutoSwitchIntervel = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxAutoSwitch = new System.Windows.Forms.CheckBox();
            this.LabelExamInfo = new System.Windows.Forms.Label();
            this.ButtonExamInfo = new System.Windows.Forms.Button();
            this.GBoxOthers = new System.Windows.Forms.GroupBox();
            this.CheckBoxTrayText = new System.Windows.Forms.CheckBox();
            this.CheckBoxWCCMS = new System.Windows.Forms.CheckBox();
            this.CheckBoxMemClean = new System.Windows.Forms.CheckBox();
            this.CheckBoxTrayIcon = new System.Windows.Forms.CheckBox();
            this.CheckBoxUniTopMost = new System.Windows.Forms.CheckBox();
            this.CheckBoxTopMost = new System.Windows.Forms.CheckBox();
            this.CheckBoxStartup = new System.Windows.Forms.CheckBox();
            this.GBoxPptsvc = new System.Windows.Forms.GroupBox();
            this.CheckBoxPptSvc = new System.Windows.Forms.CheckBox();
            this.LabelPptsvc = new System.Windows.Forms.Label();
            this.GBoxContent = new System.Windows.Forms.GroupBox();
            this.LabelCountdownEnd = new System.Windows.Forms.Label();
            this.ComboBoxCountdownEnd = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxRulesMan = new System.Windows.Forms.CheckBox();
            this.ButtonRulesMan = new System.Windows.Forms.Button();
            this.ComboBoxShowXOnly = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxCeiling = new System.Windows.Forms.CheckBox();
            this.CheckBoxShowXOnly = new System.Windows.Forms.CheckBox();
            this.GBoxDraggable = new System.Windows.Forms.GroupBox();
            this.ComboBoxPosition = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxDraggable = new System.Windows.Forms.CheckBox();
            this.LabelPosition = new System.Windows.Forms.Label();
            this.LabelScreens = new System.Windows.Forms.Label();
            this.ComboBoxScreens = new PlainCEETimer.Controls.ComboBoxEx();
            this.GBoxColors = new System.Windows.Forms.GroupBox();
            this.LabelPreviewColor4 = new System.Windows.Forms.Label();
            this.LabelColor41 = new System.Windows.Forms.Label();
            this.LabelColor42 = new System.Windows.Forms.Label();
            this.LabelColorWelcome = new System.Windows.Forms.Label();
            this.LabelPreviewColor1 = new System.Windows.Forms.Label();
            this.LabelPreviewColor2 = new System.Windows.Forms.Label();
            this.LabelPreviewColor3 = new System.Windows.Forms.Label();
            this.LabelColor = new System.Windows.Forms.Label();
            this.ButtonDefaultColor = new System.Windows.Forms.Button();
            this.LabelColor31 = new System.Windows.Forms.Label();
            this.LabelColor32 = new System.Windows.Forms.Label();
            this.LabelColor21 = new System.Windows.Forms.Label();
            this.LabelColor22 = new System.Windows.Forms.Label();
            this.LabelColor11 = new System.Windows.Forms.Label();
            this.LabelColor12 = new System.Windows.Forms.Label();
            this.LabelColorP3 = new System.Windows.Forms.Label();
            this.LabelColorP2 = new System.Windows.Forms.Label();
            this.LabelColorP1 = new System.Windows.Forms.Label();
            this.GBoxFont = new System.Windows.Forms.GroupBox();
            this.ButtonDefaultFont = new System.Windows.Forms.Button();
            this.ButtonFont = new System.Windows.Forms.Button();
            this.LabelFont = new System.Windows.Forms.Label();
            this.GBoxRestart = new System.Windows.Forms.GroupBox();
            this.LabelRestart = new System.Windows.Forms.Label();
            this.GBoxSyncTime = new System.Windows.Forms.GroupBox();
            this.ComboBoxNtpServers = new PlainCEETimer.Controls.ComboBoxEx();
            this.LabelSyncTime = new System.Windows.Forms.Label();
            this.PageNavPages = new System.Windows.Forms.Panel();
            this.PanelNav = new System.Windows.Forms.Panel();
            this.PageGeneral = new PlainCEETimer.Controls.NavigationPage();
            this.PageDisplay = new PlainCEETimer.Controls.NavigationPage();
            this.PageAppearance = new PlainCEETimer.Controls.NavigationPage();
            this.PageTools = new PlainCEETimer.Controls.NavigationPage();
            this.GBoxExamInfo.SuspendLayout();
            this.GBoxOthers.SuspendLayout();
            this.GBoxPptsvc.SuspendLayout();
            this.GBoxContent.SuspendLayout();
            this.GBoxDraggable.SuspendLayout();
            this.GBoxColors.SuspendLayout();
            this.GBoxFont.SuspendLayout();
            this.GBoxRestart.SuspendLayout();
            this.GBoxSyncTime.SuspendLayout();
            this.PageGeneral.SuspendLayout();
            this.PageDisplay.SuspendLayout();
            this.PageAppearance.SuspendLayout();
            this.PageTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(314, 262);
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
            this.ButtonSave.Location = new System.Drawing.Point(233, 262);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(75, 25);
            this.ButtonSave.TabIndex = 16;
            this.ButtonSave.Text = "保存(&S)";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // GBoxExamInfo
            // 
            this.GBoxExamInfo.Controls.Add(this.ComboBoxAutoSwitchIntervel);
            this.GBoxExamInfo.Controls.Add(this.CheckBoxAutoSwitch);
            this.GBoxExamInfo.Controls.Add(this.LabelExamInfo);
            this.GBoxExamInfo.Controls.Add(this.ButtonExamInfo);
            this.GBoxExamInfo.Location = new System.Drawing.Point(5, 3);
            this.GBoxExamInfo.Name = "GBoxExamInfo";
            this.GBoxExamInfo.Size = new System.Drawing.Size(323, 67);
            this.GBoxExamInfo.TabIndex = 45;
            this.GBoxExamInfo.TabStop = false;
            this.GBoxExamInfo.Text = "考试信息";
            // 
            // ComboBoxAutoSwitchIntervel
            // 
            this.ComboBoxAutoSwitchIntervel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxAutoSwitchIntervel.Enabled = false;
            this.ComboBoxAutoSwitchIntervel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxAutoSwitchIntervel.Location = new System.Drawing.Point(231, 37);
            this.ComboBoxAutoSwitchIntervel.Name = "ComboBoxAutoSwitchIntervel";
            this.ComboBoxAutoSwitchIntervel.Size = new System.Drawing.Size(86, 23);
            this.ComboBoxAutoSwitchIntervel.TabIndex = 10;
            // 
            // CheckBoxAutoSwitch
            // 
            this.CheckBoxAutoSwitch.AutoSize = true;
            this.CheckBoxAutoSwitch.Location = new System.Drawing.Point(125, 40);
            this.CheckBoxAutoSwitch.Name = "CheckBoxAutoSwitch";
            this.CheckBoxAutoSwitch.Size = new System.Drawing.Size(104, 19);
            this.CheckBoxAutoSwitch.TabIndex = 9;
            this.CheckBoxAutoSwitch.Text = "启用自动切换";
            this.CheckBoxAutoSwitch.UseVisualStyleBackColor = true;
            this.CheckBoxAutoSwitch.CheckedChanged += new System.EventHandler(this.CheckBoxAutoSwitch_CheckedChanged);
            // 
            // LabelExamInfo
            // 
            this.LabelExamInfo.AutoSize = true;
            this.LabelExamInfo.Location = new System.Drawing.Point(6, 19);
            this.LabelExamInfo.Name = "LabelExamInfo";
            this.LabelExamInfo.Size = new System.Drawing.Size(202, 15);
            this.LabelExamInfo.TabIndex = 4;
            this.LabelExamInfo.Text = "在此添加考试信息以启动倒计时。";
            // 
            // ButtonExamInfo
            // 
            this.ButtonExamInfo.Location = new System.Drawing.Point(8, 37);
            this.ButtonExamInfo.Name = "ButtonExamInfo";
            this.ButtonExamInfo.Size = new System.Drawing.Size(75, 23);
            this.ButtonExamInfo.TabIndex = 3;
            this.ButtonExamInfo.Text = "管理(&G)";
            this.ButtonExamInfo.UseVisualStyleBackColor = true;
            this.ButtonExamInfo.Click += new System.EventHandler(this.ButtonExamInfo_Click);
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
            this.GBoxOthers.Location = new System.Drawing.Point(5, 76);
            this.GBoxOthers.Name = "GBoxOthers";
            this.GBoxOthers.Size = new System.Drawing.Size(323, 167);
            this.GBoxOthers.TabIndex = 44;
            this.GBoxOthers.TabStop = false;
            this.GBoxOthers.Text = "其他";
            // 
            // CheckBoxTrayText
            // 
            this.CheckBoxTrayText.AutoSize = true;
            this.CheckBoxTrayText.Enabled = false;
            this.CheckBoxTrayText.Location = new System.Drawing.Point(9, 119);
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
            this.CheckBoxWCCMS.Location = new System.Drawing.Point(9, 144);
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
            this.CheckBoxMemClean.Location = new System.Drawing.Point(9, 44);
            this.CheckBoxMemClean.Name = "CheckBoxMemClean";
            this.CheckBoxMemClean.Size = new System.Drawing.Size(227, 19);
            this.CheckBoxMemClean.TabIndex = 2;
            this.CheckBoxMemClean.Text = "自动清理倒计时占用的运行内存(&M)";
            this.CheckBoxMemClean.UseVisualStyleBackColor = true;
            this.CheckBoxMemClean.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxTrayIcon
            // 
            this.CheckBoxTrayIcon.AutoSize = true;
            this.CheckBoxTrayIcon.Location = new System.Drawing.Point(9, 94);
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
            this.CheckBoxUniTopMost.Location = new System.Drawing.Point(157, 69);
            this.CheckBoxUniTopMost.Name = "CheckBoxUniTopMost";
            this.CheckBoxUniTopMost.Size = new System.Drawing.Size(120, 19);
            this.CheckBoxUniTopMost.TabIndex = 4;
            this.CheckBoxUniTopMost.Text = "顶置其他窗口(&U)";
            this.CheckBoxUniTopMost.UseVisualStyleBackColor = true;
            this.CheckBoxUniTopMost.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxTopMost
            // 
            this.CheckBoxTopMost.AutoSize = true;
            this.CheckBoxTopMost.Checked = true;
            this.CheckBoxTopMost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxTopMost.Location = new System.Drawing.Point(9, 69);
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
            this.CheckBoxStartup.Size = new System.Drawing.Size(197, 19);
            this.CheckBoxStartup.TabIndex = 18;
            this.CheckBoxStartup.Text = "系统运行时自动启动倒计时(&B)";
            this.CheckBoxStartup.UseVisualStyleBackColor = true;
            this.CheckBoxStartup.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // GBoxPptsvc
            // 
            this.GBoxPptsvc.Controls.Add(this.CheckBoxPptSvc);
            this.GBoxPptsvc.Controls.Add(this.LabelPptsvc);
            this.GBoxPptsvc.Location = new System.Drawing.Point(5, 176);
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
            this.LabelPptsvc.Size = new System.Drawing.Size(358, 15);
            this.LabelPptsvc.TabIndex = 0;
            this.LabelPptsvc.Text = "用于解决个别机型在内置白板打开后底部工具栏消失的情况。";
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
            this.GBoxContent.Location = new System.Drawing.Point(5, 3);
            this.GBoxContent.Name = "GBoxContent";
            this.GBoxContent.Size = new System.Drawing.Size(323, 98);
            this.GBoxContent.TabIndex = 1;
            this.GBoxContent.TabStop = false;
            this.GBoxContent.Text = "倒计时内容";
            // 
            // LabelCountdownEnd
            // 
            this.LabelCountdownEnd.AutoSize = true;
            this.LabelCountdownEnd.Location = new System.Drawing.Point(7, 19);
            this.LabelCountdownEnd.Name = "LabelCountdownEnd";
            this.LabelCountdownEnd.Size = new System.Drawing.Size(117, 15);
            this.LabelCountdownEnd.TabIndex = 45;
            this.LabelCountdownEnd.Text = "当考试开始后, 显示";
            // 
            // ComboBoxCountdownEnd
            // 
            this.ComboBoxCountdownEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCountdownEnd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxCountdownEnd.Location = new System.Drawing.Point(126, 15);
            this.ComboBoxCountdownEnd.MaxDropDownItems = 1;
            this.ComboBoxCountdownEnd.Name = "ComboBoxCountdownEnd";
            this.ComboBoxCountdownEnd.Size = new System.Drawing.Size(190, 23);
            this.ComboBoxCountdownEnd.TabIndex = 44;
            this.ComboBoxCountdownEnd.SelectedIndexChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxRulesMan
            // 
            this.CheckBoxRulesMan.AutoSize = true;
            this.CheckBoxRulesMan.Location = new System.Drawing.Point(9, 72);
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
            this.ButtonRulesMan.Location = new System.Drawing.Point(219, 68);
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
            this.ComboBoxShowXOnly.Location = new System.Drawing.Point(72, 43);
            this.ComboBoxShowXOnly.MaxDropDownItems = 1;
            this.ComboBoxShowXOnly.Name = "ComboBoxShowXOnly";
            this.ComboBoxShowXOnly.Size = new System.Drawing.Size(38, 23);
            this.ComboBoxShowXOnly.TabIndex = 5;
            this.ComboBoxShowXOnly.SelectedIndexChanged += new System.EventHandler(this.ComboBoxShowXOnly_SelectedIndexChanged);
            // 
            // CheckBoxCeiling
            // 
            this.CheckBoxCeiling.AutoSize = true;
            this.CheckBoxCeiling.Enabled = false;
            this.CheckBoxCeiling.Location = new System.Drawing.Point(117, 46);
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
            this.CheckBoxShowXOnly.Location = new System.Drawing.Point(9, 46);
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
            this.GBoxDraggable.Controls.Add(this.LabelPosition);
            this.GBoxDraggable.Controls.Add(this.LabelScreens);
            this.GBoxDraggable.Controls.Add(this.ComboBoxScreens);
            this.GBoxDraggable.Location = new System.Drawing.Point(5, 103);
            this.GBoxDraggable.Name = "GBoxDraggable";
            this.GBoxDraggable.Size = new System.Drawing.Size(323, 71);
            this.GBoxDraggable.TabIndex = 4;
            this.GBoxDraggable.TabStop = false;
            this.GBoxDraggable.Text = "多显示器与拖动";
            // 
            // ComboBoxPosition
            // 
            this.ComboBoxPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPosition.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxPosition.Location = new System.Drawing.Point(232, 17);
            this.ComboBoxPosition.Name = "ComboBoxPosition";
            this.ComboBoxPosition.Size = new System.Drawing.Size(84, 23);
            this.ComboBoxPosition.TabIndex = 7;
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
            // LabelPosition
            // 
            this.LabelPosition.AutoSize = true;
            this.LabelPosition.Location = new System.Drawing.Point(193, 21);
            this.LabelPosition.Name = "LabelPosition";
            this.LabelPosition.Size = new System.Drawing.Size(33, 15);
            this.LabelPosition.TabIndex = 6;
            this.LabelPosition.Text = "位置";
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
            this.ComboBoxScreens.SelectedIndexChanged += new System.EventHandler(this.ComboBoxScreens_SelectedIndexChanged);
            // 
            // GBoxColors
            // 
            this.GBoxColors.Controls.Add(this.LabelPreviewColor4);
            this.GBoxColors.Controls.Add(this.LabelColor41);
            this.GBoxColors.Controls.Add(this.LabelColor42);
            this.GBoxColors.Controls.Add(this.LabelColorWelcome);
            this.GBoxColors.Controls.Add(this.LabelPreviewColor1);
            this.GBoxColors.Controls.Add(this.LabelPreviewColor2);
            this.GBoxColors.Controls.Add(this.LabelPreviewColor3);
            this.GBoxColors.Controls.Add(this.LabelColor);
            this.GBoxColors.Controls.Add(this.ButtonDefaultColor);
            this.GBoxColors.Controls.Add(this.LabelColor31);
            this.GBoxColors.Controls.Add(this.LabelColor32);
            this.GBoxColors.Controls.Add(this.LabelColor21);
            this.GBoxColors.Controls.Add(this.LabelColor22);
            this.GBoxColors.Controls.Add(this.LabelColor11);
            this.GBoxColors.Controls.Add(this.LabelColor12);
            this.GBoxColors.Controls.Add(this.LabelColorP3);
            this.GBoxColors.Controls.Add(this.LabelColorP2);
            this.GBoxColors.Controls.Add(this.LabelColorP1);
            this.GBoxColors.Location = new System.Drawing.Point(5, 77);
            this.GBoxColors.Name = "GBoxColors";
            this.GBoxColors.Size = new System.Drawing.Size(323, 168);
            this.GBoxColors.TabIndex = 0;
            this.GBoxColors.TabStop = false;
            this.GBoxColors.Text = "字体颜色";
            // 
            // LabelPreviewColor4
            // 
            this.LabelPreviewColor4.AutoSize = true;
            this.LabelPreviewColor4.Location = new System.Drawing.Point(174, 118);
            this.LabelPreviewColor4.Name = "LabelPreviewColor4";
            this.LabelPreviewColor4.Size = new System.Drawing.Size(68, 15);
            this.LabelPreviewColor4.TabIndex = 19;
            this.LabelPreviewColor4.Text = "欢迎使用...";
            // 
            // LabelColor41
            // 
            this.LabelColor41.AutoSize = true;
            this.LabelColor41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor41.Location = new System.Drawing.Point(84, 118);
            this.LabelColor41.Name = "LabelColor41";
            this.LabelColor41.Size = new System.Drawing.Size(39, 17);
            this.LabelColor41.TabIndex = 18;
            this.LabelColor41.Text = "          ";
            // 
            // LabelColor42
            // 
            this.LabelColor42.AutoSize = true;
            this.LabelColor42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor42.Location = new System.Drawing.Point(129, 118);
            this.LabelColor42.Name = "LabelColor42";
            this.LabelColor42.Size = new System.Drawing.Size(39, 17);
            this.LabelColor42.TabIndex = 17;
            this.LabelColor42.Text = "          ";
            // 
            // LabelColorWelcome
            // 
            this.LabelColorWelcome.AutoSize = true;
            this.LabelColorWelcome.Location = new System.Drawing.Point(6, 119);
            this.LabelColorWelcome.Name = "LabelColorWelcome";
            this.LabelColorWelcome.Size = new System.Drawing.Size(73, 15);
            this.LabelColorWelcome.TabIndex = 16;
            this.LabelColorWelcome.Text = "[4]欢迎信息";
            // 
            // LabelPreviewColor1
            // 
            this.LabelPreviewColor1.AutoSize = true;
            this.LabelPreviewColor1.Location = new System.Drawing.Point(174, 53);
            this.LabelPreviewColor1.Name = "LabelPreviewColor1";
            this.LabelPreviewColor1.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor1.TabIndex = 15;
            // 
            // LabelPreviewColor2
            // 
            this.LabelPreviewColor2.AutoSize = true;
            this.LabelPreviewColor2.Location = new System.Drawing.Point(174, 75);
            this.LabelPreviewColor2.Name = "LabelPreviewColor2";
            this.LabelPreviewColor2.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor2.TabIndex = 14;
            // 
            // LabelPreviewColor3
            // 
            this.LabelPreviewColor3.AutoSize = true;
            this.LabelPreviewColor3.Location = new System.Drawing.Point(174, 97);
            this.LabelPreviewColor3.Name = "LabelPreviewColor3";
            this.LabelPreviewColor3.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor3.TabIndex = 13;
            // 
            // LabelColor
            // 
            this.LabelColor.AutoSize = true;
            this.LabelColor.Location = new System.Drawing.Point(6, 19);
            this.LabelColor.Name = "LabelColor";
            this.LabelColor.Size = new System.Drawing.Size(514, 15);
            this.LabelColor.TabIndex = 12;
            this.LabelColor.Text = "点击色块来选择文字、背景颜色。将一个色块拖放到其它色块上可快速应用相同的颜色。";
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
            this.LabelColor31.Location = new System.Drawing.Point(84, 96);
            this.LabelColor31.Name = "LabelColor31";
            this.LabelColor31.Size = new System.Drawing.Size(39, 17);
            this.LabelColor31.TabIndex = 9;
            this.LabelColor31.Text = "          ";
            // 
            // LabelColor32
            // 
            this.LabelColor32.AutoSize = true;
            this.LabelColor32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor32.Location = new System.Drawing.Point(129, 96);
            this.LabelColor32.Name = "LabelColor32";
            this.LabelColor32.Size = new System.Drawing.Size(39, 17);
            this.LabelColor32.TabIndex = 8;
            this.LabelColor32.Text = "          ";
            // 
            // LabelColor21
            // 
            this.LabelColor21.AutoSize = true;
            this.LabelColor21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor21.Location = new System.Drawing.Point(84, 74);
            this.LabelColor21.Name = "LabelColor21";
            this.LabelColor21.Size = new System.Drawing.Size(39, 17);
            this.LabelColor21.TabIndex = 7;
            this.LabelColor21.Text = "          ";
            // 
            // LabelColor22
            // 
            this.LabelColor22.AutoSize = true;
            this.LabelColor22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor22.Location = new System.Drawing.Point(129, 74);
            this.LabelColor22.Name = "LabelColor22";
            this.LabelColor22.Size = new System.Drawing.Size(39, 17);
            this.LabelColor22.TabIndex = 6;
            this.LabelColor22.Text = "          ";
            // 
            // LabelColor11
            // 
            this.LabelColor11.AutoSize = true;
            this.LabelColor11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor11.Location = new System.Drawing.Point(84, 52);
            this.LabelColor11.Name = "LabelColor11";
            this.LabelColor11.Size = new System.Drawing.Size(39, 17);
            this.LabelColor11.TabIndex = 5;
            this.LabelColor11.Text = "          ";
            // 
            // LabelColor12
            // 
            this.LabelColor12.AutoSize = true;
            this.LabelColor12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor12.Location = new System.Drawing.Point(129, 52);
            this.LabelColor12.Name = "LabelColor12";
            this.LabelColor12.Size = new System.Drawing.Size(39, 17);
            this.LabelColor12.TabIndex = 0;
            this.LabelColor12.Text = "          ";
            // 
            // LabelColorP3
            // 
            this.LabelColorP3.AutoSize = true;
            this.LabelColorP3.Location = new System.Drawing.Point(6, 97);
            this.LabelColorP3.Name = "LabelColorP3";
            this.LabelColorP3.Size = new System.Drawing.Size(60, 15);
            this.LabelColorP3.TabIndex = 4;
            this.LabelColorP3.Text = "[3]考试后";
            // 
            // LabelColorP2
            // 
            this.LabelColorP2.AutoSize = true;
            this.LabelColorP2.Location = new System.Drawing.Point(6, 75);
            this.LabelColorP2.Name = "LabelColorP2";
            this.LabelColorP2.Size = new System.Drawing.Size(60, 15);
            this.LabelColorP2.TabIndex = 3;
            this.LabelColorP2.Text = "[2]考试中";
            // 
            // LabelColorP1
            // 
            this.LabelColorP1.AutoSize = true;
            this.LabelColorP1.Location = new System.Drawing.Point(6, 53);
            this.LabelColorP1.Name = "LabelColorP1";
            this.LabelColorP1.Size = new System.Drawing.Size(60, 15);
            this.LabelColorP1.TabIndex = 2;
            this.LabelColorP1.Text = "[1]考试前";
            // 
            // GBoxFont
            // 
            this.GBoxFont.Controls.Add(this.ButtonDefaultFont);
            this.GBoxFont.Controls.Add(this.ButtonFont);
            this.GBoxFont.Controls.Add(this.LabelFont);
            this.GBoxFont.Location = new System.Drawing.Point(5, 3);
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
            // GBoxRestart
            // 
            this.GBoxRestart.Controls.Add(this.LabelRestart);
            this.GBoxRestart.Controls.Add(this.ButtonRestart);
            this.GBoxRestart.Location = new System.Drawing.Point(5, 92);
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
            this.GBoxSyncTime.Location = new System.Drawing.Point(5, 3);
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
            this.ComboBoxNtpServers.SelectedIndexChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // LabelSyncTime
            // 
            this.LabelSyncTime.AutoSize = true;
            this.LabelSyncTime.Location = new System.Drawing.Point(6, 19);
            this.LabelSyncTime.Name = "LabelSyncTime";
            this.LabelSyncTime.Size = new System.Drawing.Size(439, 15);
            this.LabelSyncTime.TabIndex = 20;
            this.LabelSyncTime.Text = "将尝试自动启动 Windows Time 服务, 并设置默认 NTP 服务器然后与之同步。";
            // 
            // PageNavPages
            // 
            this.PageNavPages.Location = new System.Drawing.Point(57, 0);
            this.PageNavPages.Name = "PageNavPages";
            this.PageNavPages.Size = new System.Drawing.Size(332, 259);
            this.PageNavPages.TabIndex = 42;
            // 
            // PanelNav
            // 
            this.PanelNav.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PanelNav.Location = new System.Drawing.Point(3, 0);
            this.PanelNav.Name = "PanelNav";
            this.PanelNav.Size = new System.Drawing.Size(53, 259);
            this.PanelNav.TabIndex = 41;
            // 
            // PageGeneral
            // 
            this.PageGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.PageGeneral.Controls.Add(this.GBoxExamInfo);
            this.PageGeneral.Controls.Add(this.GBoxOthers);
            this.PageGeneral.Index = 0;
            this.PageGeneral.Location = new System.Drawing.Point(400, 0);
            this.PageGeneral.Name = "PageGeneral";
            this.PageGeneral.Size = new System.Drawing.Size(332, 254);
            this.PageGeneral.TabIndex = 43;
            this.PageGeneral.Visible = false;
            // 
            // PageDisplay
            // 
            this.PageDisplay.BackColor = System.Drawing.SystemColors.Window;
            this.PageDisplay.Controls.Add(this.GBoxPptsvc);
            this.PageDisplay.Controls.Add(this.GBoxContent);
            this.PageDisplay.Controls.Add(this.GBoxDraggable);
            this.PageDisplay.Index = 0;
            this.PageDisplay.Location = new System.Drawing.Point(738, 0);
            this.PageDisplay.Name = "PageDisplay";
            this.PageDisplay.Size = new System.Drawing.Size(332, 261);
            this.PageDisplay.TabIndex = 44;
            this.PageDisplay.Visible = false;
            // 
            // PageAppearance
            // 
            this.PageAppearance.BackColor = System.Drawing.SystemColors.Window;
            this.PageAppearance.Controls.Add(this.GBoxColors);
            this.PageAppearance.Controls.Add(this.GBoxFont);
            this.PageAppearance.Index = 0;
            this.PageAppearance.Location = new System.Drawing.Point(400, 262);
            this.PageAppearance.Name = "PageAppearance";
            this.PageAppearance.Size = new System.Drawing.Size(332, 254);
            this.PageAppearance.TabIndex = 45;
            this.PageAppearance.Visible = false;
            // 
            // PageTools
            // 
            this.PageTools.BackColor = System.Drawing.SystemColors.Window;
            this.PageTools.Controls.Add(this.GBoxRestart);
            this.PageTools.Controls.Add(this.GBoxSyncTime);
            this.PageTools.Index = 0;
            this.PageTools.Location = new System.Drawing.Point(738, 267);
            this.PageTools.Name = "PageTools";
            this.PageTools.Size = new System.Drawing.Size(332, 185);
            this.PageTools.TabIndex = 45;
            this.PageTools.Visible = false;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.ButtonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(391, 290);
            this.Controls.Add(this.PageAppearance);
            this.Controls.Add(this.PageTools);
            this.Controls.Add(this.PageDisplay);
            this.Controls.Add(this.PageGeneral);
            this.Controls.Add(this.PageNavPages);
            this.Controls.Add(this.PanelNav);
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
            this.GBoxExamInfo.ResumeLayout(false);
            this.GBoxExamInfo.PerformLayout();
            this.GBoxOthers.ResumeLayout(false);
            this.GBoxOthers.PerformLayout();
            this.GBoxPptsvc.ResumeLayout(false);
            this.GBoxPptsvc.PerformLayout();
            this.GBoxContent.ResumeLayout(false);
            this.GBoxContent.PerformLayout();
            this.GBoxDraggable.ResumeLayout(false);
            this.GBoxDraggable.PerformLayout();
            this.GBoxColors.ResumeLayout(false);
            this.GBoxColors.PerformLayout();
            this.GBoxFont.ResumeLayout(false);
            this.GBoxFont.PerformLayout();
            this.GBoxRestart.ResumeLayout(false);
            this.GBoxRestart.PerformLayout();
            this.GBoxSyncTime.ResumeLayout(false);
            this.GBoxSyncTime.PerformLayout();
            this.PageGeneral.ResumeLayout(false);
            this.PageDisplay.ResumeLayout(false);
            this.PageAppearance.ResumeLayout(false);
            this.PageTools.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonSyncTime;
        private System.Windows.Forms.Button ButtonRestart;
        private System.Windows.Forms.Button ButtonSave;
        private System.Windows.Forms.GroupBox GBoxRestart;
        private System.Windows.Forms.GroupBox GBoxSyncTime;
        private System.Windows.Forms.Label LabelRestart;
        private System.Windows.Forms.Label LabelSyncTime;
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
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxScreens;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxShowXOnly;
        private System.Windows.Forms.GroupBox GBoxColors;
        private System.Windows.Forms.Label LabelColor31;
        private System.Windows.Forms.Label LabelColor32;
        private System.Windows.Forms.Label LabelColor21;
        private System.Windows.Forms.Label LabelColor22;
        private System.Windows.Forms.Label LabelColor11;
        private System.Windows.Forms.Label LabelColor12;
        private System.Windows.Forms.Label LabelColorP3;
        private System.Windows.Forms.Label LabelColorP2;
        private System.Windows.Forms.Label LabelColorP1;
        private System.Windows.Forms.Button ButtonDefaultColor;
        private System.Windows.Forms.Label LabelColor;
        private System.Windows.Forms.Label LabelPreviewColor1;
        private System.Windows.Forms.Label LabelPreviewColor2;
        private System.Windows.Forms.Label LabelPreviewColor3;
        private System.Windows.Forms.Label LabelPreviewColor4;
        private System.Windows.Forms.Label LabelColor41;
        private System.Windows.Forms.Label LabelColor42;
        private System.Windows.Forms.Label LabelColorWelcome;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxPosition;
        private System.Windows.Forms.Label LabelPosition;
        private System.Windows.Forms.Button ButtonRulesMan;
        private System.Windows.Forms.CheckBox CheckBoxRulesMan;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxNtpServers;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxCountdownEnd;
        private System.Windows.Forms.Label LabelCountdownEnd;
        private System.Windows.Forms.CheckBox CheckBoxTrayText;
        private System.Windows.Forms.CheckBox CheckBoxTrayIcon;
        private System.Windows.Forms.GroupBox GBoxExamInfo;
        private System.Windows.Forms.Button ButtonExamInfo;
        private System.Windows.Forms.GroupBox GBoxOthers;
        private System.Windows.Forms.CheckBox CheckBoxMemClean;
        private System.Windows.Forms.CheckBox CheckBoxUniTopMost;
        private System.Windows.Forms.CheckBox CheckBoxTopMost;
        private System.Windows.Forms.CheckBox CheckBoxStartup;
        private System.Windows.Forms.CheckBox CheckBoxWCCMS;
        private System.Windows.Forms.Label LabelExamInfo;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxAutoSwitchIntervel;
        private System.Windows.Forms.CheckBox CheckBoxAutoSwitch;
        private System.Windows.Forms.Panel PageNavPages;
        private System.Windows.Forms.Panel PanelNav;
        private Controls.NavigationPage PageGeneral;
        private Controls.NavigationPage PageDisplay;
        private Controls.NavigationPage PageAppearance;
        private Controls.NavigationPage PageTools;
    }
}