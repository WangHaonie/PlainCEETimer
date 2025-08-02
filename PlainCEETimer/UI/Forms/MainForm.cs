using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Forms
{
    public sealed class MainForm() : AppForm(AppFormParam.Special | AppFormParam.RoundCorner)
    {
        public static bool UniTopMost { get; private set; } = true;
        public static bool ValidateNeeded { get; private set; } = true;
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
        private bool ShowTrayIcon;
        private bool ShowTrayText;
        private bool TrayIconReopen;
        private bool UseCustomText;
        private int AutoSwitchInterval;
        private int CurrentTheme;
        private int CountdownMaxW;
        private int ExamIndex;
        private int ScreenIndex;
        private int ShowXOnlyIndex;
        private int LastMouseX;
        private int LastMouseY;
        private const int PptsvcThreshold = 1;
        private const int MemCleanerInterval = 300_000; // 5 min
        private string CountdownContent;
        private string CountdownContentLast = string.Empty;
        private string ExamName;
        private string[] GlobalTexts;
        private CountdownMode Mode;
        private CountdownPosition CountdownPos;
        private CountdownPhase CurrentPhase = CountdownPhase.None;
        private CountdownState SelectedState;
        private Color CountdownForeColor;
        private ColorSetObject[] CountdownColors;
        private DateTime Now;
        private DateTime ExamEnd;
        private DateTime ExamStart;
        private Point LastLocation;
        private Rectangle SelectedScreenRect;
        private AboutForm FormAbout;
        private ConfigObject AppConfig;
        private ContextMenu ContextMenuMain;
        private ContextMenu ContextMenuTray;
        private CustomRuleObject[] CustomRules;
        private CustomRuleObject[] CurrentRules;
        private ExamInfoObject CurrentExam;
        private ExamInfoObject[] Exams;
        private Font CountdownFont;
        private MatchEvaluator DefaultMatchEvaluator;
        private Menu.MenuItemCollection ExamSwitchMain;
        private NotifyIcon TrayIcon;
        private SettingsForm FormSettings;
        private System.Threading.Timer MemCleaner;
        private System.Threading.Timer Countdown;
        private System.Windows.Forms.Timer AutoSwitchHandler;
        private readonly Dictionary<string, string> CDPlaceholders = new(10);
        private readonly Regex CDRegEx = new(Validator.RegexPhPatterns, RegexOptions.Compiled);
        private readonly string[] DefaultTexts = [Constants.PH_START, Constants.PH_LEFT, Constants.PH_PAST];

        protected override void OnInitializing()
        {
            Text = "高考倒计时";
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            DefaultMatchEvaluator = m =>
            {
                var key = m.Value;
                return CDPlaceholders.TryGetValue(key, out string value) ? value : key;
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            TextRenderer.DrawText(g, CountdownContent, CountdownFont, ClientRectangle, CountdownForeColor, TextFormatFlags.Left | TextFormatFlags.WordBreak);
        }

        protected override void OnShown()
        {
            RefreshSettings();
            ValidateNeeded = false;
            new Action(() => new Updater().CheckForUpdate(false, this)).Start();
            new Action(Startup.RefreshTaskState).Start();
        }

        /*
        
        无边框窗口的拖动 参考:

        C#创建无边框可拖动窗口 - 掘金
        https://juejin.cn/post/6989144829607280648

        */

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (IsDraggable && e.Button == MouseButtons.Left)
            {
                IsReadyToMove = true;
                Cursor = Cursors.SizeAll;
                LastMouseX = e.X;
                LastMouseY = e.Y;
                LastLocation = Location;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsDraggable && IsReadyToMove)
            {
                SetLocation(MousePosition.X - LastMouseX, MousePosition.Y - LastMouseY);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (IsDraggable)
            {
                Cursor = Cursors.Default;

                if (IsReadyToMove && Location != LastLocation)
                {
                    KeepOnScreen();
                    CompatibleWithPPTService();
                    SetCountdownAutoWrap();
                    AppConfig.Location = Location;
                    SaveConfig();
                }

                IsReadyToMove = false;
            }

            base.OnMouseUp(e);
        }

        protected override bool OnClosing(CloseReason closeReason)
        {
            return closeReason != CloseReason.WindowsShutDown;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            RefreshScreen();
            SetCountdownAutoWrap();
            ApplyLocation();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ValidateLocation();
        }

        private void MainForm_LocationRefreshed()
        {
            AppConfig.Location = Location;
            SaveConfig();
        }

        private void ExamItems_Click(object sender, EventArgs e)
        {
            int index;
            var item = (MenuItem)sender;
            index = item.Index;

            if (item != null && !item.Checked)
            {
                UnselectAllExamItems();
                ExamIndex = index;
                AppConfig.ExamIndex = index;
                SaveConfig();
                LoadExams();
                TryRunCountdown();
                UpdateExamSelection();
                CountdownCallback(null);
            }
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                App.OnTrayMenuShowAllClicked();
            }
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
            SetCountdownAutoWrap();
            TopMost = false;
            TopMost = AppConfig.General.TopMost;
            ShowInTaskbar = !TopMost;
            TryRunCountdown();

            var newTheme = AppConfig.Dark;

            if (!ValidateNeeded && ThemeManager.IsThemeChanged(CurrentTheme, newTheme))
            {
                MessageX.Warn("由于更改了应用主题设置，需要立即重启倒计时！");
                App.Exit(true);
            }

            CurrentTheme = newTheme;
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

            var endIndex = AppConfig.Display.EndIndex;
            Mode = endIndex == 2 ? CountdownMode.Mode3 : (endIndex is 1 or 2 ? CountdownMode.Mode2 : CountdownMode.Mode1);
        }

        private void LoadExams()
        {
            AutoSwitch = AppConfig.General.AutoSwitch;
            AutoSwitchInterval = GetAutoSwitchInterval(AppConfig.General.Interval);
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
                    _ => IsCeiling ? CountdownState.DaysOnlyCeiling : CountdownState.DaysOnly
                };
            }

            CountdownFont = AppConfig.Font;
            LocationRefreshed -= MainForm_LocationRefreshed;

            if (IsDraggable)
            {
                LocationRefreshed += MainForm_LocationRefreshed;
                Location = AppConfig.Location;
            }
            else
            {
                RefreshScreen();
            }

            if (MemClean && !LoadedMemCleaner)
            {
                MemCleaner = new(_ => MemoryCleaner.Clean(), null, 3000, MemCleanerInterval);
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

            if (Exams.Length != 0)
            {
                ExamSwitchMain.Clear();

                for (int i = 0; i < Exams.Length; i++)
                {
                    var item = new MenuItem()
                    {
                        Text = $"{i + 1}. {Exams[i]}",
                        RadioCheck = true,
                        Checked = i == ExamIndex
                    };

                    item.Click += ExamItems_Click;
                    ExamSwitchMain.Add(item);
                }
            }

            UpdateExamSelection();

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
                                ConfigHandler.Save();
                                RefreshSettings();
                                CountdownCallback(null);
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
                b.Item("安装目录(&D)", (_, _) => Process.Start(App.CurrentExecutableDir))
            ]);
        }

        private void LoadTrayIcon()
        {
            if (TrayIcon == null)
            {
                if (ShowTrayIcon)
                {
                    if (TrayIconReopen)
                    {
                        if (MessageX.Warn("由于系统限制，重新开关托盘图标需要重启应用程序后方可正常显示。\n\n是否立即重启？", MessageButtons.YesNo) == DialogResult.Yes)
                        {
                            App.Exit(true);
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
                            b.Item("显示界面(&X)", (_, _) => App.OnTrayMenuShowAllClicked()),
                            b.Menu("关闭(&C)",
                            [
                                b.Item("重启(&R)", (_, _) => App.Exit(true)),
                                b.Item("退出(&Q)", (_, _) => App.Exit())
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
            var length = ExamSwitchMain.Count;

            for (int i = 0; i < length; i++)
            {
                ExamSwitchMain[i].Checked = false;
            }
        }

        private void UpdateExamSelection(bool canUpdate = false)
        {
            if (ExamIndex != -1)
            {
                ExamSwitchMain[ExamIndex].Checked = true;
            }
            else
            {
                var item = ExamSwitchMain[0];
                item.Checked = false;
                item.Enabled = false;
            }

            if (!canUpdate && AutoSwitch)
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
            if (IsCountdownReady)
            {
                Now = DateTime.Now;

                if (Mode >= CountdownMode.Mode1 && Now < ExamStart)
                {
                    SetPhase(CountdownPhase.P1);
                    ApplyCustomRule(0, ExamStart - Now);
                    return;
                }

                if (Mode >= CountdownMode.Mode2 && Now < ExamEnd)
                {
                    SetPhase(CountdownPhase.P2);
                    ApplyCustomRule(1, ExamEnd - Now);
                    return;
                }

                if (Mode >= CountdownMode.Mode3 && Now > ExamEnd)
                {
                    SetPhase(CountdownPhase.P3);
                    ApplyCustomRule(2, Now - ExamEnd);
                    return;
                }
            }

            Countdown.Dispose();
            UpdateCountdown("欢迎使用高考倒计时", CountdownColors[3]);
            UpdateTrayIconText(App.AppName, true);
            IsCountdownRunning = false;
        }

        private void ApplyCustomRule(int phase, TimeSpan span)
        {
            CDPlaceholders[Constants.PH_EXAMNAME] = ExamName;
            CDPlaceholders[Constants.PH_DAYS] = $"{span.Days}";
            CDPlaceholders[Constants.PH_HOURS] = $"{span.Hours:00}";
            CDPlaceholders[Constants.PH_MINUTES] = $"{span.Minutes:00}";
            CDPlaceholders[Constants.PH_SECONDS] = $"{span.Seconds:00}";
            CDPlaceholders[Constants.PH_CEILINGDAYS] = $"{span.Days + 1}";
            CDPlaceholders[Constants.PH_TOTALHOURS] = $"{span.TotalHours:0}";
            CDPlaceholders[Constants.PH_TOTALMINUTES] = $"{span.TotalMinutes:0}";
            CDPlaceholders[Constants.PH_TOTALSECONDS] = $"{span.TotalSeconds:0}";

            if (UseCustomText)
            {
                if (CanUseRules)
                {
                    foreach (var rule in CurrentRules)
                    {
                        if (phase == 2 ? (span >= rule.Tick) : (span <= rule.Tick))
                        {
                            UpdateCountdown(CDRegEx.Replace(rule.Text, DefaultMatchEvaluator), rule.Colors);
                            return;
                        }
                    }
                }

                UpdateCountdown(CDRegEx.Replace(GlobalTexts[phase], DefaultMatchEvaluator), CountdownColors[phase]);
            }
            else
            {
                CDPlaceholders["{ht}"] = DefaultTexts[phase];
                UpdateCountdown(CDRegEx.Replace(GetDefaultText(), DefaultMatchEvaluator), CountdownColors[phase]);
            }
        }

        private string GetDefaultText() => SelectedState switch
        {
            CountdownState.DaysOnly => "距离{x}{ht}{d}天",
            CountdownState.DaysOnlyCeiling => "距离{x}{ht}{cd}天",
            CountdownState.HoursOnly => "距离{x}{ht}{th}小时",
            CountdownState.MinutesOnly => "距离{x}{ht}{tm}分钟",
            CountdownState.SecondsOnly => "距离{x}{ht}{ts}秒",
            _ => "距离{x}{ht}{d}天{h}时{m}分{s}秒"
        };

        private int GetAutoSwitchInterval(int Index) => Index switch
        {
            1 => 15_000, // 15 s
            2 => 30_000, // 30 s
            3 => 45_000, // 45 s
            4 => 60_000, // 1 min
            5 => 120_000, // 2 min
            6 => 180_000, // 3 min
            7 => 300_000, // 5 min
            8 => 600_000, // 10 min
            9 => 900_000, // 15 min
            10 => 1800_000, // 30 min
            11 => 2700_000, // 45 min
            12 => 3600_000, // 1 h
            _ => 10_000 // 10 s
        };

        private void UpdateCountdown(string content, ColorSetObject colors)
        {
            BeginInvoke(() =>
            {
                if (content != CountdownContentLast)
                {
                    CountdownContent = content;
                    CountdownContentLast = content;
                    CountdownForeColor = colors.Fore;
                    BackColor = colors.Back;
                    Size = TextRenderer.MeasureText(CountdownContent, CountdownFont, new(CountdownMaxW, 0), TextFormatFlags.WordBreak);
                    Invalidate();

                    if (ShowTrayText)
                    {
                        UpdateTrayIconText(content);
                    }
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

        private void UpdateTrayIconText(string content, bool invokeNeeded = false)
        {
            if (TrayIcon != null)
            {
                content = content.Truncate(60);

                if (invokeNeeded)
                {
                    BeginInvoke(() => TrayIcon.Text = content);
                }
                else
                {
                    TrayIcon.Text = content;
                }
            }
        }

        private void CompatibleWithPPTService()
        {
            if (IsPPTService)
            {
                var screenRect = SelectedScreenRect;
                var screenRectX = screenRect.X;

                if (Top == screenRect.Y && Left == screenRectX)
                {
                    Left = screenRectX + PptsvcThreshold;
                }
            }
        }

        private void SetCountdownAutoWrap()
        {
            CountdownMaxW = GetCurrentScreenRect().Width - 10;
        }

        private void ApplyLocation()
        {
            if (!IsDraggable)
            {
                switch (CountdownPos)
                {
                    case CountdownPosition.LeftCenter:
                        SetLocation(SelectedScreenRect.X, SelectedScreenRect.Y + (SelectedScreenRect.Height - Height) / 2);
                        break;
                    case CountdownPosition.BottomLeft:
                        SetLocation(SelectedScreenRect.X, SelectedScreenRect.Bottom - Height);
                        break;
                    case CountdownPosition.TopCenter:
                        SetLocation(SelectedScreenRect.X + (SelectedScreenRect.Width - Width) / 2, SelectedScreenRect.Y);
                        break;
                    case CountdownPosition.Center:
                        MoveToScreenCenter(SelectedScreenRect);
                        break;
                    case CountdownPosition.BottomCenter:
                        SetLocation(SelectedScreenRect.X + (SelectedScreenRect.Width - Width) / 2, SelectedScreenRect.Bottom - Height);
                        break;
                    case CountdownPosition.TopRight:
                        SetLocation(SelectedScreenRect.Right - Width, SelectedScreenRect.Y);
                        break;
                    case CountdownPosition.RightCenter:
                        SetLocation(SelectedScreenRect.Right - Width, SelectedScreenRect.Y + (SelectedScreenRect.Height - Height) / 2);
                        break;
                    case CountdownPosition.BottomRight:
                        SetLocation(SelectedScreenRect.Right - Width, SelectedScreenRect.Bottom - Height);
                        break;
                    default:
                        SetLocation(IsPPTService ? SelectedScreenRect.X + PptsvcThreshold : SelectedScreenRect.X, SelectedScreenRect.Y);
                        break;
                }
            }
        }

        private void RefreshScreen()
        {
            var screens = Screen.AllScreens;

            if (ScreenIndex < 0 || ScreenIndex >= screens.Length)
            {
                ScreenIndex = 0;
                AppConfig.Display.ScreenIndex = 0;
            }

            SelectedScreenRect = screens[ScreenIndex].WorkingArea;
        }

        private void RefreshCustomRules()
        {
            CurrentRules = [.. CustomRules.Where(rule => rule.Phase == CurrentPhase).OrderByDescending(x => x)];
            CanUseRules = UseCustomText && CurrentRules.Length != 0;
        }

        private void SaveConfig()
        {
            App.AppConfig = AppConfig;
        }
    }
}