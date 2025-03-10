using Microsoft.Win32;
using PlainCEETimer.Controls;
using PlainCEETimer.Dialogs;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainCEETimer.Forms
{
    public partial class SettingsForm : AppForm
    {
        private Font SelectedFont;
        private CustomRuleObject[] EditedCustomRules;
        private string[] EditedCustomTexts;
        private bool IsColorLabelsDragging;
        private bool IsSyncingTime;
        private bool HasSettingsChanged;
        private bool InvokeChangeRequired;
        private bool IsFunny;
        private bool IsFunnyClick;
        private bool ChangingCheckBox;
        private readonly bool UseClassicContextMenu = MainForm.UseClassicContextMenu;
        private ContextMenu ContextMenuDefaultColor;
        private ContextMenuStrip ContextMenuStripDefaultColor;
        private Label[] ColorLabels;
        private Label[] ColorPreviewLabels;
        private ColorSetObject[] SelectedColors;
        private readonly ConfigObject AppConfig = App.AppConfig;
        private readonly EventHandler OnItemDarkClick;
        private readonly EventHandler OnItemLightClick;

        public string ExamName { get; set; }
        public DateTime ExamStartTime { get; set; }
        public DateTime ExamEndTime { get; set; }
        public bool RefreshNeeded { get; private set; }

        private bool EIMAutoSwitch;
        private int EIMInterval;

        public SettingsForm()
        {
            InitializeComponent();
            CompositedStyle = true;
            OnItemLightClick = new(MenuItemLight_Click);
            OnItemDarkClick = new(MenuItemDark_Click);
        }

        protected override void OnLoad()
        {
            RefreshNeeded = false;
            InitializeExtra();
            RefreshSettings();
            ChangeWorkingStyle(WorkingArea.LastColor);
            ChangeWorkingStyle(WorkingArea.Funny, false);
        }

        private void InitializeExtra()
        {
            if (UseClassicContextMenu)
            {
                ContextMenuDefaultColor = CreateNew
                ([
                    AddItem("白底(&L)", OnItemLightClick),
                    AddItem("黑底(&D)", OnItemDarkClick)
                ]);
            }
            else
            {
                ContextMenuStripDefaultColor = CreateNewStrip
                ([
                    AddStripItem("白底(&L)", OnItemLightClick),
                    AddStripItem("黑底(&D)", OnItemDarkClick)
                ]);
            }

            LabelPreviewColor1.Text = $"{Placeholders.PH_JULI}...{Placeholders.PH_START}...";
            LabelPreviewColor2.Text = $"{Placeholders.PH_JULI}...{Placeholders.PH_LEFT}...";
            LabelPreviewColor3.Text = $"{Placeholders.PH_JULI}...{Placeholders.PH_PAST}...";

            BindComboData(ComboBoxShowXOnly,
            [
                new("天", 0),
                new("时", 1),
                new("分", 2),
                new("秒", 3)
            ]);

            BindComboData(ComboBoxCountdownEnd,
            [
                new("<程序欢迎信息>", 0),
                new("考试还有多久结束", 1),
                new("考试还有多久结束 和 已过去了多久", 2),
                //new("负数 (仅供娱乐)", 3)
            ]);

            BindComboData(ComboBoxPosition,
            [
                new("左上角", 0),
                new("左侧居中", 1),
                new("左下角", 2),
                new("顶部居中", 3),
                new("中部居中", 4),
                new("底部居中", 5),
                new("右上角", 6),
                new("右侧居中", 7),
                new("右下角", 8)
            ]);

            BindComboData(ComboBoxNtpServers,
            [
                new("time.windows.com", 0),
                new("ntp.aliyun.com", 1),
                new("ntp.tencent.com", 2),
                new("time.cloudflare.com", 3)
            ]);

            var CurrentScreens = Screen.AllScreens;
            var Length = CurrentScreens.Length + 1;
            var Monitors = new ComboData[Length];
            Monitors[0] = new("<请选择>", 0);
            for (int i = 1; i < Length; i++)
            {
                var CurrentScreen = CurrentScreens[i - 1];
                Monitors[i] = new(string.Format("{0} {1} ({2}x{3})", i, CurrentScreen.DeviceName, CurrentScreen.Bounds.Width, CurrentScreen.Bounds.Height), i);
            }
            BindComboData(ComboBoxScreens, Monitors);

            ColorLabels = [LabelColor11, LabelColor21, LabelColor31, LabelColor41, LabelColor12, LabelColor22, LabelColor32, LabelColor42];
            foreach (Label ColorLabel in ColorLabels)
            {
                ColorLabel.Click += ColorLabels_Click;
                ColorLabel.MouseDown += ColorLabels_MouseDown;
                ColorLabel.MouseMove += ColorLabels_MouseMove;
                ColorLabel.MouseUp += ColorLabels_MouseUp;
            }
            ColorPreviewLabels = [LabelPreviewColor1, LabelPreviewColor2, LabelPreviewColor3, LabelPreviewColor4];
        }

        protected override void AdjustUI()
        {
            AlignControlsR(ButtonSave, ButtonCancel, TabControlMain);
            SetLabelAutoWrap(LabelPptsvc, GBoxPptsvc);
            SetLabelAutoWrap(LabelSyncTime, GBoxSyncTime);
            SetLabelAutoWrap(LabelLine01, GBoxColors);
            SetLabelAutoWrap(LabelRestart, GBoxRestart);
            CompactControlsX(ComboBoxShowXOnly, CheckBoxShowXOnly);
            CompactControlsX(CheckBoxCeiling, ComboBoxShowXOnly, 10);
            CompactControlsX(ComboBoxScreens, LabelScreens);
            CompactControlsX(LabelChar1, ComboBoxScreens);
            CompactControlsX(ComboBoxPosition, LabelChar1);
            CompactControlsX(ComboBoxCountdownEnd, LabelCountdownEnd);
            CompactControlsX(ButtonSyncTime, ComboBoxNtpServers, 3);
            CompactControlsY(ButtonSyncTime, LabelSyncTime, 3);
            CompactControlsY(ButtonRestart, LabelRestart, 3);
            CompactControlsY(ButtonExamInfo, LabelExamInfo, 3);

            Adjust(() =>
            {
                AlignControlsX(ComboBoxShowXOnly, CheckBoxShowXOnly, -1);
                AlignControlsX(ComboBoxScreens, LabelScreens);
                AlignControlsX(ComboBoxPosition, LabelChar1);
                AlignControlsX(ButtonExamInfo, LabelExamInfoWarning);
                AlignControlsX(ComboBoxCountdownEnd, LabelCountdownEnd);
                AlignControlsX(ButtonRulesMan, CheckBoxRulesMan);
                AlignControlsX(ComboBoxNtpServers, ButtonSyncTime);
            });
        }

        private void RefreshSettings()
        {
            CheckBoxStartup.Checked = (Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)?.GetValue(App.AppNameEng) is string regvalue) && regvalue.Equals($"\"{App.CurrentExecutablePath}\"", StringComparison.OrdinalIgnoreCase);
            CheckBoxTopMost.Checked = AppConfig.General.TopMost;
            LabelExamInfo.Text = $"考试名称：{ExamName}\n考试开始时间和日期：{ExamStartTime.ToString(App.DateTimeFormat)}\n考试结束时间和日期：{ExamEndTime.ToString(App.DateTimeFormat)}";
            CheckBoxMemClean.Checked = AppConfig.General.MemClean;
            CheckBoxWCCMS.Checked = AppConfig.General.WCCMS;
            CheckBoxDraggable.Checked = AppConfig.Display.Draggable;
            CheckBoxShowXOnly.Checked = AppConfig.Display.ShowXOnly;
            CheckBoxRulesMan.Checked = AppConfig.Display.CustomText;
            CheckBoxCeiling.Checked = AppConfig.Display.Ceiling;
            ComboBoxCountdownEnd.SelectedIndex = AppConfig.Display.EndIndex;
            CheckBoxPptSvc.Checked = AppConfig.Display.SeewoPptsvc;
            CheckBoxUniTopMost.Checked = MainForm.UniTopMost;
            ComboBoxScreens.SelectedIndex = AppConfig.Display.ScreenIndex;
            ComboBoxPosition.SelectedIndex = (int)AppConfig.Display.Position;
            ComboBoxShowXOnly.SelectedIndex = AppConfig.Display.X;
            ChangeWorkingStyle(WorkingArea.ChangeFont, NewFont: AppConfig.Appearance.Font);
            ChangePptsvcStyle(null, EventArgs.Empty);
            ComboBoxShowXOnly.SelectedIndex = AppConfig.Display.ShowXOnly ? AppConfig.Display.X : 0;
            ComboBoxNtpServers.SelectedIndex = AppConfig.Tools.NtpServer;
            EditedCustomTexts = AppConfig.Display.CustomTexts;
            EditedCustomRules = AppConfig.CustomRules;
            CheckBoxTrayText.Enabled = CheckBoxTrayIcon.Checked = AppConfig.General.TrayIcon;
            CheckBoxTrayText.Checked = AppConfig.General.TrayText;
            EIMAutoSwitch = AppConfig.General.AutoSwitch;
            EIMInterval = AppConfig.General.Interval;
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            Execute(() =>
            {
                HasSettingsChanged = true;
                ButtonSave.Enabled = true;
            });
        }

        private void ButtonExamInfo_Click(object sender, EventArgs e)
        {
            ExamInfoManager ExamMan = new()
            {
                AutoSwitch = EIMAutoSwitch,
                PeriodIndex = EIMInterval
            };

            if (ExamMan.ShowDialog() == DialogResult.OK)
            {
                LabelExamInfoWarning.Visible = ExamMan.ExamsChanged;
                EIMAutoSwitch = ExamMan.AutoSwitch;
                EIMInterval = ExamMan.PeriodIndex;
                SettingsChanged(sender, e);
            }

            ExamMan.Dispose();
        }

        private void CheckBoxShowXOnly_CheckedChanged(object sender, EventArgs e)
        {
            SettingsChanged(sender, e);
            CheckBoxCeiling.Enabled = ComboBoxShowXOnly.Enabled = CheckBoxShowXOnly.Checked;
            ComboBoxShowXOnly.SelectedIndex = CheckBoxShowXOnly.Checked ? AppConfig.Display.X : 0;
            ChangeCustomTextStyle(sender);

            if (CheckBoxCeiling.Checked && !CheckBoxShowXOnly.Checked)
            {
                CheckBoxCeiling.Checked = false;
                CheckBoxCeiling.Enabled = false;
            }
        }

        private void CheckBoxTopMost_CheckedChanged(object sender, EventArgs e)
        {
            ChangePptsvcStyle(sender, e);
            CheckBoxUniTopMost.Enabled = CheckBoxTopMost.Checked;

            if (CheckBoxUniTopMost.Checked && !CheckBoxTopMost.Checked)
            {
                CheckBoxUniTopMost.Checked = false;
                CheckBoxUniTopMost.Enabled = false;
            }
        }

        private void CheckBoxRulesMan_CheckedChanged(object sender, EventArgs e)
        {
            SettingsChanged(sender, e);
            ChangeCustomTextStyle(sender);
        }

        private void ButtonRulesMan_Click(object sender, EventArgs e)
        {
            RulesManager Manager = new()
            {
                CustomRules = EditedCustomRules,
                ShowWarning = !CheckBoxRulesMan.Checked,
                GlobalCustomTexts = EditedCustomTexts
            };

            if (Manager.ShowDialog() == DialogResult.OK)
            {
                SettingsChanged(sender, e);
                EditedCustomRules = Manager.CustomRules;
                EditedCustomTexts = Manager.GlobalCustomTexts;
            }
        }

        private void ButtonFont_Click(object sender, EventArgs e)
        {
            FontDialog FontDialogMain = new()
            {
                AllowScriptChange = true,
                AllowVerticalFonts = false,
                Font = AppConfig.Appearance.Font,
                FontMustExist = true,
                MinSize = ConfigPolicy.MinFontSize,
                MaxSize = ConfigPolicy.MaxFontSize,
                ScriptsOnly = true
            };

            if (FontDialogMain.ShowDialog() == DialogResult.OK)
            {
                SettingsChanged(sender, e);
                ChangeWorkingStyle(WorkingArea.ChangeFont, NewFont: FontDialogMain.Font);
            }

            FontDialogMain.Dispose();
        }

        private void ButtonDefaultFont_Click(object sender, EventArgs e)
        {
            ChangeWorkingStyle(WorkingArea.ChangeFont, NewFont: DefaultValues.CountdownDefaultFont);
            SettingsChanged(sender, e);
        }

        private void ColorLabels_Click(object sender, EventArgs e)
        {
            var LabelSender = (Label)sender;
            var ColorDialogMain = new ColorDialogEx();

            if (ColorDialogMain.ShowDialog(LabelSender.BackColor) == DialogResult.OK)
            {
                SettingsChanged(sender, e);
                LabelSender.BackColor = ColorDialogMain.Color;
                ChangeWorkingStyle(WorkingArea.SelectedColor);
            }

            ColorDialogMain.Dispose();
        }

        private void ColorLabels_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsColorLabelsDragging = true;
            }
        }

        private void ColorLabels_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsColorLabelsDragging)
            {
                Cursor = Cursors.Cross;
            }
        }

        private void ColorLabels_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                IsColorLabelsDragging = false;
                Cursor = Cursors.Default;

                var LabelSender = (Label)sender;
                var ParentContainer = LabelSender.Parent;
                var CursorPosition = ParentContainer.PointToClient(Cursor.Position);
                var TargetControl = ParentContainer.GetChildAtPoint(CursorPosition);

                if (TargetControl != null && TargetControl is Label TagetLabel && ColorLabels.Contains(TagetLabel) && LabelSender != TagetLabel)
                {
                    TagetLabel.BackColor = LabelSender.BackColor;
                    SettingsChanged(sender, e);
                    ChangeWorkingStyle(WorkingArea.SelectedColor);
                }
            }
            catch { }
        }

        private void ButtonDefaultColor_Click(object sender, EventArgs e)
        {
            var Pos = new Point(0, ButtonDefaultColor.Height);

            if (UseClassicContextMenu)
            {
                ContextMenuDefaultColor.Show(ButtonDefaultColor, Pos);
            }
            else
            {
                ContextMenuStripDefaultColor.Show(ButtonDefaultColor, Pos);
            }
        }

        private void MenuItemDark_Click(object sender, EventArgs e)
        {
            SetLabelColors(DefaultValues.CountdownDefaultColorsDark);
            SettingsChanged(sender, e);
        }

        private void MenuItemLight_Click(object sender, EventArgs e)
        {
            SetLabelColors(DefaultValues.CountdownDefaultColorsLight);
            SettingsChanged(sender, e);
        }

        private void ButtonRestart_MouseDown(object sender, MouseEventArgs e)
        {
            IsFunny = IsFunnyClick;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    IsFunnyClick = false;
                    break;
                case MouseButtons.Right:
                    ChangeWorkingStyle(WorkingArea.Funny);
                    IsFunnyClick = true;
                    break;
            }
        }

        private void ButtonRestart_Click(object sender, EventArgs e)
        {
            App.Shutdown(!IsFunny);
        }

        private void ButtonSyncTime_Click(object sender, EventArgs e)
        {
            var server = ((ComboData)ComboBoxNtpServers.SelectedItem).Display;
            ChangeWorkingStyle(WorkingArea.SyncTime);
            Task.Run(() => StartSyncTime(server)).ContinueWith(t => BeginInvoke(() => ChangeWorkingStyle(WorkingArea.SyncTime, false)));
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (IsSyncingTime)
            {
                MessageX.Warn("无法执行此操作，请等待同步网络时钟完成！");
                return;
            }

            if (IsSettingsFormatValid())
            {
                InvokeChangeRequired = true;
                HasSettingsChanged = false;
                Close();
            }
        }

        private void CheckBoxDraggable_CheckedChanged(object sender, EventArgs e)
        {
            ChangePptsvcStyle(sender, e);
            ComboBoxScreens.SelectedIndex = CheckBoxDraggable.Checked ? 0 : AppConfig.Display.ScreenIndex;
            ComboBoxPosition.SelectedIndex = CheckBoxDraggable.Checked ? 0 : (int)AppConfig.Display.Position;
            LabelScreens.Enabled = LabelChar1.Enabled = ComboBoxScreens.Enabled = !CheckBoxDraggable.Checked;
        }

        private void ComboBoxes_DropDown(object sender, EventArgs e)
        {
            #region
            /*
             
            DropDown 自适应大小 参考:

            c# - Auto-width of ComboBox's content - Stack Overflow
            https://stackoverflow.com/a/16435431/21094697
            c# - ComboBox auto DropDownWidth regardless of DataSource type - Stack Overflow
            https://stackoverflow.com/a/69350288/21094697
             
             */

            var ComboBoxes = (ComboBox)sender;
            int MaxWidth = 0;

            foreach (var Item in ComboBoxes.Items)
            {
                MaxWidth = Math.Max(MaxWidth, TextRenderer.MeasureText(ComboBoxes.GetItemText(Item), ComboBoxes.Font).Width);
            }

            ComboBoxes.DropDownWidth = MaxWidth + SystemInformation.VerticalScrollBarWidth;

            #endregion
        }

        private void ComboBoxShowXOnly_SelectedIndexChanged(object sender, EventArgs e)
        {
            SettingsChanged(sender, e);
            CheckBoxCeiling.Visible = ComboBoxShowXOnly.SelectedIndex == 0;
            CheckBoxCeiling.Checked = ComboBoxShowXOnly.SelectedIndex == 0 && AppConfig.Display.Ceiling;
        }

        private void ComboBoxScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            SettingsChanged(sender, e);
            ComboBoxPosition.Enabled = !CheckBoxDraggable.Checked && ComboBoxScreens.SelectedIndex != 0;
            ComboBoxPosition.SelectedIndex = ComboBoxPosition.Enabled ? (int)AppConfig.Display.Position : 0;
        }

        private void CheckBoxTrayIcon_CheckedChanged(object sender, EventArgs e)
        {
            SettingsChanged(sender, e);
            CheckBoxTrayText.Enabled = CheckBoxTrayIcon.Checked;
            CheckBoxTrayText.Checked = CheckBoxTrayIcon.Checked && AppConfig.General.TrayText;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosing(FormClosingEventArgs e)
        {
            if (App.AllowClosing)
            {
                e.Cancel = false;
            }
            else if (IsSyncingTime)
            {
                e.Cancel = true;
            }
            else if (HasSettingsChanged)
            {
                ShowUnsavedWarning("检测到当前设置未保存，是否立即进行保存？", e, () => ButtonSave_Click(null, null), () =>
                {
                    HasSettingsChanged = false;
                    Close();
                });
            }
        }

        protected override void OnClosed()
        {
            if (InvokeChangeRequired)
            {
                SaveSettings();
            }

            ChangeWorkingStyle(WorkingArea.Funny, false);
        }

        private void ChangeCustomTextStyle(object sender)
        {
            if (ChangingCheckBox) return;
            ChangingCheckBox = true;
            var cb = (CheckBox)sender;

            try
            {
                if (cb == CheckBoxShowXOnly)
                {
                    CheckBoxRulesMan.Enabled = !cb.Checked;
                    ButtonRulesMan.Enabled = false;
                }
                else
                {
                    ButtonRulesMan.Enabled = cb.Checked;
                    CheckBoxShowXOnly.Enabled = !cb.Checked;
                }
            }
            finally
            {
                ChangingCheckBox = false;
            }
        }

        private void ChangePptsvcStyle(object sender, EventArgs e)
        {
            SettingsChanged(sender, e);

            var a = CheckBoxTopMost.Checked;
            var b = ComboBoxPosition.SelectedIndex == 0;
            var c = CheckBoxDraggable.Checked;

            if (!a)
            {
                ChangeWorkingStyle(WorkingArea.SetPPTService, false);
            }
            else if ((a && c) || (a && b))
            {
                ChangeWorkingStyle(WorkingArea.SetPPTService);
            }
            else
            {
                ChangeWorkingStyle(WorkingArea.SetPPTService, false, 1);
            }
        }

        private bool IsSettingsFormatValid()
        {
            int ColorCheckMsg = 0;
            var Length = 4;
            SelectedColors = new ColorSetObject[Length];

            for (int i = 0; i < Length; i++)
            {
                var Fore = ColorLabels[i].BackColor;
                var Back = ColorLabels[i + Length].BackColor;

                if (!ColorHelper.IsNiceContrast(Fore, Back))
                {
                    ColorCheckMsg = i + 1;
                    break;
                }

                SelectedColors[i] = new(Fore, Back);
            }

            if (ColorCheckMsg != 0)
            {
                MessageX.Error($"第{ColorCheckMsg}组的颜色相似或对比度较低，将无法看清文字。\n\n请尝试更换其它背景颜色或文字颜色！", TabControlMain, TabPageAppearance);
                return false;
            }

            return true;
        }

        private void StartSyncTime(string Server)
        {
            try
            {
                if (!App.IsAdmin) MessageX.Warn("检测到当前用户不具有管理员权限，运行该操作会发生错误。\n\n程序将在此消息框关闭后尝试弹出 UAC 提示框，前提要把系统的 UAC 设置为 \"仅当应用尝试更改我的计算机时通知我\" 或及以上，否则将无法进行授权。\n\n稍后若没有看见提示框，请更改 UAC 设置: 开始菜单搜索 uac");

                Process SyncTimeProcess = ProcessHelper.RunProcess("cmd.exe", $"/c net stop w32time & sc config w32time start= auto & net start w32time && w32tm /config /manualpeerlist:{Server} /syncfromflags:manual /reliable:YES /update && w32tm /resync && w32tm /resync", AdminRequired: true);

                SyncTimeProcess.WaitForExit();
                var ExitCode = SyncTimeProcess.ExitCode;
                MessageX.Info($"命令执行完成！\n\n返回值为 {ExitCode} (0x{ExitCode:X})\n(0 代表成功，其他值为失败)", TabControlMain, TabPageTools);
            }
            #region 来自网络
            /*
                 
                检测用户是否点击了 UAC 提示框的 "否" 参考:

                c# - Run process as administrator from a non-admin application - Stack Overflow
                https://stackoverflow.com/a/20872219/21094697
                 
            */
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                MessageX.Error($"授权失败，请在 UAC 对话框弹出时点击 \"是\"。{ex.ToMessage()}", TabControlMain, TabPageTools);
            }
            #endregion
            catch (Exception ex)
            {
                MessageX.Error($"命令执行时发生了错误。{ex.ToMessage()}", TabControlMain, TabPageTools);
            }
        }

        private void ChangeWorkingStyle(WorkingArea Where, bool IsWorking = true, int SubCase = 0, Font NewFont = null)
        {
            switch (Where)
            {
                case WorkingArea.SyncTime:
                    IsSyncingTime = IsWorking;
                    ButtonSyncTime.Enabled = !IsWorking;
                    ComboBoxNtpServers.Enabled = !IsWorking;
                    ButtonRestart.Enabled = !IsWorking;
                    ButtonSave.Enabled = !IsWorking && HasSettingsChanged;
                    ButtonCancel.Enabled = !IsWorking;
                    ButtonSyncTime.Text = IsWorking ? "正在同步中，请稍候..." : "立即同步(&S)";
                    break;
                case WorkingArea.Funny:
                    GBoxRestart.Text = IsWorking ? "关闭倒计时" : "重启倒计时";
                    LabelRestart.Text = $"用于更改了屏幕缩放之后, 可以点击此按钮来重启程序以确保 UI 正常显示。{(IsWorking ? "(●'◡'●)" : "")}";
                    ButtonRestart.Text = IsWorking ? "点击关闭(&G)" : "点击重启(&R)";
                    break;
                case WorkingArea.SetPPTService:
                    CheckBoxPptSvc.Enabled = IsWorking;
                    CheckBoxPptSvc.Checked = IsWorking && AppConfig.Display.SeewoPptsvc;
                    CheckBoxPptSvc.Text = IsWorking ? "启用此功能(&X)" : $"此项暂不可用，因为倒计时没有{(SubCase == 0 ? "顶置" : "在左上角")}。";
                    break;
                case WorkingArea.ChangeFont:
                    SelectedFont = NewFont;
                    LabelFont.Text = $"当前字体: {NewFont.Name}, {NewFont.Size}pt, {NewFont.Style}";
                    break;
                case WorkingArea.LastColor:
                    SetLabelColors(AppConfig.Appearance.Colors);
                    break;
                case WorkingArea.SelectedColor:
                    LabelPreviewColor1.BackColor = LabelColor12.BackColor;
                    LabelPreviewColor2.BackColor = LabelColor22.BackColor;
                    LabelPreviewColor3.BackColor = LabelColor32.BackColor;
                    LabelPreviewColor4.BackColor = LabelColor42.BackColor;
                    LabelPreviewColor1.ForeColor = LabelColor11.BackColor;
                    LabelPreviewColor2.ForeColor = LabelColor21.BackColor;
                    LabelPreviewColor3.ForeColor = LabelColor31.BackColor;
                    LabelPreviewColor4.ForeColor = LabelColor41.BackColor;
                    break;
            }
        }

        private void SetLabelColors(ColorSetObject[] Colors)
        {
            for (int i = 0; i < 4; i++)
            {
                ColorLabels[i].BackColor = Colors[i].Fore;
                ColorPreviewLabels[i].ForeColor = Colors[i].Fore;
                ColorLabels[i + 4].BackColor = Colors[i].Back;
                ColorPreviewLabels[i].BackColor = Colors[i].Back;
            }
        }

        private void SaveSettings()
        {
            try
            {
                using var reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (CheckBoxStartup.Checked)
                    reg.SetValue(App.AppNameEng, $"\"{App.CurrentExecutablePath}\"");
                else
                    reg.DeleteValue(App.AppNameEng, false);

                AppConfig.General = new()
                {
                    ExamInfo = AppConfig.General.ExamInfo,
                    ExamIndex = AppConfig.General.ExamIndex,
                    AutoSwitch = EIMAutoSwitch,
                    Interval = EIMInterval,
                    MemClean = CheckBoxMemClean.Checked,
                    TopMost = CheckBoxTopMost.Checked,
                    UniTopMost = CheckBoxUniTopMost.Checked,
                    TrayIcon = CheckBoxTrayIcon.Checked,
                    TrayText = CheckBoxTrayText.Checked,
                    WCCMS = CheckBoxWCCMS.Checked
                };

                AppConfig.Display = new()
                {
                    ShowXOnly = CheckBoxShowXOnly.Checked,
                    X = ComboBoxShowXOnly.SelectedIndex,
                    Ceiling = CheckBoxCeiling.Checked,
                    EndIndex = ComboBoxCountdownEnd.SelectedIndex,
                    CustomText = CheckBoxRulesMan.Checked,
                    CustomTexts = EditedCustomTexts,
                    ScreenIndex = ComboBoxScreens.SelectedIndex,
                    Position = (CountdownPosition)ComboBoxPosition.SelectedIndex,
                    Draggable = CheckBoxDraggable.Checked,
                    SeewoPptsvc = CheckBoxPptSvc.Checked
                };

                AppConfig.Appearance = new()
                {
                    Font = SelectedFont,
                    Colors = SelectedColors
                };

                AppConfig.Tools = new()
                {
                    NtpServer = ComboBoxNtpServers.SelectedIndex
                };

                AppConfig.CustomRules = EditedCustomRules;

                App.AppConfig = AppConfig;
                RefreshNeeded = true;
            }
            catch
            {

            }
        }
    }
}
