using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Forms;

public sealed class MainForm : AppForm
{
    public static bool UniTopMost { get; private set; } = true;

    protected override AppFormParam Params => AppFormParam.Special | AppFormParam.RoundCorner;

    public static event Action UniTopMostChanged;

    private int CurrentTheme;
    private int CountdownWidth;
    private int ExamIndex;
    private int ScreenIndex;
    private int FieldValue;
    private bool IsDraggable;
    private bool IsPPTService;
    private bool IsReadyToMove;
    private bool ShowTrayIcon;
    private bool ShowTrayText;
    private bool TrayIconReopen;
    private bool BorderUseAccentColor;
    private string CountdownContent;
    private Color CountdownForeColor;
    private BorderColorObject BorderColor;
    private Point LastLocation;
    private Point LastMouseLocation;
    private Rectangle SelectedScreenRect;
    private CountdownPosition CountdownPos;
    private ICountdownService MainCountdown;
    private AboutForm FormAbout;
    private AppConfig AppConfig;
    private ContextMenu ContextMenuMain;
    private DisplayObject Display;
    private Exam[] Exams;
    private Font CountdownFont;
    private GeneralObject General;
    private MemoryCleaner MemCleaner;
    private MenuItem ExamSwitchMenu;
    private Menu.MenuItemCollection ExamSwitchMenuItems;
    private NotifyIcon TrayIcon;
    private SettingsForm FormSettings;
    private const int PptsvcThreshold = 1;
    private const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

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
            LastMouseLocation = e.Location;
            LastLocation = Location;
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (IsDraggable && IsReadyToMove)
        {
            SetLocation(MousePosition.X - LastMouseLocation.X, MousePosition.Y - LastMouseLocation.Y);
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

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_DWMCOLORIZATIONCOLORCHANGED && BorderUseAccentColor)
        {
            SetBorderColor(BOOL.TRUE, ThemeManager.GetAccentColor(m.WParam));
        }

        base.WndProc(ref m);
    }

    private void MainForm_LocationRefreshed()
    {
        AppConfig.Location = Location;
        Validator.DemandConfig();
    }

    private void ExamItems_Click(object sender, EventArgs e)
    {
        int index = ((MenuItem)sender).Index;

        if (!Win32UI.MenuGetItemCheckStateByPosition(ExamSwitchMenu.Handle, index))
        {
            MainCountdown.SwitchToExam(index);
            SwitchToExam(index);
        }
    }

    private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            App.OnTrayMenuShowAllClicked();
        }
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
        LoadConfig();
        ApplyLocation();
        CompatibleWithPPTService();
        LoadContextMenu();
        LoadTrayIcon();
        SetCountdownAutoWrap();
        ApplyStyle();
        RunCountdown();
    }

    private void LoadConfig()
    {
        AppConfig = App.AppConfig;
        General = AppConfig.General;
        Display = AppConfig.Display;

        IsDraggable = Display.Draggable;
        UniTopMost = General.UniTopMost;
        IsPPTService = Display.SeewoPptsvc;
        ScreenIndex = Display.ScreenIndex;
        CountdownPos = Display.Position;
        FieldValue = Display.X;
        ShowTrayIcon = General.TrayIcon;
        ShowTrayText = General.TrayText;

        Exams = AppConfig.Exams;
        ExamIndex = AppConfig.ExamIndex;
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

        if (General.MemClean)
        {
            MemCleaner ??= new();
            MemCleaner.Start();
        }
        else
        {
            MemCleaner.Destory();
            MemCleaner = null;
        }
    }

    private void ApplyStyle()
    {
        var topmost = General.TopMost;
        BorderColor = General.BorderColor;
        TopMost = false;
        TopMost = topmost;
        UniTopMostChanged?.Invoke();
        ShowInTaskbar = !topmost;
        Opacity = General.Opacity / 100D;

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

    private void RunCountdown()
    {
        if (MainCountdown == null)
        {
            MainCountdown = new DefaultCountdownService();

            MainCountdown.ExamSwitched += (_, e) => SwitchToExam(e.Index);

            MainCountdown.CountdownUpdated += (_, e) =>
            {
                var content = e.Content;
                var back = e.BackColor;
                CountdownContent = content;
                CountdownForeColor = e.ForeColor;
                BackColor = back;
                Size = TextRenderer.MeasureText(CountdownContent, CountdownFont, new(CountdownWidth, 0), TextFormatFlags.WordBreak);
                Invalidate();

                if (ShowTrayText)
                {
                    UpdateTrayIconText(content);
                }

                var type = BorderColor.Type;

                if (BorderColor.Enabled && type is 1 or 2)
                {
                    SetBorderColor(BOOL.TRUE, type == 1 ? CountdownForeColor : back);
                }
            };

        }

        var options = CountdownOption.None;
        var endIndex = Display.EndIndex;
        var mode = endIndex == 2 ? CountdownMode.Mode3 : (endIndex is 1 or 2 ? CountdownMode.Mode2 : CountdownMode.Mode1);

        if (Display.CustomText)
        {
            options |= CountdownOption.UseCustomText;
        }

        if (General.AutoSwitch)
        {
            options |= CountdownOption.EnableAutoSwitch;
        }

        MainCountdown.Start(new()
        {
            AutoSwitchInterval = GetAutoSwitchInterval(General.Interval),
            ExamIndex = ExamIndex,
            GlobalCustomText = AppConfig.GlobalCustomTexts,
            Options = options,
            Mode = mode,
            Field = Display.ShowXOnly ? (CountdownField)(FieldValue + 1) : CountdownField.Normal,
            GlobalColors = AppConfig.GlobalColors,
            Exams = Exams,
            CustomRules = AppConfig.CustomRules
        });
    }

    private void LoadContextMenu()
    {
        ContextMenuMain = BaseContextMenu();
        ExamSwitchMenu = ContextMenuMain.MenuItems[0];
        ExamSwitchMenuItems = ExamSwitchMenu.MenuItems;
        ContextMenu = ContextMenuMain;

        if (Exams.Length != 0)
        {
            ExamSwitchMenuItems.Clear();

            for (int i = 0; i < Exams.Length; i++)
            {
                var item = new MenuItem()
                {
                    Text = $"{i + 1}. {Exams[i]}",
                    RadioCheck = true
                };

                item.Click += ExamItems_Click;
                ExamSwitchMenuItems.Add(item);
            }
        }
    }

    private void LoadTrayIcon()
    {
        if (ShowTrayIcon)
        {
            if (TrayIcon == null)
            {
                if (TrayIconReopen)
                {
                    if (MessageX.Warn("由于系统限制，重新开关托盘图标需要重启应用程序后方可正常显示。\n\n是否立即重启？", MessageButtons.YesNo) == DialogResult.Yes)
                    {
                        App.Exit(true);
                    }

                    return;
                }

                var tmp = BaseContextMenu();

                for (int i = 0; i < 2; i++)
                {
                    tmp.MenuItems.RemoveAt(0);
                }

                TrayIcon = new()
                {
                    Visible = true,
                    Text = Text,
                    Icon = App.AppIcon,

                    ContextMenu = ContextMenuBuilder.Merge(tmp, ContextMenuBuilder.Build(b =>
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

                TrayIcon.MouseClick += TrayIcon_MouseClick;
            }

            if (!ShowTrayText)
            {
                UpdateTrayIconText(App.AppName);
            }
        }
        else
        {
            TrayIcon.Dispose();
            TrayIcon = null;
            TrayIconReopen = true;
        }
    }

    private void SwitchToExam(int index)
    {
        if (index < 0)
        {
            var item = ExamSwitchMenuItems[0];
            item.Checked = false;
            item.Enabled = false;
        }
        else
        {
            ExamSwitchMenu.DoRadioCheck(index);
        }

        ExamIndex = index;
        AppConfig.ExamIndex = index;
        Validator.DemandConfig();
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

    private ContextMenu BaseContextMenu() => ContextMenuBuilder.Build(b =>
    [
        /*

        克隆 (重用) 现有 ContextMenuStrip 实例 参考：

        .net - C# - Duplicate ContextMenuStrip Items into another - Stack Overflow
        https://stackoverflow.com/questions/37884815/c-sharp-duplicate-contextmenustrip-items-into-another

        */

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

    private void UpdateTrayIconText(string content)
    {
        if (TrayIcon != null)
        {
            var text = content.Truncate(60);
            TrayIcon.Text = text;
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
        CountdownWidth = GetCurrentScreenRect().Width - 10;
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
            Display.ScreenIndex = 0;
            Validator.DemandConfig();
        }

        SelectedScreenRect = screens[ScreenIndex].WorkingArea;
    }

    private void SetBorderColor(BOOL enabled, COLORREF color)
    {
        ThemeManager.SetBorderColor(Handle, color, enabled);
    }
}