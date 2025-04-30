using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using PlainCEETimer.Controls;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Forms
{
    public sealed partial class MainForm : AppForm
    {
        public static bool IsNormalStart { get; set; }
        public static bool UniTopMost { get; private set; } = true;
        public static bool ValidateNeeded = true;
        public static event Action UniTopMostChanged;

        private bool AutoSwitch;
        private bool CanUseRules;
        private bool IsCeiling;
        private bool IsCountdownReady;
        private bool IsCountdownRunning;
        private bool IsDraggable;
        private bool IsPPTService;
        private bool IsReadyToMove;
        private bool IsShowXOnly;
        private bool LoadedMemCleaner;
        private bool MemClean;
        private bool SetRoundRegion;
        private bool ShowThemeChangedWarning;
        private bool ShowTrayIcon;
        private bool ShowTrayText;
        private bool TrayIconReopen;
        private bool UseCustomText;
        private int AutoSwitchInterval;
        private int CurrentTheme;
        private int ExamIndex;
        private int ScreenIndex;
        private int ShowXOnlyIndex;
        private const int BorderRadius = 13;
        private const int PptsvcThreshold = 1;
        private const int MemCleanerInterval = 300_000; // 5 min
        private string ExamName;
        private string[] GlobalTexts;
        private CountdownMode Mode;
        private CountdownPosition CountdownPos;
        private CountdownPhase CurrentPhase = CountdownPhase.None;
        private CountdownState SelectedState;
        private ColorSetObject[] CountdownColors;
        private DateTime ExamEnd;
        private DateTime ExamStart;
        private Point LastLocation;
        private Point LastMouseLocation;
        private Rectangle SelectedScreenRect;
        private AboutForm FormAbout;
        private ConfigObject AppConfig;
        private ContextMenu ContextMenuMain;
        private ContextMenu ContextMenuTray;
        private CustomRuleObject[] CustomRules;
        private CustomRuleObject[] CurrentRules;
        private ExamInfoObject CurrentExam;
        private ExamInfoObject[] Exams;
        private Menu.MenuItemCollection ExamSwitchMain;
        private NotifyIcon TrayIcon;
        private SettingsForm FormSettings;
        private System.Threading.Timer MemCleaner;
        private System.Threading.Timer Countdown;
        private System.Windows.Forms.Timer AutoSwitchHandler;
        private readonly string[] DefaultTexts = [Constants.PH_START, Constants.PH_LEFT, Constants.PH_PAST];
        private static readonly StringBuilder CustomTextBuilder = new();


        public MainForm() : base(AppFormParam.Special)
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetRoundCorners();
        }

        protected override void OnLoad()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            SizeChanged += MainForm_SizeChanged;
        }

        protected override void OnShown()
        {
            RefreshSettings();
            ValidateNeeded = false;
            Task.Run(() => new Updater().CheckForUpdate(false, this));
            IsNormalStart = true;
        }

        protected override void OnClosing(FormClosingEventArgs e)
        {
            if (App.AllowUIClosing || e.CloseReason == CloseReason.WindowsShutDown)
            {
                if (App.CanSaveConfig)
                {
                    RealSaveConfig();
                }

                Countdown?.Dispose();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            RefreshScreen();
            SetLabelCountdownAutoWrap();
            ApplyLocation();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (SetRoundRegion)
            {
                RoundCorner.SetRoundCornerRegion(Handle, Width, Height, ScaleToDpi(BorderRadius));
            }

            ValidateLocation();
        }

        private void MainForm_LocationRefreshed()
        {
            AppConfig.Location = Location;
            SaveConfig();
        }

        private void ExamItems_Click(object sender, EventArgs e)
        {
            int ItemIndex;
            var Sender = (MenuItem)sender;
            ItemIndex = Sender.Index;

            if (Sender != null && !Sender.Checked)
            {
                UnselectAllExamItems();
                ExamIndex = ItemIndex;
                AppConfig.ExamIndex = ItemIndex;
                SaveConfig();
                LoadExams();
                TryRunCountdown();
                UpdateExamSelection();
            }
        }

        #region 来自网络
        /*
        
        无边框窗口的拖动 参考:

        C#创建无边框可拖动窗口 - 掘金
        https://juejin.cn/post/6989144829607280648

        */
        private void LabelCountdown_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsReadyToMove = true;
                Cursor = Cursors.SizeAll;
                LastMouseLocation = e.Location;
                LastLocation = Location;
            }
        }

        private void LabelCountdown_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsReadyToMove)
            {
                Location = new(MousePosition.X - LastMouseLocation.X, MousePosition.Y - LastMouseLocation.Y);
            }
        }

        private void LabelCountdown_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;

            if (IsReadyToMove && Location != LastLocation)
            {
                KeepOnScreen();
                CompatibleWithPPTService();
                SetLabelCountdownAutoWrap();
                AppConfig.Location = Location;
                SaveConfig();
            }

            IsReadyToMove = false;
        }
        #endregion

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) App.OnTrayMenuShowAllClicked();
        }

        private void ExamAutoSwitch(object sender, EventArgs e)
        {
            AppConfig.ExamIndex = (ExamIndex + 1) % Exams.Length;
            SaveConfig();
            LoadExams();
            TryRunCountdown();
            UnselectAllExamItems();
            UpdateExamSelection(true);
        }

        private void ValidateLocation()
        {
            if (!IsReadyToMove)
            {
                ApplyLocation();
                KeepOnScreen();
            }
        }

        private void RefreshSettings()
        {
            AppConfig = App.AppConfig;
            ValidateConfig();
            LoadConfig();
            LoadExams();
            PrepareCountdown();
            ApplyLocation();
            CompatibleWithPPTService();
            LoadContextMenu();
            LoadTrayIcon();
            UniTopMostChanged?.Invoke();
            SetLabelCountdownAutoWrap();
            TopMost = false;
            TopMost = AppConfig.General.TopMost;
            ShowInTaskbar = !TopMost;
            TryRunCountdown();

            if (ShowThemeChangedWarning)
            {
                MessageX.Warn("由于更改了应用主题设置，需要立即重启倒计时！");
                App.Shutdown(true);
            }
        }

        private void TryRunCountdown()
        {
            if (!IsCountdownRunning)
            {
                IsCountdownRunning = true;
                Countdown = new(CountdownCallback, null, 0, 1000);
            }
        }

        private void ValidateConfig()
        {
            if (ValidateNeeded)
            {
                AppConfig.Display.Ceiling = AppConfig.Display.Ceiling && AppConfig.Display.ShowXOnly && AppConfig.Display.X == 0;
                AppConfig.Display.CustomText = AppConfig.Display.CustomText && !AppConfig.Display.ShowXOnly;
                AppConfig.Display.SeewoPptsvc = AppConfig.Display.SeewoPptsvc && ((AppConfig.General.TopMost && AppConfig.Display.X == 0) || AppConfig.Display.Draggable);
                AppConfig.General.TrayText = AppConfig.General.TrayText && AppConfig.General.TrayIcon;
            }
        }

        private void LoadConfig()
        {
            GlobalTexts = AppConfig.GlobalCustomTexts;
            MemClean = AppConfig.General.MemClean;
            IsShowXOnly = AppConfig.Display.ShowXOnly;
            IsCeiling = AppConfig.Display.Ceiling;
            IsDraggable = AppConfig.Display.Draggable;
            UniTopMost = AppConfig.General.UniTopMost;
            IsPPTService = AppConfig.Display.SeewoPptsvc;
            UseCustomText = AppConfig.Display.CustomText;
            ScreenIndex = AppConfig.Display.ScreenIndex;
            CountdownPos = AppConfig.Display.Position;
            ShowXOnlyIndex = AppConfig.Display.X;
            ShowTrayIcon = AppConfig.General.TrayIcon;
            ShowTrayText = AppConfig.General.TrayText;
            CustomRules = AppConfig.CustomRules;
            CountdownColors = AppConfig.GlobalColors;
            var theme = AppConfig.Dark;

            if (!ValidateNeeded && theme != CurrentTheme)
            {
                ShowThemeChangedWarning = true;
            }
            else if (ValidateNeeded)
            {
                CurrentTheme = theme;
            }

            var EndIndex = AppConfig.Display.EndIndex;
            Mode = EndIndex == 2 ? CountdownMode.Mode3 : (EndIndex is 1 or 2 ? CountdownMode.Mode2 : CountdownMode.Mode1);
        }

        private void LoadExams()
        {
            AutoSwitch = AppConfig.General.AutoSwitch;
            AutoSwitchInterval = Validator.GetAutoSwitchInterval(AppConfig.General.Interval);
            Exams = AppConfig.Exams;
            var i = AppConfig.ExamIndex;
            ExamIndex = i < Exams.Length ? i : 0;

            try
            {
                CurrentExam = Exams[ExamIndex];
            }
            catch
            {
                ExamIndex = -1;
                CurrentExam = new();
            }

            ExamName = CurrentExam.Name;
            ExamStart = CurrentExam.Start;
            ExamEnd = CurrentExam.End;
            IsCountdownReady = !string.IsNullOrWhiteSpace(ExamName) && (ExamEnd > ExamStart || Mode == CountdownMode.Mode1);

            if (IsCountdownReady && CurrentPhase != CountdownPhase.None)
            {
                RefreshCustomRules();
            }
        }

        private void PrepareCountdown()
        {
            SelectedState = CountdownState.Normal;

            if (IsShowXOnly)
            {
                SelectedState = ShowXOnlyIndex switch
                {
                    1 => CountdownState.HoursOnly,
                    2 => CountdownState.MinutesOnly,
                    3 => CountdownState.SecondsOnly,
                    _ => IsCeiling ? CountdownState.DaysOnlyWithCeiling : CountdownState.DaysOnly
                };
            }

            LabelCountdown.Font = AppConfig.Font;

            LabelCountdown.MouseDown -= LabelCountdown_MouseDown;
            LabelCountdown.MouseMove -= LabelCountdown_MouseMove;
            LabelCountdown.MouseUp -= LabelCountdown_MouseUp;
            LocationRefreshed -= MainForm_LocationRefreshed;

            if (IsDraggable)
            {
                LabelCountdown.MouseDown += LabelCountdown_MouseDown;
                LabelCountdown.MouseMove += LabelCountdown_MouseMove;
                LabelCountdown.MouseUp += LabelCountdown_MouseUp;
                LocationRefreshed += MainForm_LocationRefreshed;
                Location = AppConfig.Location;
            }
            else
            {
                RefreshScreen();
            }

            if (MemClean && !LoadedMemCleaner)
            {
                MemCleaner = new(_ => MemoryCleaner.CleanMemory(9 * 1024 * 1024), null, 3000, MemCleanerInterval);
                LoadedMemCleaner = true;
            }

            if (!MemClean && LoadedMemCleaner)
            {
                MemCleaner.Dispose();
            }
        }

        private void LoadContextMenu()
        {
            ContextMenuMain = BaseContextMenu();
            ExamSwitchMain = ContextMenuMain.MenuItems[0].MenuItems;

            if (ShowTrayIcon)
            {
                var tmp = BaseContextMenu();
                tmp.MenuItems.RemoveAt(0);
                tmp.MenuItems.RemoveAt(0);
                ContextMenuTray = tmp;
            }

            ContextMenu = ContextMenuMain;
            LabelCountdown.ContextMenu = ContextMenuMain;

            if (Exams.Length != 0)
            {
                ExamSwitchMain.Clear();

                for (int i = 0; i < Exams.Length; i++)
                {
                    var Item = new MenuItem()
                    {
                        Text = $"{i + 1}. {Exams[i]}",
                        RadioCheck = true,
                        Checked = i == ExamIndex
                    };

                    Item.Click += ExamItems_Click;
                    ExamSwitchMain.Add(Item);
                }
            }

            UpdateExamSelection();

            #region 来自网络
            /*

            克隆 (重用) 现有 ContextMenuStrip 实例 参考：

            .net - C# - Duplicate ContextMenuStrip Items into another - Stack Overflow
            https://stackoverflow.com/questions/37884815/c-sharp-duplicate-contextmenustrip-items-into-another

            */

            ContextMenu BaseContextMenu() => ContextMenuBuilder.Build(b =>
            [
                b.Menu("切换(&Q)",
                [
                    b.Item("请先添加考试信息")
                ]),

                b.Separator(),

                b.Item("设置(&S)", (_, _) =>
                {
                    if (FormSettings == null || FormSettings.IsDisposed)
                    {
                        FormSettings = new();

                        FormSettings.FormClosed += (_, _) =>
                        {
                            if (FormSettings.RefreshNeeded)
                            {
                                RealSaveConfig();
                                RefreshSettings();
                            }
                        };
                    }

                    FormSettings.ReActivate();
                }),

                b.Item("关于(&A)", (_, _) =>
                {
                    if (FormAbout == null || FormAbout.IsDisposed)
                    {
                        FormAbout = new();
                    }

                    FormAbout.ReActivate();
                }),

                b.Separator(),
                b.Item("安装目录(&D)", (_, _) => App.OpenInstallDir())
            ]);
            #endregion
        }

        private void LoadTrayIcon()
        {
            if (TrayIcon == null)
            {
                if (ShowTrayIcon)
                {
                    if (TrayIconReopen)
                    {
                        if (MessageX.Warn("由于系统限制，重新开关托盘图标需要重启应用程序后方可正常显示。\n\n是否立即重启？", Buttons: MessageButtons.YesNo) == DialogResult.Yes)
                        {
                            App.Shutdown(true);
                        }
                        else
                        {
                            return;
                        }
                    }

                    TrayIcon = new()
                    {
                        Visible = true,
                        Text = Text,
                        Icon = App.AppIcon,
                        ContextMenu = ContextMenuBuilder.Merge(ContextMenuTray, ContextMenuBuilder.Build(b =>
                        [
                            b.Separator(),
                            b.Item("显示界面(&S)", (_, _) => App.OnTrayMenuShowAllClicked()),
                            b.Menu("关闭(&C)",
                            [
                                b.Item("重启(&R)", (_, _) => App.Shutdown(true)),
                                b.Item("退出(&Q)", (_, _) => App.Shutdown())
                            ])
                        ]))
                    };

                    TrayIcon.MouseClick -= TrayIcon_MouseClick;
                    TrayIcon.MouseClick += TrayIcon_MouseClick;

                    if (!ShowTrayText)
                    {
                        UpdateTrayIconText(App.AppName);
                    }
                }
            }
            else
            {
                if (!ShowTrayIcon)
                {
                    TrayIcon.Dispose();
                    TrayIcon = null;
                    TrayIconReopen = true;
                }
                else if (!ShowTrayText)
                {
                    UpdateTrayIconText(App.AppName);
                }
            }
        }

        private void UnselectAllExamItems()
        {
            foreach (MenuItem Item in ExamSwitchMain)
            {
                Item.Checked = false;
            }
        }

        private void UpdateExamSelection(bool UpdateOnly = false)
        {
            if (ExamIndex != -1)
            {
                ExamSwitchMain[ExamIndex].Checked = true;
            }
            else
            {
                var Item = ExamSwitchMain[0];
                Item.Checked = false;
                Item.Enabled = false;
            }

            if (!UpdateOnly && AutoSwitch)
            {
                AutoSwitchHandler?.Dispose();

                if (IsCountdownReady && Exams.Length > 1)
                {
                    AutoSwitchHandler = new() { Interval = AutoSwitchInterval };
                    AutoSwitchHandler.Tick += ExamAutoSwitch;
                    AutoSwitchHandler.Start();
                }
            }
            else if (!AutoSwitch)
            {
                AutoSwitchHandler?.Dispose();
            }
        }

        private void CountdownCallback(object state)
        {
            var Now = DateTime.Now;

            if (IsCountdownReady)
            {
                if (Mode >= CountdownMode.Mode1 && Now < ExamStart)
                {
                    SetPhase(CountdownPhase.P1);
                    ApplyCustomRule((int)CountdownPhase.P1, ExamStart - Now);
                    return;
                }

                if (Mode >= CountdownMode.Mode2 && Now < ExamEnd)
                {
                    SetPhase(CountdownPhase.P2);
                    ApplyCustomRule((int)CountdownPhase.P2, ExamEnd - Now);
                    return;
                }

                if (Mode >= CountdownMode.Mode3 && Now > ExamEnd)
                {
                    SetPhase(CountdownPhase.P3);
                    ApplyCustomRule((int)CountdownPhase.P3, Now - ExamEnd);
                    return;
                }
            }

            Countdown.Dispose();
            UpdateCountdown("欢迎使用高考倒计时", CountdownColors[3]);
            UpdateTrayIconText(App.AppName, true);
            IsCountdownRunning = false;
        }

        private void ApplyCustomRule(int Phase, TimeSpan Span)
        {
            if (UseCustomText)
            {
                if (CanUseRules)
                {
                    for (int i = 0; i < CurrentRules.Length; i++)
                    {
                        var Rule = CurrentRules[i];

                        if (Phase == (int)CountdownPhase.P3 ? (Span >= Rule.Tick) : (Span <= Rule.Tick))
                        {
                            UpdateCountdown(SetCustomRule(Span, Rule.Text), Rule.Colors);
                            return;
                        }
                    }
                }

                UpdateCountdown(SetCustomRule(Span, GlobalTexts[Phase]), CountdownColors[Phase]);
                return;
            }

            UpdateCountdown(GetCountdown(Span, DefaultTexts[Phase]), CountdownColors[Phase]);
        }

        private string SetCustomRule(TimeSpan ExamSpan, string Custom)
        {
            CustomTextBuilder.Clear();
            CustomTextBuilder.Append(Custom);
            CustomTextBuilder.Replace(Constants.PH_EXAMNAME, ExamName);
            CustomTextBuilder.Replace(Constants.PH_DAYS, $"{ExamSpan.Days}");
            CustomTextBuilder.Replace(Constants.PH_HOURS, $"{ExamSpan.Hours:00}");
            CustomTextBuilder.Replace(Constants.PH_MINUTES, $"{ExamSpan.Minutes:00}");
            CustomTextBuilder.Replace(Constants.PH_SECONDS, $"{ExamSpan.Seconds:00}");
            CustomTextBuilder.Replace(Constants.PH_CEILINGDAYS, $"{ExamSpan.Days + 1}");
            CustomTextBuilder.Replace(Constants.PH_TOTALHOURS, $"{ExamSpan.TotalHours:0}");
            CustomTextBuilder.Replace(Constants.PH_TOTALMINUTES, $"{ExamSpan.TotalMinutes:0}");
            CustomTextBuilder.Replace(Constants.PH_TOTALSECONDS, $"{ExamSpan.TotalSeconds:0}");
            return CustomTextBuilder.ToString();
        }

        private string GetCountdown(TimeSpan Span, string Hint) => SelectedState switch
        {
            CountdownState.DaysOnly => string.Format("距离{0}{1}{2}天", ExamName, Hint, Span.Days),
            CountdownState.DaysOnlyWithCeiling => string.Format("距离{0}{1}{2}天", ExamName, Hint, Span.Days + 1),
            CountdownState.HoursOnly => string.Format("距离{0}{1}{2:0}小时", ExamName, Hint, Span.TotalHours),
            CountdownState.MinutesOnly => string.Format("距离{0}{1}{2:0}分钟", ExamName, Hint, Span.TotalMinutes),
            CountdownState.SecondsOnly => string.Format("距离{0}{1}{2:0}秒", ExamName, Hint, Span.TotalSeconds),
            _ => string.Format("距离{0}{1}{2}天{3:00}时{4:00}分{5:00}秒", ExamName, Hint, Span.Days, Span.Hours, Span.Minutes, Span.Seconds)
        };

        private void UpdateCountdown(string Content, ColorSetObject Colors)
        {
            BeginInvoke(() =>
            {
                LabelCountdown.Text = Content;
                LabelCountdown.ForeColor = Colors.Fore;
                BackColor = Colors.Back;

                if (ShowTrayText)
                {
                    UpdateTrayIconText(Content);
                }
            });
        }

        private void SetPhase(CountdownPhase phase)
        {
            if (CurrentPhase != phase)
            {
                CurrentPhase = phase;
                RefreshCustomRules();
            }
        }

        private void UpdateTrayIconText(string cText, bool cInvokeRequired = false)
        {
            if (TrayIcon != null)
            {
                cText = cText.Truncate(60);

                if (cInvokeRequired)
                {
                    BeginInvoke(() => TrayIcon.Text = cText);
                }
                else
                {
                    TrayIcon.Text = cText;
                }
            }
        }

        private void CompatibleWithPPTService()
        {
            if (IsPPTService)
            {
                var ValidArea = SelectedScreenRect;

                if (Left == ValidArea.Left && Top == ValidArea.Top)
                {
                    Left = ValidArea.Left + PptsvcThreshold;
                }
            }
        }

        private void SetLabelCountdownAutoWrap()
        {
            SetLabelAutoWrap(LabelCountdown, true);
        }

        private void ApplyLocation()
        {
            if (!IsDraggable)
            {
                Location = CountdownPos switch
                {
                    CountdownPosition.LeftCenter => new(SelectedScreenRect.Left, SelectedScreenRect.Top + SelectedScreenRect.Height / 2 - Height / 2),
                    CountdownPosition.BottomLeft => new(SelectedScreenRect.Left, SelectedScreenRect.Bottom - Height),
                    CountdownPosition.TopCenter => new(SelectedScreenRect.Left + SelectedScreenRect.Width / 2 - Width / 2, SelectedScreenRect.Top),
                    CountdownPosition.Center => GetScreenCenter(SelectedScreenRect),
                    CountdownPosition.BottomCenter => new(SelectedScreenRect.Left + SelectedScreenRect.Width / 2 - Width / 2, SelectedScreenRect.Bottom - Height),
                    CountdownPosition.TopRight => new(SelectedScreenRect.Right - Width, SelectedScreenRect.Top),
                    CountdownPosition.RightCenter => new(SelectedScreenRect.Right - Width, SelectedScreenRect.Top + SelectedScreenRect.Height / 2 - Height / 2),
                    CountdownPosition.BottomRight => new(SelectedScreenRect.Right - Width, SelectedScreenRect.Bottom - Height),
                    _ => IsPPTService ? new(SelectedScreenRect.Location.X + PptsvcThreshold, SelectedScreenRect.Location.Y) : SelectedScreenRect.Location
                };
            }
        }

        private void RefreshScreen()
        {
            SelectedScreenRect = Screen.AllScreens[ScreenIndex].WorkingArea;
        }

        private void RefreshCustomRules()
        {
            CanUseRules = UseCustomText && (CurrentRules = [.. CustomRules.Where(rule => rule.Phase == CurrentPhase).OrderByDescending(x => x)]).Length != 0;
        }

        private void SaveConfig()
        {
            App.AppConfig = AppConfig;
        }

        private void RealSaveConfig()
        {
            ConfigHandler.Save();
        }

        private void SetRoundCorners()
        {
            if (App.OSBuild >= WindowsBuilds.Windows11_21H2)
            {
                RoundCorner.SetRoundCornerModern(Handle);
            }
            else
            {
                SetRoundRegion = true;
            }
        }
    }
}