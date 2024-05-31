﻿using CEETimerCSharpWinForms.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CEETimerCSharpWinForms.Forms
{
    public partial class FormMain : Form
    {
        public static bool IsUniTopMost { get; private set; } = true;

        private bool IsFeatureMOEnabled;
        private bool IsShowOnly;
        private bool IsDragable;
        private bool IsShowEnd;
        private bool IsShowPast;
        private bool IsRounding;
        private bool IsPPTService;
        private int ScreenIndex;
        private int PositionIndex;
        private int ShowOnlyIndex;
        private DateTime ExamEndTime;
        private DateTime ExamStartTime;
        private Font SelectedFont;
        private FontStyle SelectedFontStyle;
        private readonly List<PairItems<Color, Color>> CountdownColors = [];
        private List<PairItems<Color, Color>> DefaultColors;
        private string ExamName;

        private enum CountdownState
        {
            Normal,
            DaysOnly,
            DaysOnlyWithRounding,
            HoursOnly,
            MinutesOnly,
            SecondsOnly
        }

        private bool IsReadyToMove;
        private bool IsReady;
        private bool IsWin10BelowRounded;
        private readonly int PptsvcThreshold = 1;
        private readonly int BorderRadius = 13;
        private CountdownState SelectedState;
        private Timer TimerCountdown;
        private System.Threading.Timer TimerMORunner;
        private Point LastLocation;
        private Point LastMouseLocation;
        private Rectangle SelectedScreen;
        private FormSettings formSettings;
        private FormAbout formAbout;
        private readonly ConfigManager configManager = new();
        private readonly FontConverter fontConverter = new();

        public FormMain()
        {
            InitializeComponent();
            FormClosed += (sender, e) => FormManager.Remove(this);
            SizeChanged += FormMain_SizeChanged;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            DefaultColors = [new(Color.Red, Color.White), new(Color.Green, Color.White), new(Color.Black, Color.White), new(Color.Black, Color.White)];
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            SetFormRounded();
            RefreshSettings(sender, e);
            TimerCountdown = new() { Interval = 1000 };
            TimerCountdown.Tick += StartCountdown;
            TimerCountdown.Start();
            LabelCountdown.ForeColor = CountdownColors[3].Item1;
            BackColor = CountdownColors[3].Item2;
            Task.Run(() => UpdateChecker.CheckUpdate(true, this));
            _ = 1.WithDpi(this);
            FormManager.Add(this);
        }

        private void SetFormRounded()
        {
            if (Environment.OSVersion.Version > new Version(10, 0, 21999))
            {
                var attribute = WindowsAPI.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = WindowsAPI.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                WindowsAPI.DwmSetWindowAttribute(Handle, attribute, ref preference, sizeof(uint));
            }
            else
            {
                IsWin10BelowRounded = true;
            }
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (IsWin10BelowRounded)
            {
                var _BorderRadius = BorderRadius.WithDpi(this);
                Region = Region.FromHrgn(WindowsAPI.CreateRoundRectRgn(0, 0, Width, Height, _BorderRadius, _BorderRadius));
            }
        }

        private void RefreshSettings(object sender, EventArgs e)
        {
            configManager.MountConfig(true);

            ExamName = configManager.ReadConfig(ConfigItems.ExamName);
            TopMost = !bool.TryParse(configManager.ReadConfig(ConfigItems.TopMost), out bool tmpa) || tmpa;
            IsFeatureMOEnabled = bool.TryParse(configManager.ReadConfig(ConfigItems.MemOpti), out bool tmpc) && tmpc;
            IsShowOnly = bool.TryParse(configManager.ReadConfig(ConfigItems.ShowOnly), out bool tmpd) && tmpd;
            IsRounding = bool.TryParse(configManager.ReadConfig(ConfigItems.Rounding), out bool tmpe) && tmpe;
            IsShowPast = bool.TryParse(configManager.ReadConfig(ConfigItems.ShowPast), out bool tmpg) && tmpg;
            IsShowEnd = bool.TryParse(configManager.ReadConfig(ConfigItems.ShowEnd), out bool tmpf) && tmpf;
            IsDragable = bool.TryParse(configManager.ReadConfig(ConfigItems.Dragable), out bool tmph) && tmph;
            IsUniTopMost = bool.TryParse(configManager.ReadConfig(ConfigItems.UniTopMost), out bool tmpi) && tmpi;
            IsPPTService = bool.TryParse(configManager.ReadConfig(ConfigItems.SeewoPptSvc), out bool tmpj) && tmpj;
            ScreenIndex = int.TryParse(configManager.ReadConfig(ConfigItems.Screen), out int tmpk) ? tmpk : 0;
            PositionIndex = int.TryParse(configManager.ReadConfig(ConfigItems.Position), out int tmpu) ? tmpu : 0;
            ShowOnlyIndex = int.TryParse(configManager.ReadConfig(ConfigItems.ShowValue), out int tmpl) ? tmpl : 0;

            for (int i = 0; i < 4; i++)
            {
                var Fore = ColorHelper.TryParseRGB($"Fore{i + 1}", out Color tmpfore) ? tmpfore : DefaultColors[i].Item1;
                var Back = ColorHelper.TryParseRGB($"Back{i + 1}", out Color tmpback) ? tmpback : DefaultColors[i].Item2;

                if (!ColorHelper.IsNiceContrast(Fore, Back))
                {
                    Fore = DefaultColors[i].Item1;
                    Back = DefaultColors[i].Item2;
                }

                CountdownColors.Add(new(Fore, Back));
            }

#if DEBUG
            Console.WriteLine("##########################");
#endif

            ExamStartTime = DateTime.TryParseExact(configManager.ReadConfig(ConfigItems.StartTime), "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime tmpw) ? tmpw : DateTime.Now;
            ExamEndTime = DateTime.TryParseExact(configManager.ReadConfig(ConfigItems.EndTime), "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime tmpx) ? tmpx : DateTime.Now;
            int.TryParse(configManager.ReadConfig(ConfigItems.PosX), out int x);
            int.TryParse(configManager.ReadConfig(ConfigItems.PosY), out int y);

            ShowInTaskbar = !TopMost;
            IsShowPast = IsShowPast && IsShowEnd;
            IsRounding = IsRounding && IsShowOnly && ShowOnlyIndex == 0;
            IsUniTopMost = IsUniTopMost && TopMost;
            if (ScreenIndex < 0 || ScreenIndex > Screen.AllScreens.Length) ScreenIndex = 0;
            if (PositionIndex < 0 || PositionIndex > 8) PositionIndex = 0;
            if (ShowOnlyIndex > 3) ShowOnlyIndex = 0;
            if (ExamName.Length > ConfigPolicy.MaxExamNameLength || ExamName.Length < ConfigPolicy.MinExamNameLength) ExamName = "";
            IsReady = !string.IsNullOrWhiteSpace(ExamName) && configManager.IsValidData(tmpw) && configManager.IsValidData(tmpx) && (tmpx > tmpw || !IsShowEnd);
            IsPPTService = IsPPTService && ((TopMost && ShowOnlyIndex == 0) || IsDragable);

            SelectedState = CountdownState.Normal;

            if (IsShowOnly)
            {
                SelectedState = ShowOnlyIndex switch
                {
                    0 => IsRounding ? CountdownState.DaysOnlyWithRounding : CountdownState.DaysOnly,
                    1 => CountdownState.HoursOnly,
                    2 => CountdownState.MinutesOnly,
                    3 => CountdownState.SecondsOnly,
                    _ => throw new Exception()
                };
            }

            try
            {
                SelectedFont = (Font)fontConverter.ConvertFromString(configManager.ReadConfig(ConfigItems.Font));
                SelectedFontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), configManager.ReadConfig(ConfigItems.FontStyle));

                if (SelectedFont.Size > ConfigPolicy.MaxFontSize || SelectedFont.Size < ConfigPolicy.MinFontSize)
                {
                    throw new Exception();
                }
            }
            catch
            {
                SelectedFont = (Font)fontConverter.ConvertFromString(ConfigPolicy.DefaultFont);
                SelectedFontStyle = FontStyle.Bold;
            }

            LabelCountdown.Font = new(SelectedFont, SelectedFontStyle);

            LabelCountdown.MouseDown -= LabelCountdown_MouseDown;
            LabelCountdown.MouseMove -= LabelCountdown_MouseMove;
            LabelCountdown.MouseUp -= LabelCountdown_MouseUp;

            if (IsDragable)
            {
                LabelCountdown.MouseDown += LabelCountdown_MouseDown;
                LabelCountdown.MouseMove += LabelCountdown_MouseMove;
                LabelCountdown.MouseUp += LabelCountdown_MouseUp;
                Location = new(x, y);
            }
            else
            {
                RefreshScreen();
            }

            ApplyLocation();
            CompatibleWithPPTService();

            var OpeningForms = Application.OpenForms.Cast<Form>().ToList();
            foreach (Form form in OpeningForms)
            {
                if (form == this) continue;
                form.TopMost = IsUniTopMost;
            }

            TimerMORunner?.Dispose();

            if (IsFeatureMOEnabled)
                TimerMORunner = new(OptimizeMemory, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            configManager.MountConfig(false);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            RefreshScreen();
        }

        #region 来自网络
        /*
        
        无边框窗口的拖动 参考：

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
            IsReadyToMove = false;
            Cursor = Cursors.Default;

            if (LastLocation != Location)
            {
                KeepOnScreen();
                CompatibleWithPPTService();
                SaveLocation();
            }
        }
        #endregion

        private void ContextMenuSettings_Click(object sender, EventArgs e)
        {
            if (formSettings == null || formSettings.IsDisposed)
            {
                formSettings = new()
                {
                    FeatureMOEnabled = IsFeatureMOEnabled,
                    TopMostChecked = TopMost,
                    ExamStartTime = ExamStartTime,
                    ExamEndTime = ExamEndTime,
                    CountdownFont = LabelCountdown.Font,
                    CountdownFontStyle = LabelCountdown.Font.Style,
                    ExamName = ExamName,
                    IsShowOnly = IsShowOnly,
                    ShowOnlyIndex = ShowOnlyIndex,
                    IsShowEnd = IsShowEnd,
                    IsShowPast = IsShowPast,
                    IsRounding = IsRounding,
                    IsDragable = IsDragable,
                    IsPPTService = IsPPTService,
                    ScreenIndex = ScreenIndex,
                    PositionIndex = PositionIndex,
                    DefaultColors = DefaultColors,
                    CountdownColors = CountdownColors
                };

                formSettings.ConfigChanged += RefreshSettings;
            }

            formSettings.WindowState = FormWindowState.Normal;
            formSettings.Show();
            formSettings.Activate();
        }

        private void ContextMenuAbout_Click(object sender, EventArgs e)
        {
            if (formAbout == null || formAbout.IsDisposed)
            {
                formAbout = new();
            }

            formAbout.WindowState = FormWindowState.Normal;
            formAbout.Show();
            formAbout.Activate();
        }

        private void ContextMenuOpenDir_Click(object sender, EventArgs e)
        {
            Process.Start(LaunchManager.CurrentExecutablePath);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = e.CloseReason != CloseReason.WindowsShutDown;
        }

        private void StartCountdown(object sender, EventArgs e)
        {
            if (IsReady && DateTime.Now < ExamStartTime)
            {
                TimeSpan TimeLeft = ExamStartTime - DateTime.Now;
                LabelCountdown.ForeColor = CountdownColors[0].Item1;
                BackColor = CountdownColors[0].Item2;

                LabelCountdown.Text = SelectedState switch
                {
                    CountdownState.Normal => $"距离{ExamName}还有{TimeLeft.Days}天{TimeLeft.Hours:00}时{TimeLeft.Minutes:00}分{TimeLeft.Seconds:00}秒",
                    CountdownState.DaysOnly => $"距离{ExamName}还有{TimeLeft.Days}天",
                    CountdownState.DaysOnlyWithRounding => $"距离{ExamName}还有{TimeLeft.Days + 1}天",
                    CountdownState.HoursOnly => $"距离{ExamName}还有{TimeLeft.TotalHours:0}小时",
                    CountdownState.MinutesOnly => $"距离{ExamName}还有{TimeLeft.TotalMinutes:0}分钟",
                    CountdownState.SecondsOnly => $"距离{ExamName}还有{TimeLeft.TotalSeconds:0}秒",
                    _ => throw new Exception()
                };
            }
            else if (IsReady && IsShowEnd && DateTime.Now >= ExamStartTime && DateTime.Now < ExamEndTime)
            {
                TimeSpan TimeLeftPast = ExamEndTime - DateTime.Now;
                LabelCountdown.ForeColor = CountdownColors[1].Item1;
                BackColor = CountdownColors[1].Item2;

                LabelCountdown.Text = SelectedState switch
                {
                    CountdownState.Normal => $"距离{ExamName}结束还有{TimeLeftPast.Days}天{TimeLeftPast.Hours:00}时{TimeLeftPast.Minutes:00}分{TimeLeftPast.Seconds:00}秒",
                    CountdownState.DaysOnly => $"距离{ExamName}结束还有{TimeLeftPast.Days}天",
                    CountdownState.DaysOnlyWithRounding => $"距离{ExamName}结束还有{TimeLeftPast.Days + 1}天",
                    CountdownState.HoursOnly => $"距离{ExamName}结束还有{TimeLeftPast.TotalHours:0}小时",
                    CountdownState.MinutesOnly => $"距离{ExamName}结束还有{TimeLeftPast.TotalMinutes:0}分钟",
                    CountdownState.SecondsOnly => $"距离{ExamName}结束还有{TimeLeftPast.TotalSeconds:0}秒",
                    _ => throw new Exception()
                };
            }
            else if (IsReady && IsShowEnd && DateTime.Now >= ExamEndTime && IsShowPast)
            {
                TimeSpan TimePast = DateTime.Now - ExamEndTime;
                LabelCountdown.ForeColor = CountdownColors[2].Item1;
                BackColor = CountdownColors[2].Item2;

                LabelCountdown.Text = SelectedState switch
                {
                    CountdownState.Normal => $"距离{ExamName}已过去了{TimePast.Days}天{TimePast.Hours:00}时{TimePast.Minutes:00}分{TimePast.Seconds:00}秒",
                    CountdownState.DaysOnly => $"距离{ExamName}已过去了{TimePast.Days}天",
                    CountdownState.DaysOnlyWithRounding => $"距离{ExamName}已过去了{TimePast.Days + 1}天",
                    CountdownState.HoursOnly => $"距离{ExamName}已过去了{TimePast.TotalHours:0}小时",
                    CountdownState.MinutesOnly => $"距离{ExamName}已过去了{TimePast.TotalMinutes:0}分钟",
                    CountdownState.SecondsOnly => $"距离{ExamName}已过去了{TimePast.TotalSeconds:0}秒",
                    _ => throw new Exception()
                };
            }
            else
            {
                LabelCountdown.ForeColor = CountdownColors[3].Item1;
                BackColor = CountdownColors[3].Item2;
                LabelCountdown.Text = "欢迎使用高考倒计时, 请右键点击此处到设置里添加考试信息";
            }

            if (!IsReadyToMove)
            {
                ApplyLocation();
                KeepOnScreen();
            }
        }

        private void ApplyLocation()
        {
            if (!IsDragable)
            {
                Location = PositionIndex switch
                {
                    0 => IsPPTService ? new(SelectedScreen.Location.X + PptsvcThreshold, SelectedScreen.Location.Y) : SelectedScreen.Location,
                    1 => new(SelectedScreen.Left, SelectedScreen.Top + SelectedScreen.Height / 2 - Height / 2),
                    2 => new(SelectedScreen.Left, SelectedScreen.Bottom - Height),
                    3 => new(SelectedScreen.Left + SelectedScreen.Width / 2 - Width / 2, SelectedScreen.Top),
                    4 => new(SelectedScreen.Left + SelectedScreen.Width / 2 - Width / 2, SelectedScreen.Top + SelectedScreen.Height / 2 - Height / 2),
                    5 => new(SelectedScreen.Left + SelectedScreen.Width / 2 - Width / 2, SelectedScreen.Bottom - Height),
                    6 => new(SelectedScreen.Right - Width, SelectedScreen.Top),
                    7 => new(SelectedScreen.Right - Width, SelectedScreen.Top + SelectedScreen.Height / 2 - Height / 2),
                    8 => new(SelectedScreen.Right - Width, SelectedScreen.Bottom - Height),
                    _ => throw new Exception()
                };
            }
        }

        private void CompatibleWithPPTService()
        {
            if (IsPPTService)
            {
                var ValidArea = Screen.GetWorkingArea(this);

                if (Left == ValidArea.Left && Top == ValidArea.Top)
                {
                    Left = ValidArea.Left + PptsvcThreshold;
                }
            }
        }

        private void RefreshScreen()
        {
            SelectedScreen = Screen.AllScreens[ScreenIndex - 1 == -1 ? 0 : ScreenIndex - 1].WorkingArea;
        }

        private void KeepOnScreen()
        {
            var ValidArea = Screen.GetWorkingArea(this);

            if (Left < ValidArea.Left) Left = ValidArea.Left;
            if (Top < ValidArea.Top) Top = ValidArea.Top;
            if (Right > ValidArea.Right) Left = ValidArea.Right - Width;
            if (Bottom > ValidArea.Bottom) Top = ValidArea.Bottom - Height;
        }

        private void SaveLocation()
        {
            configManager.WriteConfig(new()
            {
                { ConfigItems.PosX, $"{Location.X}" },
                { ConfigItems.PosY, $"{Location.Y}" }
            });
        }

        private void OptimizeMemory(object state)
        {
            try
            {
                Process ProcessGetCurrentMemory = ProcessHelper.RunProcess("powershell.exe", $"-Command (Get-Counter \\\"\\Process({LaunchManager.AppNameEn})\\Working Set - Private\\\").CounterSamples.CookedValue", RedirectOutput: true);

                ProcessGetCurrentMemory.WaitForExit();
                int MemoryUsage = int.Parse(ProcessGetCurrentMemory.StandardOutput.ReadToEnd().Trim());

                if (MemoryUsage > 9437184)
                {
                    throw new Exception();
                }
            }
            catch
            {
                WindowsAPI.EmptyWorkingSet(Process.GetCurrentProcess().Handle);
            }
        }
    }
}