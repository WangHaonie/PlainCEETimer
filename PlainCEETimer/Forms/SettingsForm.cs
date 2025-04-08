using PlainCEETimer.Controls;
using PlainCEETimer.Dialogs;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainCEETimer.Forms
{
    public partial class SettingsForm : AppForm
    {
        public bool RefreshNeeded { get; private set; }

        private Font SelectedFont;
        private CustomRuleObject[] EditedCustomRules;
        private ExamInfoObject[] EditedExamInfo;
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
        private NavigationBar NavBar;
        private Label[] ColorLabels;
        private Label[] ColorPreviewLabels;
        private ColorSetObject[] SelectedColors;
        private readonly ConfigObject AppConfig = App.AppConfig;
        private readonly StartUp StartUp = new();

        public SettingsForm()
        {
            CompositedStyle = true;
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            RefreshNeeded = false;
            InitializeExtra();
            RefreshSettings();
            UpdateSettingsArea(SettingsArea.LastColor);
            UpdateSettingsArea(SettingsArea.Funny, false);
        }

        protected override void AdjustUI()
        {
            AlignControlsR(ButtonSave, ButtonCancel, PageNavPages);
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
            CompactControlsX(ComboBoxAutoSwitchIntervel, CheckBoxAutoSwitch);
            CompactControlsY(ButtonSyncTime, LabelSyncTime, 3);
            CompactControlsY(ButtonRestart, LabelRestart, 3);

            WhenHighDpi(() =>
            {
                AlignControlsX(ComboBoxAutoSwitchIntervel, CheckBoxAutoSwitch);
                AlignControlsX(ComboBoxShowXOnly, CheckBoxShowXOnly, -1);
                AlignControlsX(ComboBoxScreens, LabelScreens);
                AlignControlsX(ComboBoxPosition, LabelChar1);
                AlignControlsX(ComboBoxCountdownEnd, LabelCountdownEnd);
                AlignControlsX(ButtonRulesMan, CheckBoxRulesMan);
                AlignControlsX(ComboBoxNtpServers, ButtonSyncTime);
            });
        }

        protected override void OnShown()
        {
            NavBar.Focus();
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

            UpdateSettingsArea(SettingsArea.Funny, false);
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            WhenLoaded(() =>
            {
                HasSettingsChanged = true;
                ButtonSave.Enabled = true;
            });
        }

        private void ButtonExamInfo_Click(object sender, EventArgs e)
        {
            ExamInfoManager ExamMan = new()
            {
                Data = EditedExamInfo
            };

            if (ExamMan.ShowDialog() == DialogResult.OK)
            {
                EditedExamInfo = ExamMan.Data;
                SettingsChanged(sender, e);
            }
        }

        private void CheckBoxAutoSwitch_CheckedChanged(object sender, EventArgs e)
        {
            ComboBoxAutoSwitchIntervel.Enabled = CheckBoxAutoSwitch.Checked;
            SettingsChanged(sender, e);
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
                Data = EditedCustomRules,
                ColorPresets = SelectedColors,
                CustomTextPreset = EditedCustomTexts
            };

            if (Manager.ShowDialog() == DialogResult.OK)
            {
                EditedCustomRules = Manager.Data;
                EditedCustomTexts = Manager.CustomTextPreset;
                SettingsChanged(sender, e);
            }
        }

        private void ButtonFont_Click(object sender, EventArgs e)
        {
            FontDialog FontDialogMain = new()
            {
                AllowScriptChange = true,
                AllowVerticalFonts = false,
                Font = AppConfig.Font,
                FontMustExist = true,
                MinSize = Validator.MinFontSize,
                MaxSize = Validator.MaxFontSize,
                ScriptsOnly = true
            };

            if (FontDialogMain.ShowDialog() == DialogResult.OK)
            {
                SettingsChanged(sender, e);
                UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: FontDialogMain.Font);
            }
        }

        private void ButtonDefaultFont_Click(object sender, EventArgs e)
        {
            UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: DefaultValues.CountdownDefaultFont);
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
                UpdateSettingsArea(SettingsArea.SelectedColor);
            }
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
                    UpdateSettingsArea(SettingsArea.SelectedColor);
                }
            }
            catch
            {

            }
        }

        private void ButtonDefaultColor_Click(object sender, EventArgs e)
        {
            ShowContextMenu(ButtonDefaultColor, ContextMenuDefaultColor, ContextMenuStripDefaultColor, UseClassicContextMenu);
        }

        private void ContextDark_Click(object sender, EventArgs e)
        {
            SetLabelColors(DefaultValues.CountdownDefaultColorsDark);
            SettingsChanged(sender, e);
        }

        private void ContextLight_Click(object sender, EventArgs e)
        {
            SetLabelColors(DefaultValues.CountdownDefaultColorsLight);
            SettingsChanged(sender, e);
        }

        private void ButtonRestart_MouseDown(object sender, MouseEventArgs e)
        {
            IsFunny = IsFunnyClick;

            if (e.Button == MouseButtons.Left)
            {
                IsFunnyClick = false;
            }
            else
            {
                UpdateSettingsArea(SettingsArea.Funny);
                IsFunnyClick = true;
            }
        }

        private void ButtonRestart_Click(object sender, EventArgs e)
        {
            App.Shutdown(!IsFunny);
        }

        private void ButtonSyncTime_Click(object sender, EventArgs e)
        {
            var server = ((ComboData)ComboBoxNtpServers.SelectedItem).Display;
            UpdateSettingsArea(SettingsArea.SyncTime);
            Task.Run(() => StartSyncTime(server)).ContinueWith(t => BeginInvoke(() => UpdateSettingsArea(SettingsArea.SyncTime, false)));
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
            ComboBoxPosition.SelectedIndex = CheckBoxDraggable.Checked ? 3 : (int)AppConfig.Display.Position;

            var flag = !CheckBoxDraggable.Checked;
            LabelScreens.Enabled = flag;
            LabelChar1.Enabled = flag;
            ComboBoxScreens.Enabled = flag;
            ComboBoxPosition.Enabled = flag;
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
            ComboBoxPosition.SelectedIndex = ComboBoxPosition.Enabled ? (int)AppConfig.Display.Position : 3;
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

        private void InitializeExtra()
        {
            PanelNav.Controls.Add(NavBar = new(["基本", "显示", "外观", "工具"], [PageGeneral, PageDisplay, PageAppearance, PageTools], PageNavPages)
            {
                Indent = ScaleToDpi(5),
                ItemHeight = ScaleToDpi(25)
            });

            if (UseClassicContextMenu)
            {
                ContextMenuDefaultColor = CreateNew
                ([
                    AddItem(ContextMenuConstants.Light, ContextLight_Click),
                    AddItem(ContextMenuConstants.Dark, ContextDark_Click)
                ]);
            }
            else
            {
                ContextMenuStripDefaultColor = CreateNewStrip
                ([
                    AddStripItem(ContextMenuConstants.Light, ContextLight_Click),
                    AddStripItem(ContextMenuConstants.Dark, ContextDark_Click)
                ]);
            }

            LabelPreviewColor1.Text = $"{Placeholders.PH_JULI}...{Placeholders.PH_START}...";
            LabelPreviewColor2.Text = $"{Placeholders.PH_JULI}...{Placeholders.PH_LEFT}...";
            LabelPreviewColor3.Text = $"{Placeholders.PH_JULI}...{Placeholders.PH_PAST}...";

            BindComboData(ComboBoxAutoSwitchIntervel,
            [
                new("10 秒", 0),
                new("15 秒", 1),
                new("30 秒", 2),
                new("45 秒", 3),
                new("1 分钟", 4),
                new("2 分钟", 5),
                new("3 分钟", 6),
                new("5 分钟", 7),
                new("10 分钟", 8),
                new("15 分钟", 9),
                new("30 分钟", 10),
                new("45 分钟", 11),
                new("1 小时", 12),
            ]);

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
                new("考试还有多久结束 和 已过去了多久", 2)
            ]);

            BindComboData(ComboBoxPosition,
            [
                new("左上角", 0),
                new("左侧居中", 1),
                new("左下角", 2),
                new("顶部居中", 3),
                new("屏幕中心", 4),
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
            var Length = CurrentScreens.Length;
            var Monitors = new ComboData[Length];
            for (int i = 0; i < Length; i++)
            {
                var CurrentScreen = CurrentScreens[i];
                Monitors[i] = new(string.Format("{0} {1} ({2}x{3})", i + 1, CurrentScreen.DeviceName, CurrentScreen.Bounds.Width, CurrentScreen.Bounds.Height), i);
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

        private void RefreshSettings()
        {
            CheckBoxStartup.Checked = (bool)StartUp.Operate(0);
            CheckBoxTopMost.Checked = AppConfig.General.TopMost;
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
            UpdateSettingsArea(SettingsArea.ChangeFont, NewFont: AppConfig.Font);
            ChangePptsvcStyle(null, EventArgs.Empty);
            SelectedColors = AppConfig.GlobalColors;
            ComboBoxShowXOnly.SelectedIndex = AppConfig.Display.ShowXOnly ? AppConfig.Display.X : 0;
            ComboBoxNtpServers.SelectedIndex = AppConfig.NtpServer;
            EditedCustomTexts = AppConfig.GlobalCustomTexts;
            EditedCustomRules = AppConfig.CustomRules;
            EditedExamInfo = AppConfig.Exams;
            CheckBoxTrayText.Enabled = CheckBoxTrayIcon.Checked = AppConfig.General.TrayIcon;
            CheckBoxTrayText.Checked = AppConfig.General.TrayText;
            CheckBoxAutoSwitch.Checked = AppConfig.General.AutoSwitch;
            ComboBoxAutoSwitchIntervel.SelectedIndex = AppConfig.General.Interval;
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
                UpdateSettingsArea(SettingsArea.SetPPTService, false);
            }
            else if ((a && c) || (a && b))
            {
                UpdateSettingsArea(SettingsArea.SetPPTService);
            }
            else
            {
                UpdateSettingsArea(SettingsArea.SetPPTService, false, 1);
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

                if (!Validator.IsNiceContrast(Fore, Back))
                {
                    ColorCheckMsg = i + 1;
                    break;
                }

                SelectedColors[i] = new(Fore, Back);
            }

            if (ColorCheckMsg != 0)
            {
                NavBar.SwitchTo(PageAppearance);
                MessageX.Error($"第{ColorCheckMsg}组颜色的对比度较低，将无法看清文字。\n\n请更换其它背景颜色或文字颜色！");
                return false;
            }

            return true;
        }

        private void StartSyncTime(string Server)
        {
            try
            {
                if (!App.IsAdmin)
                {
                    MessageX.Warn("检测到当前用户不具有管理员权限，运行该操作会发生错误。\n\n程序将在此消息框关闭后尝试弹出 UAC 提示框，前提要把系统的 UAC 设置为 \"仅当应用尝试更改我的计算机时通知我\" 或及以上，否则将无法进行授权。\n\n稍后若没有看见提示框，请更改 UAC 设置: 开始菜单搜索 uac");
                }

                var ExitCode = (int)ProcessHelper.Run("cmd.exe", string.Format("/c net stop w32time & sc config w32time start= auto & net start w32time && w32tm /config /manualpeerlist:{0} /syncfromflags:manual /reliable:YES /update && w32tm /resync && w32tm /resync", Server), 2, true);
                SwitchToToolsSafe();
                MessageX.Info($"命令执行完成！\n\n返回值为 {ExitCode} (0x{ExitCode:X})\n(0 代表成功，其他值为失败)");
            }
            #region 来自网络
            /*
                 
                检测用户是否点击了 UAC 提示框的 "否" 参考:

                c# - Run process as administrator from a non-admin application - Stack Overflow
                https://stackoverflow.com/a/20872219/21094697
                 
            */
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                SwitchToToolsSafe();
                MessageX.Error("授权失败，请在 UAC 对话框弹出时点击 \"是\"。", ex);
            }
            #endregion
            catch (Exception ex)
            {
                SwitchToToolsSafe();
                MessageX.Error("命令执行时发生了错误。", ex);
            }
        }

        private void UpdateSettingsArea(SettingsArea Where, bool IsWorking = true, int SubCase = 0, Font NewFont = null)
        {
            switch (Where)
            {
                case SettingsArea.SyncTime:
                    IsSyncingTime = IsWorking;
                    ButtonSyncTime.Enabled = !IsWorking;
                    ComboBoxNtpServers.Enabled = !IsWorking;
                    ButtonRestart.Enabled = !IsWorking;
                    ButtonSave.Enabled = !IsWorking && HasSettingsChanged;
                    ButtonCancel.Enabled = !IsWorking;
                    ButtonSyncTime.Text = IsWorking ? "正在同步中，请稍候..." : "立即同步(&S)";
                    break;
                case SettingsArea.Funny:
                    GBoxRestart.Text = IsWorking ? "关闭倒计时" : "重启倒计时";
                    LabelRestart.Text = $"用于更改了屏幕缩放之后, 可以点击此按钮来重启程序以确保 UI 正常显示。{(IsWorking ? "(●'◡'●)" : "")}";
                    ButtonRestart.Text = IsWorking ? "点击关闭(&G)" : "点击重启(&R)";
                    break;
                case SettingsArea.SetPPTService:
                    CheckBoxPptSvc.Enabled = IsWorking;
                    CheckBoxPptSvc.Checked = IsWorking && AppConfig.Display.SeewoPptsvc;
                    CheckBoxPptSvc.Text = IsWorking ? "启用此功能(&X)" : $"此项暂不可用，因为倒计时没有{(SubCase == 0 ? "顶置" : "在左上角")}。";
                    break;
                case SettingsArea.ChangeFont:
                    SelectedFont = NewFont;
                    LabelFont.Text = $"当前字体: {NewFont.Name}, {NewFont.Size}pt, {NewFont.Style}";
                    break;
                case SettingsArea.LastColor:
                    SetLabelColors(SelectedColors);
                    break;
                case SettingsArea.SelectedColor:
                    for (int i = 0; i < 4; i++)
                    {
                        ColorPreviewLabels[i].ForeColor = ColorLabels[i].BackColor;
                        ColorPreviewLabels[i].BackColor = ColorLabels[i + 4].BackColor;
                    }
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

        private void SwitchToToolsSafe()
        {
            BeginInvoke(() => NavBar.SwitchTo(PageTools));
        }

        private void SaveSettings()
        {
            StartUp.Operate(CheckBoxStartup.Checked ? 1 : 2);

            App.AppConfig = new()
            {
                General = new()
                {
                    AutoSwitch = CheckBoxAutoSwitch.Checked,
                    Interval = ComboBoxAutoSwitchIntervel.SelectedIndex,
                    TrayIcon = CheckBoxTrayIcon.Checked,
                    TrayText = CheckBoxTrayText.Checked,
                    MemClean = CheckBoxMemClean.Checked,
                    TopMost = CheckBoxTopMost.Checked,
                    UniTopMost = CheckBoxUniTopMost.Checked,
                    WCCMS = CheckBoxWCCMS.Checked
                },

                Display = new()
                {
                    ShowXOnly = CheckBoxShowXOnly.Checked,
                    X = ComboBoxShowXOnly.SelectedIndex,
                    Ceiling = CheckBoxCeiling.Checked,
                    EndIndex = ComboBoxCountdownEnd.SelectedIndex,
                    CustomText = CheckBoxRulesMan.Checked,
                    ScreenIndex = ComboBoxScreens.SelectedIndex,
                    Position = (CountdownPosition)ComboBoxPosition.SelectedIndex,
                    Draggable = CheckBoxDraggable.Checked,
                    SeewoPptsvc = CheckBoxPptSvc.Checked
                },

                Exams = EditedExamInfo,
                ExamIndex = AppConfig.ExamIndex,
                GlobalCustomTexts = EditedCustomTexts,
                GlobalColors = SelectedColors,
                CustomRules = EditedCustomRules,
                CustomColors = AppConfig.CustomColors,
                Font = SelectedFont,
                NtpServer = ComboBoxNtpServers.SelectedIndex,
                Location = AppConfig.Location,
            };

            RefreshNeeded = true;
        }
    }
}
