﻿using Microsoft.Win32;
using PlainCEETimer.Controls;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainCEETimer.Forms
{
    public partial class MainForm : AppForm
    {
        public static bool UniTopMost { get; private set; } = true;
        public static bool IsNormalStart { get; set; }
        public static bool ValidateNeeded { get; private set; } = true;
        public static bool UseClassicContextMenu { get; private set; }
        public static Screen CurrentScreen { get; private set; }

        private bool MemClean;
        private bool IsShowXOnly;
        private bool IsDraggable;
        private bool IsShowEnd;
        private bool IsShowPast;
        private bool IsCeiling;
        private bool IsPPTService;
        private bool IsCustomText;
        private int ScreenIndex;
        private CountdownPosition CountdownPos;
        private int ShowXOnlyIndex;
        private DateTime ExamEndTime;
        private DateTime ExamStartTime;
        private ConfigObject AppConfig;
        private ColorSetObject[] CountdownColors;
        private CustomRuleObject[] CustomRules;
        private string ExamName;
        private string[] CustomText;

        private bool IsReadyToMove;
        private bool IsCountdownReady;
        private bool IsCountdownRunning;
        private bool SetRoundCornerRegion;
        private bool ShowTrayIcon;
        private bool ShowTrayText;
        private bool LoadedMemCleaner;
        private bool AutoSwitch;
        private const int PptsvcThreshold = 1;
        private const int BorderRadius = 13;
        private const int MemCleanerInterval = 300_000; // 5 min
        private CountdownState SelectedState;
        private System.Threading.Timer MemCleaner;
        private Timer AutoSwitchHandler;
        private System.Threading.Timer Countdown;
        private readonly StringBuilder Builder = new();
        private int AutoSwitchInterval;
        private Point LastLocation;
        private Point LastMouseLocation;
        private Rectangle CurrentScreenRect;
        private SettingsForm FormSettings;
        private AboutForm FormAbout;
        private NotifyIcon TrayIcon;

        private bool TrayIconReopen;
        private bool ContextMenuStyleChanged;
        private bool UseClassicContextMenuBak;
        private ExamInfoObject CurrentExam;
        private ExamInfoObject[] Exams;
        private int ExamIndex;

        private ContextMenu ContextMenuMain;
        private ContextMenu ContextMenuTray;
        private Menu.MenuItemCollection ExamSwitchMain;
        private ContextMenuStrip ContextMenuStripMain;
        private ContextMenuStrip ContextMenuStripTray;
        private ToolStripItemCollection ExamSwitchMainStrip;
        private readonly EventHandler OnContextInstDirClick;
        private readonly EventHandler OnContextShowAllClick;
        private readonly EventHandler OnContextRestartClick;
        private readonly EventHandler OnContextQuitClick;

        public MainForm()
        {
            InitializeComponent();
            OnContextInstDirClick = new((_, _) => App.OpenInstallDir());
            OnContextShowAllClick = new((_, _) => App.OnTrayMenuShowAllClicked());
            OnContextRestartClick = new((_, _) => App.Shutdown(true));
            OnContextQuitClick = new((_, _) => App.Shutdown());
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
            Task.Run(() => new Updater().CheckForUpdate(true, this));
            IsNormalStart = true;
        }

        protected override void OnClosing(FormClosingEventArgs e)
        {
            if (App.AllowClosing || e.CloseReason == CloseReason.WindowsShutDown)
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
            if (SetRoundCornerRegion)
            {
                RoundCorner.SetRoundCornerRegion(Handle, Width, Height, BorderRadius.ScaleToDpi());
            }

            ValidateLocation();
        }

        private void MainForm_LocationRefreshed(object sender, EventArgs e)
        {
            AppConfig.Pos = Location;
            SaveConfig();
        }

        private void ExamItems_Click(object sender, EventArgs e)
        {
            int ItemIndex;
            MenuItem Sender = null;
            ToolStripMenuItem SenderStrip = null;

            if (UseClassicContextMenu)
            {
                Sender = (MenuItem)sender;
                ItemIndex = Sender.Index;
            }
            else
            {
                SenderStrip = (ToolStripMenuItem)sender;
                ItemIndex = (int)SenderStrip.Tag;
            }

            if (SenderStrip != null && SenderStrip.Checked)
            {
                SenderStrip.Checked = true;
            }

            if ((SenderStrip != null && !SenderStrip.Checked) || (Sender != null && !Sender.Checked))
            {
                UnselectAllExamItems();
                ExamIndex = ItemIndex;
                AppConfig.General.ExamIndex = ItemIndex;
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
                AppConfig.Pos = Location;
                SaveConfig();
            }

            IsReadyToMove = false;
        }
        #endregion

        private void ContextSettings_Click(object sender, EventArgs e)
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
        }

        private void ContextAbout_Click(object sender, EventArgs e)
        {
            if (FormAbout == null || FormAbout.IsDisposed)
            {
                FormAbout = new();
            }

            FormAbout.ReActivate();
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) App.OnTrayMenuShowAllClicked();
        }

        private void ExamAutoSwitch(object sender, EventArgs e)
        {
            AppConfig.General.ExamIndex = (ExamIndex + 1) % Exams.Length;
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
            LoadExams();
            LoadConfig();
            PrepareCountdown();
            ApplyLocation();
            CompatibleWithPPTService();
            LoadContextMenu();
            LoadTrayIcon();
            App.OnUniTopMostStateChanged();
            SetLabelCountdownAutoWrap();
            TopMost = false;
            TopMost = AppConfig.General.TopMost;
            ShowInTaskbar = !TopMost;
            TryRunCountdown();
        }

        private void TryRunCountdown()
        {
            if (!IsCountdownRunning)
            {
                IsCountdownRunning = true;
                Countdown = new(StartCountdown, null, 0, 1000);
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
            var tmp = AppConfig.General.WCCMS;

            if (UseClassicContextMenu != tmp && !ValidateNeeded)
            {
                UseClassicContextMenuBak = UseClassicContextMenu;
                ContextMenuStyleChanged = true;
            }
            else if (tmp == UseClassicContextMenuBak)
            {
                ContextMenuStyleChanged = false;
            }

            UseClassicContextMenu = tmp;
            CustomText = AppConfig.Display.CustomTexts;
            MemClean = AppConfig.General.MemClean;
            IsShowXOnly = AppConfig.Display.ShowXOnly;
            IsCeiling = AppConfig.Display.Ceiling;
            IsShowEnd = AppConfig.Display.EndIndex is 1 or 2;
            IsShowPast = AppConfig.Display.EndIndex == 2;
            IsDraggable = AppConfig.Display.Draggable;
            UniTopMost = AppConfig.General.UniTopMost;
            IsPPTService = AppConfig.Display.SeewoPptsvc;
            IsCustomText = AppConfig.Display.CustomText;
            ScreenIndex = AppConfig.Display.ScreenIndex - 1;
            CountdownPos = AppConfig.Display.Position;
            ShowXOnlyIndex = AppConfig.Display.X;
            ShowTrayIcon = AppConfig.General.TrayIcon;
            ShowTrayText = AppConfig.General.TrayText;
            CustomRules = AppConfig.CustomRules;
            CountdownColors = AppConfig.Appearance.Colors;
        }

        private void LoadExams()
        {
            AutoSwitch = AppConfig.General.AutoSwitch;
            AutoSwitchInterval = ConfigHandler.GetAutoSwitchInterval(AppConfig.General.Interval);
            Exams = AppConfig.General.ExamInfo;
            var i = AppConfig.General.ExamIndex;
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
            ExamStartTime = CurrentExam.Start;
            ExamEndTime = CurrentExam.End;
            IsCountdownReady = !string.IsNullOrWhiteSpace(ExamName) && ExamStartTime.IsValid() && ExamEndTime.IsValid() && (ExamEndTime > ExamStartTime || !IsShowEnd);
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

            LabelCountdown.Font = AppConfig.Appearance.Font;

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
                Location = AppConfig.Pos;
            }
            else
            {
                RefreshScreen();
            }

            if (MemClean && !LoadedMemCleaner)
            {
                MemCleaner = new((state) => MemoryCleaner.CleanMemory(9437184), null, 3000, MemCleanerInterval);
                LoadedMemCleaner = true;
            }

            if (!MemClean && LoadedMemCleaner)
            {
                MemCleaner.Dispose();
            }
        }

        private void LoadContextMenu()
        {
            if (ContextMenuStyleChanged)
            {
                UseClassicContextMenu = UseClassicContextMenuBak;

                if (MessageX.Warn("由于系统限制，切换右键菜单样式需要重启应用程序后才能生效。\n\n是否立即重启？", Buttons: AppMessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    App.Shutdown(true);
                }
            }

            if (UseClassicContextMenu)
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
                    var ItemIndex = 0;

                    foreach (var Exam in Exams)
                    {
                        var Item = new MenuItem()
                        {
                            Text = $"{ItemIndex + 1}. {Exam}",
                            RadioCheck = true,
                            Checked = ItemIndex == ExamIndex
                        };

                        Item.Click += ExamItems_Click;
                        ExamSwitchMain.Add(Item);
                        ItemIndex++;
                    }
                }
            }
            else
            {
                ContextMenuStripMain = BaseContextMenuStrip();
                ExamSwitchMainStrip = ((ToolStripMenuItem)ContextMenuStripMain.Items[0]).DropDownItems;

                if (ShowTrayIcon)
                {
                    var tmp = BaseContextMenuStrip();
                    tmp.Items.RemoveAt(0);
                    tmp.Items.RemoveAt(0);
                    ContextMenuStripTray = tmp;
                }

                ContextMenuStrip = ContextMenuStripMain;
                LabelCountdown.ContextMenuStrip = ContextMenuStripMain;

                if (Exams.Length != 0)
                {
                    ExamSwitchMainStrip.Clear();
                    var ItemIndex = 0;

                    foreach (var Exam in Exams)
                    {
                        var Item = new ToolStripMenuItem()
                        {
                            Text = $"{ItemIndex + 1}. {Exam}",
                            Checked = ItemIndex == ExamIndex,
                            Tag = ItemIndex
                        };

                        Item.Click += ExamItems_Click;
                        ExamSwitchMainStrip.Add(Item);
                        ItemIndex++;
                    }
                }
            }

            UpdateExamSelection();

            #region 来自网络
            /*

            克隆 (重用) 现有 ContextMenuStrip 实例 参考：

            .net - C# - Duplicate ContextMenuStrip Items into another - Stack Overflow
            https://stackoverflow.com/questions/37884815/c-sharp-duplicate-contextmenustrip-items-into-another

            */

            ContextMenu BaseContextMenu() => CreateNew
            ([
                AddSubMenu(ContextMenuConstants.Switch,
                [
                    AddItem(ContextMenuConstants.AddExamInfo)
                ]),
                AddSeparator(),
                AddItem(ContextMenuConstants.Settings, ContextSettings_Click),
                AddItem(ContextMenuConstants.About, ContextAbout_Click),
                AddSeparator(),
                AddItem(ContextMenuConstants.InstallDir, OnContextInstDirClick)
            ]);

            ContextMenuStrip BaseContextMenuStrip() => CreateNewStrip
            ([
                AddSubStrip(ContextMenuConstants.Switch,
                [
                    AddStripItem(ContextMenuConstants.AddExamInfo)
                ]),
                AddStripSeparator(),
                AddStripItem(ContextMenuConstants.Settings, ContextSettings_Click),
                AddStripItem(ContextMenuConstants.About, ContextAbout_Click),
                AddStripSeparator(),
                AddStripItem(ContextMenuConstants.InstallDir, OnContextInstDirClick)
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
                        if (MessageX.Warn("由于系统限制，重新开关托盘图标需要重启应用程序后方可正常显示。\n\n是否立即重启？", Buttons: AppMessageBoxButtons.YesNo) == DialogResult.Yes)
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
                    };

                    if (UseClassicContextMenu)
                    {
                        TrayIcon.ContextMenu = Merge(ContextMenuTray, CreateNew
                        ([
                            AddSeparator(),
                            AddItem(ContextMenuConstants.Show, OnContextShowAllClick),
                            AddSubMenu(ContextMenuConstants.Close,
                            [
                                AddItem(ContextMenuConstants.Restart, OnContextRestartClick),
                                AddItem(ContextMenuConstants.Quit, OnContextQuitClick)
                            ])
                        ]));
                    }
                    else
                    {
                        TrayIcon.ContextMenuStrip = MergeStrip(ContextMenuStripTray,
                        [
                            AddStripSeparator(),
                            AddStripItem(ContextMenuConstants.Show, OnContextShowAllClick),
                            AddSubStrip(ContextMenuConstants.Close,
                            [
                                AddStripItem(ContextMenuConstants.Restart, OnContextRestartClick),
                                AddStripItem(ContextMenuConstants.Quit, OnContextQuitClick)
                            ])
                        ]);
                    }

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
            if (UseClassicContextMenu)
            {
                foreach (MenuItem Item in ExamSwitchMain)
                {
                    Item.Checked = false;
                }
            }
            else
            {
                foreach (ToolStripMenuItem Item in ExamSwitchMainStrip)
                {
                    Item.Checked = false;
                }
            }
        }

        private void UpdateExamSelection(bool UpdateOnly = false)
        {
            if (ExamIndex != -1)
            {
                if (UseClassicContextMenu)
                {
                    ExamSwitchMain[ExamIndex].Checked = true;
                }
                else
                {
                    ((ToolStripMenuItem)ExamSwitchMainStrip[ExamIndex]).Checked = true;
                }
            }
            else
            {
                if (UseClassicContextMenu)
                {
                    var Item = ExamSwitchMain[0];
                    Item.Checked = false;
                    Item.Enabled = false;
                }
                else
                {
                    var Item = (ToolStripMenuItem)ExamSwitchMainStrip[0];
                    Item.Checked = false;
                    Item.Enabled = false;
                }
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

        private void StartCountdown(object state)
        {
            if (IsCountdownReady && DateTime.Now < ExamStartTime)
            {
                ApplyColorRule(0, ExamStartTime - DateTime.Now, ExamName, Placeholders.PH_START);
            }
            else if (IsCountdownReady && DateTime.Now < ExamEndTime && IsShowEnd)
            {
                ApplyColorRule(1, ExamEndTime - DateTime.Now, ExamName, Placeholders.PH_LEFT);
            }
            else if (IsCountdownReady && DateTime.Now > ExamEndTime && IsShowEnd && IsShowPast)
            {
                ApplyColorRule(2, DateTime.Now - ExamEndTime, ExamName, Placeholders.PH_PAST);
            }
            else
            {
                Countdown.Dispose();
                UpdateCountdown("欢迎使用高考倒计时", CountdownColors[3].Fore, CountdownColors[3].Back);
                UpdateTrayIconText(App.AppName, true);
                IsCountdownRunning = false;
            }
        }

        private void ApplyColorRule(int Phase, TimeSpan Span, string Name, string Hint)
        {
            if (IsCustomText)
            {
                var r = CustomRules.Where(x => (int)x.Phase == Phase);
                var Rules = Phase == 2 ? r.OrderByDescending(x => x.Tick) : r.OrderBy(x => x.Tick);

                if (Rules.Count() > 0)
                {
                    foreach (var Rule in Rules)
                    {
                        if (Phase == 2 ? (Span >= Rule.Tick) : (Span <= Rule.Tick + new TimeSpan(0, 0, 0, 1)))
                        {
                            SetCountdown(Span, Name, Hint, Rule.Fore, Rule.Back, Rule.Text);
                            return;
                        }
                    }
                }
            }

            SetCountdown(Span, Name, Hint, CountdownColors[Phase].Fore, CountdownColors[Phase].Back, CustomText[Phase]);
        }

        private void ApplyLocation()
        {
            if (!IsDraggable)
            {
                Location = CountdownPos switch
                {
                    CountdownPosition.LeftCenter => new(CurrentScreenRect.Left, CurrentScreenRect.Top + CurrentScreenRect.Height / 2 - Height / 2),
                    CountdownPosition.BottomLeft => new(CurrentScreenRect.Left, CurrentScreenRect.Bottom - Height),
                    CountdownPosition.TopCenter => new(CurrentScreenRect.Left + CurrentScreenRect.Width / 2 - Width / 2, CurrentScreenRect.Top),
                    CountdownPosition.Center => new(CurrentScreenRect.Left + CurrentScreenRect.Width / 2 - Width / 2, CurrentScreenRect.Top + CurrentScreenRect.Height / 2 - Height / 2),
                    CountdownPosition.BottomCenter => new(CurrentScreenRect.Left + CurrentScreenRect.Width / 2 - Width / 2, CurrentScreenRect.Bottom - Height),
                    CountdownPosition.TopRight => new(CurrentScreenRect.Right - Width, CurrentScreenRect.Top),
                    CountdownPosition.RightCenter => new(CurrentScreenRect.Right - Width, CurrentScreenRect.Top + CurrentScreenRect.Height / 2 - Height / 2),
                    CountdownPosition.BottomRight => new(CurrentScreenRect.Right - Width, CurrentScreenRect.Bottom - Height),
                    _ => IsPPTService ? new(CurrentScreenRect.Location.X + PptsvcThreshold, CurrentScreenRect.Location.Y) : CurrentScreenRect.Location
                };
            }
        }

        private void SetCountdown(TimeSpan Span, string Name, string Hint, Color Fore, Color Back, string Custom)
        {
            UpdateCountdown(IsCustomText ? GetCountdownWithCustomText(Span, Name, Custom) : GetCountdown(Span, Name, Hint), Fore, Back);
        }

        private string GetCountdownWithCustomText(TimeSpan Span, string Name, string Custom)
        {
            Builder.Clear();
            Builder.Append(Custom);
            Builder.Replace(Placeholders.PH_EXAMNAME, Name);
            Builder.Replace(Placeholders.PH_DAYS, $"{Span.Days}");
            Builder.Replace(Placeholders.PH_HOURS, $"{Span.Hours:00}");
            Builder.Replace(Placeholders.PH_MINUTES, $"{Span.Minutes:00}");
            Builder.Replace(Placeholders.PH_SECONDS, $"{Span.Seconds:00}");
            Builder.Replace(Placeholders.PH_CEILINGDAYS, $"{Span.Days + 1}");
            Builder.Replace(Placeholders.PH_TOTALHOURS, $"{Span.TotalHours:0}");
            Builder.Replace(Placeholders.PH_TOTALMINUTES, $"{Span.TotalMinutes:0}");
            Builder.Replace(Placeholders.PH_TOTALSECONDS, $"{Span.TotalSeconds:0}");
            return Builder.ToString();
        }

        private string GetCountdown(TimeSpan Span, string Name, string Hint) => SelectedState switch
        {
            CountdownState.DaysOnly => string.Format("{0}{1}{2}{3}天", Placeholders.PH_JULI, Name, Hint, Span.Days),
            CountdownState.DaysOnlyWithCeiling => string.Format("{0}{1}{2}{3}天", Placeholders.PH_JULI, Name, Hint, Span.Days + 1),
            CountdownState.HoursOnly => string.Format("{0}{1}{2}{3:0}小时", Placeholders.PH_JULI, Name, Hint, Span.TotalHours),
            CountdownState.MinutesOnly => string.Format("{0}{1}{2}{3:0}分钟", Placeholders.PH_JULI, Name, Hint, Span.TotalMinutes),
            CountdownState.SecondsOnly => string.Format("{0}{1}{2}{3:0}秒", Placeholders.PH_JULI, Name, Hint, Span.TotalSeconds),
            _ => string.Format("{0}{1}{2}{3}天{4:00}时{5:00}分{6:00}秒", Placeholders.PH_JULI, Name, Hint, Span.Days, Span.Hours, Span.Minutes, Span.Seconds)
        };


        private void UpdateCountdown(string CountdownText, Color Fore, Color Back)
        {
            BeginInvoke(() =>
            {
                LabelCountdown.Text = CountdownText;
                LabelCountdown.ForeColor = Fore;
                BackColor = Back;

                if (ShowTrayText)
                {
                    UpdateTrayIconText(CountdownText);
                }
            });
        }

        private void UpdateTrayIconText(string cText, bool cInvokeRequired = false)
        {
            if (TrayIcon != null)
            {
                cText = cText.Truncate(60);

                if (cInvokeRequired)
                {
                    BeginInvoke(() =>
                    {
                        TrayIcon.Text = cText;
                    });
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
                var ValidArea = GetScreenRect();

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

        private void RefreshScreen()
        {
            CurrentScreenRect = GetScreenRect(ScreenIndex);
            CurrentScreen = Screen.FromControl(this);
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
                SetRoundCornerRegion = true;
            }
        }
    }
}