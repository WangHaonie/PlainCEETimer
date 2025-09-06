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

namespace PlainCEETimer.UI.Forms;

public sealed class MainForm : AppForm
{
    public static bool UniTopMost { get; private set; } = true;

    protected override AppFormParam Params => AppFormParam.Special | AppFormParam.RoundCorner;

    public static event Action UniTopMostChanged;

    private int AutoSwitchInterval;
    private int CurrentTheme;
    private int CountdownMaxW;
    private int ScreenIndex;
    private int LastMouseX;
    private int LastMouseY;
    private bool AutoSwitch;
    private bool IsDraggable;
    private bool IsPPTService;
    private bool IsReadyToMove;
    private bool LoadedMemCleaner;
    private bool MemClean;
    private bool ShowTrayIcon;
    private bool ShowTrayText;
    private bool TrayIconReopen;
    private bool BorderUseAccentColor;
    private string CountdownContent;
    private CountdownPosition CountdownPos;
    private Color CountdownForeColor;
    private BorderColorObject BorderColor;
    private Point LastLocation;
    private Rectangle SelectedScreenRect;
    private AboutForm FormAbout;
    private AppConfig AppConfig;
    private ContextMenu ContextMenuMain;
    private ContextMenu ContextMenuTray;
    private Font CountdownFont;
    private Menu.MenuItemCollection ExamSwitchMain;
    private NotifyIcon TrayIcon;
    private SettingsForm FormSettings;
    private System.Threading.Timer MemCleaner;
    private System.Windows.Forms.Timer AutoSwitchHandler;
    private const int PptsvcThreshold = 1;
    private const int MemCleanerInterval = 300_000; // 5 min
    private const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

    private DefaultCountdownService Countdown;
    private int ExamIndex;
    private ExamInfoObject[] Exams;

    protected override void OnInitializing()
    {
        Text = "高考倒计时";

        SystemEvents.DisplaySettingsChanged += (_, _) =>
        {
            RefreshScreen();
            SetCountdownAutoWrap();
            ApplyLocation();
        };

        SystemEvents.SessionEnding += (_, _) => Validator.SaveConfig();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_DWMCOLORIZATIONCOLORCHANGED && BorderUseAccentColor)
        {
            SetBorderColor(BOOL.TRUE, ThemeManager.GetAccentColor(m.WParam));
        }

        base.WndProc(ref m);
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
        Validator.ValidateNeeded = false;
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
                MainForm_LocationRefreshed();
            }

            IsReadyToMove = false;
        }

        base.OnMouseUp(e);
    }

    protected override bool OnClosing(CloseReason closeReason)
    {
        return closeReason != CloseReason.WindowsShutDown;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        VerifyLocation();
    }

    private void MainForm_LocationRefreshed()
    {
        AppConfig.Location = Location;
        Validator.DemandConfig();
    }

    private void ExamItems_Click(object sender, EventArgs e)
    {
        int index;
        var item = (MenuItem)sender;
        index = item.Index;

        if (item != null && !item.Checked)
        {
            UnselectAllExamItems();
            AppConfig.ExamIndex = index;
            Validator.DemandConfig();
            LoadExams();
            TryRunCountdown();
            UpdateExamSelection();
            Countdown.Refresh();
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
        Validator.DemandConfig();
        LoadExams();
        TryRunCountdown();
        UnselectAllExamItems();
        UpdateExamSelection(true);
    }

    private void VerifyLocation()
    {
        if (!IsReadyToMove)
        {
            ApplyLocation();
            KeepOnScreen();
        }
    }

    private void RefreshSettings()
    {
        SetCountdownAutoWrap();
        LoadConfig();
        LoadExams();
        PrepareCountdown();
        TryRunCountdown();
        ApplyLocation();
        CompatibleWithPPTService();
        LoadContextMenu();
        LoadTrayIcon();
        ApplyStyle();
    }

    private void LoadConfig()
    {
        AppConfig = App.AppConfig;
        MemClean = AppConfig.General.MemClean;
        IsDraggable = AppConfig.Display.Draggable;
        UniTopMost = AppConfig.General.UniTopMost;
        IsPPTService = AppConfig.Display.SeewoPptsvc;
        ScreenIndex = AppConfig.Display.ScreenIndex;
        CountdownPos = AppConfig.Display.Position;
        ShowTrayIcon = AppConfig.General.TrayIcon;
        ShowTrayText = AppConfig.General.TrayText;
    }

    private void Countdown_CountdownChanged(object sender, CountdownChangedEventArgs e)
    {
        var content = e.Content;
        CountdownContent = content;
        CountdownForeColor = e.ForeColor;
        BackColor = e.BackColor;
        Size = TextRenderer.MeasureText(CountdownContent, CountdownFont, new(CountdownMaxW, 0), TextFormatFlags.WordBreak);
        Invalidate();

        if (ShowTrayText)
        {
            UpdateTrayIconText(content);
        }

        var type = BorderColor.Type;

        if (BorderColor.Enabled && type is 1 or 2)
        {
            SetBorderColor(BOOL.TRUE, type == 1 ? CountdownForeColor : BackColor);
        }
    }

    private void LoadExams()
    {
        AutoSwitch = AppConfig.General.AutoSwitch;
        AutoSwitchInterval = GetAutoSwitchInterval(AppConfig.General.Interval);

        if (Countdown == null)
        {
            Countdown = new();
            Countdown.CountdownChanged += Countdown_CountdownChanged;
        }

        Exams = AppConfig.Exams;
        ExamIndex = AppConfig.ExamIndex;

        Countdown.StartInfo = new()
        {
            CountdownText = AppConfig.GlobalCustomTexts,
            UseCustomText = AppConfig.Display.CustomText,
            CustomRules = AppConfig.CustomRules,
            ShowFieldOnly = AppConfig.Display.ShowXOnly,
            CountdownField = AppConfig.Display.X,
            Exams = Exams,
            CountdownMode = AppConfig.Display.EndIndex,
            CountdownColors = AppConfig.GlobalColors,
            ExamIndex = ExamIndex
        };

    }

    private void PrepareCountdown()
    {
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

        if (MemClean ^ LoadedMemCleaner)
        {
            if (MemClean)
            {
                MemCleaner = new(_ => MemoryCleaner.Clean(), null, 3000, MemCleanerInterval);
            }
            else
            {
                MemCleaner.Dispose();
            }
        }

        LoadedMemCleaner = MemClean;
    }

    private void ApplyStyle()
    {
        var topmost = AppConfig.General.TopMost;
        BorderColor = AppConfig.General.BorderColor;
        TopMost = false;
        TopMost = topmost;
        UniTopMostChanged?.Invoke();
        ShowInTaskbar = !topmost;
        Opacity = AppConfig.General.Opacity / 100D;

        if (!BorderColor.Enabled)
        {
            SetBorderColor(BOOL.FALSE, default);
        }
        else
        {
            switch (BorderColor.Type)
            {
                case 0:
                    SetBorderColor(BOOL.TRUE, BorderColor.Color);
                    break;
                case 3:
                    BorderUseAccentColor = true;
                    SetBorderColor(BOOL.TRUE, ThemeManager.GetAccentColor());
                    break;
            }
        }

        var newTheme = AppConfig.Dark;

        if (!Validator.ValidateNeeded && ThemeManager.IsThemeChanged(CurrentTheme, newTheme))
        {
            MessageX.Warn("由于更改了应用主题设置，需要立即重启倒计时！");
            App.Exit(true);
        }

        CurrentTheme = newTheme;
    }

    private void TryRunCountdown()
    {
        Countdown.Start();
    }

    private void LoadContextMenu()
    {
        ContextMenuMain = BaseContextMenu();
        ExamSwitchMain = ContextMenuMain.MenuItems[0].MenuItems;

        if (ShowTrayIcon)
        {
            var tmp = BaseContextMenu();

            for (int i = 0; i < 2; i++)
            {
                tmp.MenuItems.RemoveAt(0);
            }

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

                    FormSettings.DialogEnd += dr =>
                    {
                        if (dr == DialogResult.OK)
                        {
                            RefreshSettings();
                            Countdown.Refresh();
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
            b.Item("安装目录(&D)", (_, _) => Process.Start(App.ExecutableDir))
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

                    return;
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

        if (!AutoSwitch)
        {
            AutoSwitchHandler.Destory();
        }
        else if (!canUpdate)
        {
            AutoSwitchHandler.Destory();

            if (Countdown.IsCountdownReady && Exams.Length > 1)
            {
                AutoSwitchHandler = new() { Interval = AutoSwitchInterval };
                AutoSwitchHandler.Tick += ExamAutoSwitch;
                AutoSwitchHandler.Start();
            }
        }
    }

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
                    SetLocation(SelectedScreenRect.X + (SelectedScreenRect.Width - Width) / 2, SelectedScreenRect.Y + (SelectedScreenRect.Height - Height) / 2);
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
            Validator.DemandConfig();
        }

        SelectedScreenRect = screens[ScreenIndex].WorkingArea;
    }

    private void SetBorderColor(BOOL enabled, COLORREF color)
    {
        ThemeManager.SetBorderColor(Handle, color, enabled);
    }
}