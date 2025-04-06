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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("基本");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("显示");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("外观");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("工具");
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonSyncTime = new System.Windows.Forms.Button();
            this.ButtonRestart = new System.Windows.Forms.Button();
            this.ButtonSave = new System.Windows.Forms.Button();
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
            this.ButtonDefaultFont = new System.Windows.Forms.Button();
            this.ButtonFont = new System.Windows.Forms.Button();
            this.LabelFont = new System.Windows.Forms.Label();
            this.LabelRestart = new System.Windows.Forms.Label();
            this.ComboBoxNtpServers = new PlainCEETimer.Controls.ComboBoxEx();
            this.LabelSyncTime = new System.Windows.Forms.Label();
            this.ComboBoxAutoSwitchIntervel = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxAutoSwitch = new System.Windows.Forms.CheckBox();
            this.LabelExamInfo = new System.Windows.Forms.Label();
            this.ButtonExamInfo = new System.Windows.Forms.Button();
            this.CheckBoxTrayText = new System.Windows.Forms.CheckBox();
            this.CheckBoxWCCMS = new System.Windows.Forms.CheckBox();
            this.CheckBoxMemClean = new System.Windows.Forms.CheckBox();
            this.CheckBoxTrayIcon = new System.Windows.Forms.CheckBox();
            this.CheckBoxUniTopMost = new System.Windows.Forms.CheckBox();
            this.CheckBoxTopMost = new System.Windows.Forms.CheckBox();
            this.CheckBoxStartup = new System.Windows.Forms.CheckBox();
            this.CheckBoxPptSvc = new System.Windows.Forms.CheckBox();
            this.LabelCountdownEnd = new System.Windows.Forms.Label();
            this.ComboBoxCountdownEnd = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxRulesMan = new System.Windows.Forms.CheckBox();
            this.ButtonRulesMan = new System.Windows.Forms.Button();
            this.ComboBoxShowXOnly = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxCeiling = new System.Windows.Forms.CheckBox();
            this.CheckBoxShowXOnly = new System.Windows.Forms.CheckBox();
            this.ComboBoxPosition = new PlainCEETimer.Controls.ComboBoxEx();
            this.CheckBoxDraggable = new System.Windows.Forms.CheckBox();
            this.LabelPosition = new System.Windows.Forms.Label();
            this.LabelScreens = new System.Windows.Forms.Label();
            this.ComboBoxScreens = new PlainCEETimer.Controls.ComboBoxEx();
            this.SplitContainerMain = new System.Windows.Forms.SplitContainer();
            this.NavBar = new PlainCEETimer.Controls.NavigationBar();
            this.PageGeneral = new PlainCEETimer.Controls.Page();
            this.PageDisplay = new PlainCEETimer.Controls.Page();
            this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.PageAppearance = new PlainCEETimer.Controls.Page();
            this.PageTools = new PlainCEETimer.Controls.Page();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerMain)).BeginInit();
            this.SplitContainerMain.Panel1.SuspendLayout();
            this.SplitContainerMain.SuspendLayout();
            this.PageGeneral.SuspendLayout();
            this.PageDisplay.SuspendLayout();
            this.PageAppearance.SuspendLayout();
            this.PageTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(258, 204);
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
            this.ButtonSyncTime.Location = new System.Drawing.Point(142, 24);
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
            this.ButtonRestart.Location = new System.Drawing.Point(6, 67);
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
            this.ButtonSave.Location = new System.Drawing.Point(177, 204);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(75, 25);
            this.ButtonSave.TabIndex = 16;
            this.ButtonSave.Text = "保存(&S)";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // LabelPreviewColor4
            // 
            this.LabelPreviewColor4.AutoSize = true;
            this.LabelPreviewColor4.Location = new System.Drawing.Point(169, 151);
            this.LabelPreviewColor4.Name = "LabelPreviewColor4";
            this.LabelPreviewColor4.Size = new System.Drawing.Size(68, 15);
            this.LabelPreviewColor4.TabIndex = 19;
            this.LabelPreviewColor4.Text = "欢迎使用...";
            // 
            // LabelColor41
            // 
            this.LabelColor41.AutoSize = true;
            this.LabelColor41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor41.Location = new System.Drawing.Point(79, 151);
            this.LabelColor41.Name = "LabelColor41";
            this.LabelColor41.Size = new System.Drawing.Size(39, 17);
            this.LabelColor41.TabIndex = 18;
            this.LabelColor41.Text = "          ";
            // 
            // LabelColor42
            // 
            this.LabelColor42.AutoSize = true;
            this.LabelColor42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor42.Location = new System.Drawing.Point(124, 151);
            this.LabelColor42.Name = "LabelColor42";
            this.LabelColor42.Size = new System.Drawing.Size(39, 17);
            this.LabelColor42.TabIndex = 17;
            this.LabelColor42.Text = "          ";
            // 
            // LabelLine05
            // 
            this.LabelLine05.AutoSize = true;
            this.LabelLine05.Location = new System.Drawing.Point(3, 152);
            this.LabelLine05.Name = "LabelLine05";
            this.LabelLine05.Size = new System.Drawing.Size(73, 15);
            this.LabelLine05.TabIndex = 16;
            this.LabelLine05.Text = "[4]欢迎信息";
            // 
            // LabelPreviewColor1
            // 
            this.LabelPreviewColor1.AutoSize = true;
            this.LabelPreviewColor1.Location = new System.Drawing.Point(169, 86);
            this.LabelPreviewColor1.Name = "LabelPreviewColor1";
            this.LabelPreviewColor1.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor1.TabIndex = 15;
            // 
            // LabelPreviewColor2
            // 
            this.LabelPreviewColor2.AutoSize = true;
            this.LabelPreviewColor2.Location = new System.Drawing.Point(169, 108);
            this.LabelPreviewColor2.Name = "LabelPreviewColor2";
            this.LabelPreviewColor2.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor2.TabIndex = 14;
            // 
            // LabelPreviewColor3
            // 
            this.LabelPreviewColor3.AutoSize = true;
            this.LabelPreviewColor3.Location = new System.Drawing.Point(169, 130);
            this.LabelPreviewColor3.Name = "LabelPreviewColor3";
            this.LabelPreviewColor3.Size = new System.Drawing.Size(0, 15);
            this.LabelPreviewColor3.TabIndex = 13;
            // 
            // LabelLine01
            // 
            this.LabelLine01.AutoSize = true;
            this.LabelLine01.Location = new System.Drawing.Point(3, 52);
            this.LabelLine01.Name = "LabelLine01";
            this.LabelLine01.Size = new System.Drawing.Size(520, 15);
            this.LabelLine01.TabIndex = 12;
            this.LabelLine01.Text = "颜色: 点击色块以选择文字、背景颜色。将色块拖放到其它色块上可快速应用相同的颜色。";
            // 
            // ButtonDefaultColor
            // 
            this.ButtonDefaultColor.AutoSize = true;
            this.ButtonDefaultColor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonDefaultColor.Location = new System.Drawing.Point(6, 170);
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
            this.LabelColor31.Location = new System.Drawing.Point(79, 129);
            this.LabelColor31.Name = "LabelColor31";
            this.LabelColor31.Size = new System.Drawing.Size(39, 17);
            this.LabelColor31.TabIndex = 9;
            this.LabelColor31.Text = "          ";
            // 
            // LabelColor32
            // 
            this.LabelColor32.AutoSize = true;
            this.LabelColor32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor32.Location = new System.Drawing.Point(124, 129);
            this.LabelColor32.Name = "LabelColor32";
            this.LabelColor32.Size = new System.Drawing.Size(39, 17);
            this.LabelColor32.TabIndex = 8;
            this.LabelColor32.Text = "          ";
            // 
            // LabelColor21
            // 
            this.LabelColor21.AutoSize = true;
            this.LabelColor21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor21.Location = new System.Drawing.Point(79, 107);
            this.LabelColor21.Name = "LabelColor21";
            this.LabelColor21.Size = new System.Drawing.Size(39, 17);
            this.LabelColor21.TabIndex = 7;
            this.LabelColor21.Text = "          ";
            // 
            // LabelColor22
            // 
            this.LabelColor22.AutoSize = true;
            this.LabelColor22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor22.Location = new System.Drawing.Point(124, 107);
            this.LabelColor22.Name = "LabelColor22";
            this.LabelColor22.Size = new System.Drawing.Size(39, 17);
            this.LabelColor22.TabIndex = 6;
            this.LabelColor22.Text = "          ";
            // 
            // LabelColor11
            // 
            this.LabelColor11.AutoSize = true;
            this.LabelColor11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor11.Location = new System.Drawing.Point(79, 85);
            this.LabelColor11.Name = "LabelColor11";
            this.LabelColor11.Size = new System.Drawing.Size(39, 17);
            this.LabelColor11.TabIndex = 5;
            this.LabelColor11.Text = "          ";
            // 
            // LabelColor12
            // 
            this.LabelColor12.AutoSize = true;
            this.LabelColor12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelColor12.Location = new System.Drawing.Point(124, 85);
            this.LabelColor12.Name = "LabelColor12";
            this.LabelColor12.Size = new System.Drawing.Size(39, 17);
            this.LabelColor12.TabIndex = 0;
            this.LabelColor12.Text = "          ";
            // 
            // LabelLine04
            // 
            this.LabelLine04.AutoSize = true;
            this.LabelLine04.Location = new System.Drawing.Point(3, 130);
            this.LabelLine04.Name = "LabelLine04";
            this.LabelLine04.Size = new System.Drawing.Size(60, 15);
            this.LabelLine04.TabIndex = 4;
            this.LabelLine04.Text = "[3]考试后";
            // 
            // LabelLine03
            // 
            this.LabelLine03.AutoSize = true;
            this.LabelLine03.Location = new System.Drawing.Point(3, 108);
            this.LabelLine03.Name = "LabelLine03";
            this.LabelLine03.Size = new System.Drawing.Size(60, 15);
            this.LabelLine03.TabIndex = 3;
            this.LabelLine03.Text = "[2]考试中";
            // 
            // LabelLine02
            // 
            this.LabelLine02.AutoSize = true;
            this.LabelLine02.Location = new System.Drawing.Point(3, 86);
            this.LabelLine02.Name = "LabelLine02";
            this.LabelLine02.Size = new System.Drawing.Size(60, 15);
            this.LabelLine02.TabIndex = 2;
            this.LabelLine02.Text = "[1]考试前";
            // 
            // ButtonDefaultFont
            // 
            this.ButtonDefaultFont.AutoSize = true;
            this.ButtonDefaultFont.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonDefaultFont.Location = new System.Drawing.Point(95, 24);
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
            this.ButtonFont.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonFont.Location = new System.Drawing.Point(6, 24);
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
            this.LabelFont.Location = new System.Drawing.Point(3, 6);
            this.LabelFont.Name = "LabelFont";
            this.LabelFont.Size = new System.Drawing.Size(0, 15);
            this.LabelFont.TabIndex = 1;
            // 
            // LabelRestart
            // 
            this.LabelRestart.AutoSize = true;
            this.LabelRestart.Location = new System.Drawing.Point(3, 50);
            this.LabelRestart.Name = "LabelRestart";
            this.LabelRestart.Size = new System.Drawing.Size(0, 15);
            this.LabelRestart.TabIndex = 37;
            // 
            // ComboBoxNtpServers
            // 
            this.ComboBoxNtpServers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxNtpServers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxNtpServers.Location = new System.Drawing.Point(6, 24);
            this.ComboBoxNtpServers.MaxDropDownItems = 1;
            this.ComboBoxNtpServers.Name = "ComboBoxNtpServers";
            this.ComboBoxNtpServers.Size = new System.Drawing.Size(130, 23);
            this.ComboBoxNtpServers.TabIndex = 21;
            this.ComboBoxNtpServers.SelectedIndexChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // LabelSyncTime
            // 
            this.LabelSyncTime.AutoSize = true;
            this.LabelSyncTime.Location = new System.Drawing.Point(3, 6);
            this.LabelSyncTime.Name = "LabelSyncTime";
            this.LabelSyncTime.Size = new System.Drawing.Size(497, 15);
            this.LabelSyncTime.TabIndex = 20;
            this.LabelSyncTime.Text = "同步网络时钟: 将尝试自动启动 Windows Time 服务, 并设置默认 NTP 服务器与之同步。";
            // 
            // ComboBoxAutoSwitchIntervel
            // 
            this.ComboBoxAutoSwitchIntervel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxAutoSwitchIntervel.Enabled = false;
            this.ComboBoxAutoSwitchIntervel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxAutoSwitchIntervel.Location = new System.Drawing.Point(112, 27);
            this.ComboBoxAutoSwitchIntervel.Name = "ComboBoxAutoSwitchIntervel";
            this.ComboBoxAutoSwitchIntervel.Size = new System.Drawing.Size(86, 23);
            this.ComboBoxAutoSwitchIntervel.TabIndex = 10;
            // 
            // CheckBoxAutoSwitch
            // 
            this.CheckBoxAutoSwitch.AutoSize = true;
            this.CheckBoxAutoSwitch.Location = new System.Drawing.Point(6, 30);
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
            this.LabelExamInfo.Location = new System.Drawing.Point(3, 6);
            this.LabelExamInfo.Name = "LabelExamInfo";
            this.LabelExamInfo.Size = new System.Drawing.Size(169, 15);
            this.LabelExamInfo.TabIndex = 4;
            this.LabelExamInfo.Text = "添加考试信息以启动倒计时: ";
            // 
            // ButtonExamInfo
            // 
            this.ButtonExamInfo.AutoSize = true;
            this.ButtonExamInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonExamInfo.Location = new System.Drawing.Point(172, 2);
            this.ButtonExamInfo.Name = "ButtonExamInfo";
            this.ButtonExamInfo.Size = new System.Drawing.Size(59, 25);
            this.ButtonExamInfo.TabIndex = 3;
            this.ButtonExamInfo.Text = "管理(&G)";
            this.ButtonExamInfo.UseVisualStyleBackColor = true;
            this.ButtonExamInfo.Click += new System.EventHandler(this.ButtonExamInfo_Click);
            // 
            // CheckBoxTrayText
            // 
            this.CheckBoxTrayText.AutoSize = true;
            this.CheckBoxTrayText.Enabled = false;
            this.CheckBoxTrayText.Location = new System.Drawing.Point(6, 155);
            this.CheckBoxTrayText.Name = "CheckBoxTrayText";
            this.CheckBoxTrayText.Size = new System.Drawing.Size(251, 19);
            this.CheckBoxTrayText.TabIndex = 1;
            this.CheckBoxTrayText.Text = "鼠标悬停在通知图标上时显示倒计时(&N)";
            this.CheckBoxTrayText.UseVisualStyleBackColor = true;
            this.CheckBoxTrayText.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxWCCMS
            // 
            this.CheckBoxWCCMS.AutoSize = true;
            this.CheckBoxWCCMS.Location = new System.Drawing.Point(6, 180);
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
            this.CheckBoxMemClean.Location = new System.Drawing.Point(6, 80);
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
            this.CheckBoxTrayIcon.Location = new System.Drawing.Point(6, 130);
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
            this.CheckBoxUniTopMost.Location = new System.Drawing.Point(154, 105);
            this.CheckBoxUniTopMost.Name = "CheckBoxUniTopMost";
            this.CheckBoxUniTopMost.Size = new System.Drawing.Size(120, 19);
            this.CheckBoxUniTopMost.TabIndex = 4;
            this.CheckBoxUniTopMost.Text = "顶置其它窗口(&U)";
            this.CheckBoxUniTopMost.UseVisualStyleBackColor = true;
            this.CheckBoxUniTopMost.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxTopMost
            // 
            this.CheckBoxTopMost.AutoSize = true;
            this.CheckBoxTopMost.Checked = true;
            this.CheckBoxTopMost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxTopMost.Location = new System.Drawing.Point(6, 105);
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
            this.CheckBoxStartup.Location = new System.Drawing.Point(6, 55);
            this.CheckBoxStartup.Name = "CheckBoxStartup";
            this.CheckBoxStartup.Size = new System.Drawing.Size(210, 19);
            this.CheckBoxStartup.TabIndex = 18;
            this.CheckBoxStartup.Text = "当系统启动时自动运行倒计时(&B)";
            this.CheckBoxStartup.UseVisualStyleBackColor = true;
            this.CheckBoxStartup.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxPptSvc
            // 
            this.CheckBoxPptSvc.AutoSize = true;
            this.CheckBoxPptSvc.Cursor = System.Windows.Forms.Cursors.Help;
            this.CheckBoxPptSvc.Location = new System.Drawing.Point(6, 85);
            this.CheckBoxPptSvc.Name = "CheckBoxPptSvc";
            this.CheckBoxPptSvc.Size = new System.Drawing.Size(153, 19);
            this.CheckBoxPptSvc.TabIndex = 3;
            this.CheckBoxPptSvc.Text = "兼容希沃PPT小工具(&X)";
            this.CheckBoxPptSvc.UseVisualStyleBackColor = true;
            this.CheckBoxPptSvc.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // LabelCountdownEnd
            // 
            this.LabelCountdownEnd.AutoSize = true;
            this.LabelCountdownEnd.Location = new System.Drawing.Point(3, 110);
            this.LabelCountdownEnd.Name = "LabelCountdownEnd";
            this.LabelCountdownEnd.Size = new System.Drawing.Size(117, 15);
            this.LabelCountdownEnd.TabIndex = 45;
            this.LabelCountdownEnd.Text = "当考试开始后, 显示";
            // 
            // ComboBoxCountdownEnd
            // 
            this.ComboBoxCountdownEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCountdownEnd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxCountdownEnd.Location = new System.Drawing.Point(122, 106);
            this.ComboBoxCountdownEnd.MaxDropDownItems = 1;
            this.ComboBoxCountdownEnd.Name = "ComboBoxCountdownEnd";
            this.ComboBoxCountdownEnd.Size = new System.Drawing.Size(150, 23);
            this.ComboBoxCountdownEnd.TabIndex = 44;
            this.ComboBoxCountdownEnd.SelectedIndexChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // CheckBoxRulesMan
            // 
            this.CheckBoxRulesMan.AutoSize = true;
            this.CheckBoxRulesMan.Location = new System.Drawing.Point(6, 35);
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
            this.ButtonRulesMan.Location = new System.Drawing.Point(204, 31);
            this.ButtonRulesMan.Name = "ButtonRulesMan";
            this.ButtonRulesMan.Size = new System.Drawing.Size(58, 25);
            this.ButtonRulesMan.TabIndex = 41;
            this.ButtonRulesMan.Text = "管理(&R)";
            this.ButtonRulesMan.UseVisualStyleBackColor = true;
            this.ButtonRulesMan.Click += new System.EventHandler(this.ButtonRulesMan_Click);
            // 
            // ComboBoxShowXOnly
            // 
            this.ComboBoxShowXOnly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxShowXOnly.Enabled = false;
            this.ComboBoxShowXOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxShowXOnly.Location = new System.Drawing.Point(73, 6);
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
            this.CheckBoxCeiling.Location = new System.Drawing.Point(112, 8);
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
            this.CheckBoxShowXOnly.Location = new System.Drawing.Point(6, 8);
            this.CheckBoxShowXOnly.Name = "CheckBoxShowXOnly";
            this.CheckBoxShowXOnly.Size = new System.Drawing.Size(65, 19);
            this.CheckBoxShowXOnly.TabIndex = 0;
            this.CheckBoxShowXOnly.Text = "只显示";
            this.CheckBoxShowXOnly.UseVisualStyleBackColor = true;
            this.CheckBoxShowXOnly.CheckedChanged += new System.EventHandler(this.CheckBoxShowXOnly_CheckedChanged);
            // 
            // ComboBoxPosition
            // 
            this.ComboBoxPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPosition.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxPosition.Location = new System.Drawing.Point(77, 163);
            this.ComboBoxPosition.Name = "ComboBoxPosition";
            this.ComboBoxPosition.Size = new System.Drawing.Size(95, 23);
            this.ComboBoxPosition.TabIndex = 7;
            this.ComboBoxPosition.SelectedIndexChanged += new System.EventHandler(this.ChangePptsvcStyle);
            // 
            // CheckBoxDraggable
            // 
            this.CheckBoxDraggable.AutoSize = true;
            this.CheckBoxDraggable.Location = new System.Drawing.Point(6, 60);
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
            this.LabelPosition.Location = new System.Drawing.Point(3, 167);
            this.LabelPosition.Name = "LabelPosition";
            this.LabelPosition.Size = new System.Drawing.Size(72, 15);
            this.LabelPosition.TabIndex = 6;
            this.LabelPosition.Text = "固定在位置";
            // 
            // LabelScreens
            // 
            this.LabelScreens.AutoSize = true;
            this.LabelScreens.Location = new System.Drawing.Point(3, 138);
            this.LabelScreens.Name = "LabelScreens";
            this.LabelScreens.Size = new System.Drawing.Size(72, 15);
            this.LabelScreens.TabIndex = 5;
            this.LabelScreens.Text = "固定在屏幕";
            // 
            // ComboBoxScreens
            // 
            this.ComboBoxScreens.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxScreens.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxScreens.Location = new System.Drawing.Point(77, 134);
            this.ComboBoxScreens.MaxDropDownItems = 1;
            this.ComboBoxScreens.Name = "ComboBoxScreens";
            this.ComboBoxScreens.Size = new System.Drawing.Size(170, 23);
            this.ComboBoxScreens.TabIndex = 4;
            this.ComboBoxScreens.SelectedIndexChanged += new System.EventHandler(this.ComboBoxScreens_SelectedIndexChanged);
            // 
            // SplitContainerMain
            // 
            this.SplitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainerMain.IsSplitterFixed = true;
            this.SplitContainerMain.Location = new System.Drawing.Point(3, 0);
            this.SplitContainerMain.Name = "SplitContainerMain";
            // 
            // SplitContainerMain.Panel1
            // 
            this.SplitContainerMain.Panel1.Controls.Add(this.NavBar);
            this.SplitContainerMain.Size = new System.Drawing.Size(330, 200);
            this.SplitContainerMain.SplitterDistance = 53;
            this.SplitContainerMain.SplitterWidth = 1;
            this.SplitContainerMain.TabIndex = 41;
            // 
            // NavBar
            // 
            this.NavBar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NavBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NavBar.FullRowSelect = true;
            this.NavBar.HideSelection = false;
            this.NavBar.HotTracking = true;
            this.NavBar.Indent = 5;
            this.NavBar.ItemHeight = 25;
            this.NavBar.Location = new System.Drawing.Point(0, 0);
            this.NavBar.Name = "NavBar";
            treeNode1.Name = "Node0";
            treeNode1.Text = "基本";
            treeNode2.Name = "Node1";
            treeNode2.Text = "显示";
            treeNode3.Name = "Node2";
            treeNode3.Text = "外观";
            treeNode4.Name = "Node3";
            treeNode4.Text = "工具";
            this.NavBar.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.NavBar.ShowLines = false;
            this.NavBar.Size = new System.Drawing.Size(53, 200);
            this.NavBar.TabIndex = 0;
            this.NavBar.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.NavBar_AfterSelect);
            // 
            // PageGeneral
            // 
            this.PageGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.PageGeneral.Controls.Add(this.ComboBoxAutoSwitchIntervel);
            this.PageGeneral.Controls.Add(this.CheckBoxTrayText);
            this.PageGeneral.Controls.Add(this.CheckBoxStartup);
            this.PageGeneral.Controls.Add(this.CheckBoxWCCMS);
            this.PageGeneral.Controls.Add(this.CheckBoxAutoSwitch);
            this.PageGeneral.Controls.Add(this.CheckBoxTopMost);
            this.PageGeneral.Controls.Add(this.CheckBoxMemClean);
            this.PageGeneral.Controls.Add(this.CheckBoxUniTopMost);
            this.PageGeneral.Controls.Add(this.LabelExamInfo);
            this.PageGeneral.Controls.Add(this.ButtonExamInfo);
            this.PageGeneral.Controls.Add(this.CheckBoxTrayIcon);
            this.PageGeneral.Index = 0;
            this.PageGeneral.Location = new System.Drawing.Point(339, 0);
            this.PageGeneral.Name = "PageGeneral";
            this.PageGeneral.Size = new System.Drawing.Size(276, 211);
            this.PageGeneral.TabIndex = 42;
            this.PageGeneral.Visible = false;
            // 
            // PageDisplay
            // 
            this.PageDisplay.BackColor = System.Drawing.SystemColors.Window;
            this.PageDisplay.Controls.Add(this.CheckBoxPptSvc);
            this.PageDisplay.Controls.Add(this.ComboBoxPosition);
            this.PageDisplay.Controls.Add(this.LabelCountdownEnd);
            this.PageDisplay.Controls.Add(this.CheckBoxDraggable);
            this.PageDisplay.Controls.Add(this.CheckBoxRulesMan);
            this.PageDisplay.Controls.Add(this.LabelPosition);
            this.PageDisplay.Controls.Add(this.ComboBoxCountdownEnd);
            this.PageDisplay.Controls.Add(this.LabelScreens);
            this.PageDisplay.Controls.Add(this.ComboBoxScreens);
            this.PageDisplay.Controls.Add(this.CheckBoxShowXOnly);
            this.PageDisplay.Controls.Add(this.CheckBoxCeiling);
            this.PageDisplay.Controls.Add(this.ButtonRulesMan);
            this.PageDisplay.Controls.Add(this.ComboBoxShowXOnly);
            this.PageDisplay.Index = 1;
            this.PageDisplay.Location = new System.Drawing.Point(339, 217);
            this.PageDisplay.Name = "PageDisplay";
            this.PageDisplay.Size = new System.Drawing.Size(276, 195);
            this.PageDisplay.TabIndex = 43;
            this.PageDisplay.Visible = false;
            // 
            // ToolTipMain
            // 
            this.ToolTipMain.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ToolTipMain.ToolTipTitle = "功能描述";
            // 
            // PageAppearance
            // 
            this.PageAppearance.BackColor = System.Drawing.SystemColors.Window;
            this.PageAppearance.Controls.Add(this.LabelPreviewColor4);
            this.PageAppearance.Controls.Add(this.ButtonDefaultFont);
            this.PageAppearance.Controls.Add(this.LabelColor41);
            this.PageAppearance.Controls.Add(this.ButtonFont);
            this.PageAppearance.Controls.Add(this.LabelColor42);
            this.PageAppearance.Controls.Add(this.LabelFont);
            this.PageAppearance.Controls.Add(this.LabelLine05);
            this.PageAppearance.Controls.Add(this.LabelLine01);
            this.PageAppearance.Controls.Add(this.LabelPreviewColor1);
            this.PageAppearance.Controls.Add(this.LabelLine02);
            this.PageAppearance.Controls.Add(this.LabelPreviewColor2);
            this.PageAppearance.Controls.Add(this.LabelLine03);
            this.PageAppearance.Controls.Add(this.LabelPreviewColor3);
            this.PageAppearance.Controls.Add(this.LabelLine04);
            this.PageAppearance.Controls.Add(this.LabelColor12);
            this.PageAppearance.Controls.Add(this.ButtonDefaultColor);
            this.PageAppearance.Controls.Add(this.LabelColor11);
            this.PageAppearance.Controls.Add(this.LabelColor31);
            this.PageAppearance.Controls.Add(this.LabelColor22);
            this.PageAppearance.Controls.Add(this.LabelColor32);
            this.PageAppearance.Controls.Add(this.LabelColor21);
            this.PageAppearance.Index = 2;
            this.PageAppearance.Location = new System.Drawing.Point(621, 0);
            this.PageAppearance.Name = "PageAppearance";
            this.PageAppearance.Size = new System.Drawing.Size(276, 199);
            this.PageAppearance.TabIndex = 44;
            this.PageAppearance.Visible = false;
            // 
            // PageTools
            // 
            this.PageTools.BackColor = System.Drawing.SystemColors.Window;
            this.PageTools.Controls.Add(this.LabelRestart);
            this.PageTools.Controls.Add(this.LabelSyncTime);
            this.PageTools.Controls.Add(this.ButtonRestart);
            this.PageTools.Controls.Add(this.ButtonSyncTime);
            this.PageTools.Controls.Add(this.ComboBoxNtpServers);
            this.PageTools.Index = 3;
            this.PageTools.Location = new System.Drawing.Point(621, 205);
            this.PageTools.Name = "PageTools";
            this.PageTools.Size = new System.Drawing.Size(276, 108);
            this.PageTools.TabIndex = 45;
            this.PageTools.Visible = false;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.ButtonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(339, 234);
            //this.Controls.Add(this.PageGeneral);
            //this.Controls.Add(this.PageTools);
            //this.Controls.Add(this.PageAppearance);
            //this.Controls.Add(this.PageDisplay);
            this.PageGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PageDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PageAppearance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PageTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.SplitContainerMain);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.ButtonCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "设置 - 高考倒计时";
            this.SplitContainerMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerMain)).EndInit();
            this.SplitContainerMain.ResumeLayout(false);
            this.PageGeneral.ResumeLayout(false);
            this.PageGeneral.PerformLayout();
            this.PageDisplay.ResumeLayout(false);
            this.PageDisplay.PerformLayout();
            this.PageAppearance.ResumeLayout(false);
            this.PageAppearance.PerformLayout();
            this.PageTools.ResumeLayout(false);
            this.PageTools.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonSyncTime;
        private System.Windows.Forms.Button ButtonRestart;
        private System.Windows.Forms.Button ButtonSave;
        private System.Windows.Forms.Label LabelRestart;
        private System.Windows.Forms.Label LabelSyncTime;
        private System.Windows.Forms.Label LabelFont;
        private System.Windows.Forms.Button ButtonFont;
        private System.Windows.Forms.Button ButtonDefaultFont;
        private System.Windows.Forms.CheckBox CheckBoxShowXOnly;
        private System.Windows.Forms.CheckBox CheckBoxCeiling;
        private System.Windows.Forms.CheckBox CheckBoxDraggable;
        private System.Windows.Forms.CheckBox CheckBoxPptSvc;
        private System.Windows.Forms.Label LabelScreens;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxScreens;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxShowXOnly;
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
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxPosition;
        private System.Windows.Forms.Label LabelPosition;
        private System.Windows.Forms.Button ButtonRulesMan;
        private System.Windows.Forms.CheckBox CheckBoxRulesMan;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxNtpServers;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxCountdownEnd;
        private System.Windows.Forms.Label LabelCountdownEnd;
        private System.Windows.Forms.CheckBox CheckBoxTrayText;
        private System.Windows.Forms.CheckBox CheckBoxTrayIcon;
        private System.Windows.Forms.Button ButtonExamInfo;
        private System.Windows.Forms.CheckBox CheckBoxMemClean;
        private System.Windows.Forms.CheckBox CheckBoxUniTopMost;
        private System.Windows.Forms.CheckBox CheckBoxTopMost;
        private System.Windows.Forms.CheckBox CheckBoxStartup;
        private System.Windows.Forms.CheckBox CheckBoxWCCMS;
        private System.Windows.Forms.Label LabelExamInfo;
        private PlainCEETimer.Controls.ComboBoxEx ComboBoxAutoSwitchIntervel;
        private System.Windows.Forms.CheckBox CheckBoxAutoSwitch;
        private System.Windows.Forms.SplitContainer SplitContainerMain;
        private PlainCEETimer.Controls.NavigationBar NavBar;
        private PlainCEETimer.Controls.Page PageGeneral;
        private PlainCEETimer.Controls.Page PageDisplay;
        private System.Windows.Forms.ToolTip ToolTipMain;
        private PlainCEETimer.Controls.Page PageAppearance;
        private PlainCEETimer.Controls.Page PageTools;
    }
}