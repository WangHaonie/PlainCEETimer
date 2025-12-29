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
using PlainCEETimer.Modules.Http;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Dialogs;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Forms;

public sealed class MainForm : AppForm
{
    public static bool UniTopMost { get; private set; } = true;

    protected override AppFormParam Params => AppFormParam.Special | AppFormParam.RoundCorner;

    public static event Action UniTopMostChanged;

    private int CurrentTheme;
    private int CountdownMaxWidth;
    private int ExamIndex;
    private int ScreenIndex;
    private bool IsDraggable;
    private bool IsPPTService;
    private bool IsReadyToMove;
    private bool ShowTrayIcon;
    private bool ShowTrayText;
    private bool TrayIconReopen;
    private bool IsHotKey1Activated;
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
    private string[] ExamItems;
    private Font CountdownFont;
    private GeneralObject General;
    private MemoryCleaner MemCleaner;
    private MenuItem FontNameMenuItem;
    private PagedContextMenu MenuSwitchExams;
    private NotifyIcon TrayIcon;
    private SettingsForm FormSettings;
    private HotKeyDialog DialogHotKey;
    private HotKeyService[] hksvc;
    private Action<HotKeyPressEventArgs>[] hkevents;
    private const int PptsvcThreshold = 1;

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
        new Action(Startup.RefreshTaskState).Start();
        new NetworkedAction(() => new Updater().CheckForUpdate(false, this)).Invoke();
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
                SaveLocation();
            }

            IsReadyToMove = false;
        }

        base.OnMouseUp(e);
    }

    private void SaveLocation()
    {
        AppConfig.Location = Location;
        Validator.DemandConfig();
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
        const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

        if (m.Msg == WM_DWMCOLORIZATIONCOLORCHANGED && BorderUseAccentColor)
        {
            SetBorderColor(true, ThemeManager.GetAccentColor(m.WParam));
        }

        base.WndProc(ref m);
    }

    private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            App.OnActivateMain();
        }
    }

    private void VerifyLocation()
    {
        if (!IsReadyToMove)
        {
            ApplyLocation();
            var p = Location;

            if (KeepOnScreen() != p)
            {
                SaveLocation();
            }
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
        RegisterHotKeys();
    }

    private void LoadConfig()
    {
        AppConfig = App.AppConfig;
        General = AppConfig.General;
        Display = AppConfig.Display;

        IsDraggable = Display.Drag;
        UniTopMost = General.UniTopMost;
        IsPPTService = Display.SeewoPptsvc;
        ScreenIndex = Display.Screen;
        CountdownPos = Display.Position;
        ShowTrayIcon = General.TrayIcon;
        ShowTrayText = General.TrayText;

        Exams = AppConfig.Exams;
        ExamIndex = AppConfig.Exam;

        if (IsDraggable)
        {
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
            SetBorderColor(false, default);
        }
        else
        {
            switch (BorderColor.Type)
            {
                case 0:
                    SetBorderColor(true, BorderColor.Color);
                    break;
                case 3:
                    BorderUseAccentColor = true;
                    SetBorderColor(true, ThemeManager.GetAccentColor());
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
            MainCountdown.ExamSwitched += (_, e) => SwitchToExam(e.Index, true);

            MainCountdown.CountdownUpdated += (_, e) =>
            {
                var content = e.Content;
                var back = e.BackColor;
                CountdownContent = content;
                CountdownForeColor = e.ForeColor;
                BackColor = back;
                Size = TextRenderer.MeasureText(CountdownContent, CountdownFont, new(CountdownMaxWidth, 0), TextFormatFlags.WordBreak);
                Invalidate();

                if (ShowTrayText)
                {
                    UpdateTrayIconText(content);
                }

                var type = BorderColor.Type;

                if (BorderColor.Enabled && type is 1 or 2)
                {
                    SetBorderColor(true, type == 1 ? CountdownForeColor : back);
                }
            };
        }

        MainCountdown.Start(new()
        {
            AutoSwitchInterval = GetAutoSwitchInterval(General.Interval),
            ExamIndex = ExamIndex,
            GlobalRules = AppConfig.GlobalRules,
            AutoSwitch = General.AutoSwitch,
            Mode = Display.Mode,
            Format = Display.Format,
            Exams = Exams,
            CustomRules = AppConfig.CustomRules,
            DefaultRules = DefaultValues.GlobalDefaultRules,
            DefaultColor = DefaultValues.GlobalDefaultColor
        });
    }

    private void LoadContextMenu()
    {
        ContextMenuMain = GetBaseContextMenu();
        FontNameMenuItem = ContextMenuMain.MenuItems[1].MenuItems[0];
        FontNameMenuItem.Enabled = false;
        ChangeCountdownFont(AppConfig.Font);
        ContextMenu = ContextMenuMain;

        if (MenuSwitchExams == null)
        {
            MenuSwitchExams = new();

            MenuSwitchExams.ItemClick += (_, _) =>
            {
                var index = MenuSwitchExams.SelectedIndex;
                MainCountdown.SwitchTo(SwitchOption.ByIndex, index);
                SwitchToExam(index, false);
            };
        }

        MenuSwitchExams.Parent = ContextMenuMain.MenuItems[0];
        MenuSwitchExams.DefaultText = "请先添加考试信息";
        MenuSwitchExams.SelectedIndex = ExamIndex;
        MenuSwitchExams.CountPerPage = General.CountPerPage;

        if (Exams.IsNullOrEmpty())
        {
            ExamItems = [];
        }
        else
        {
            ExamItems = new string[Exams.Length];

            for (int i = 0; i < Exams.Length; i++)
            {
                var e = Exams[i];
                ExamItems[i] = $"{i + 1}. {e.Name.Truncate(6)} ({e.Start.Format()})";
            }
        }

        MenuSwitchExams.Items = ExamItems;
        MenuSwitchExams.Build();
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

                var tmp = GetBaseContextMenu();
                var mi = tmp.MenuItems;

                for (int i = 0; i < 3; i++)
                {
                    mi.RemoveAt(0);
                }

                TrayIcon = new()
                {
                    Visible = true,
                    Text = Text,
                    Icon = App.AppIcon,

                    ContextMenu = ContextMenuBuilder.Merge(tmp, ContextMenuBuilder.Build(b =>
                    [
                        b.Separator(),
                        b.Item("显示界面(&X)", (_, _) => App.OnActivateMain()).Default(),

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

    private void SwitchToExam(int index, bool sw)
    {
        if (index >= 0 && index != ExamIndex)
        {
            if (sw)
            {
                MenuSwitchExams.SelectedIndex = index;
            }

            ExamIndex = index;
            AppConfig.Exam = index;
            Validator.DemandConfig();
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

    private void ChangeCountdownFont(Font newFont)
    {
        CountdownFont = newFont;
        FontNameMenuItem.Text = newFont.Format().Truncate(35);

        if (!Validator.ValidateNeeded)
        {
            MainCountdown.ForceRefresh();
            AppConfig.Font = CountdownFont;
            Validator.DemandConfig();
        }
    }

    private ContextMenu GetBaseContextMenu() => ContextMenuBuilder.Build(b =>
    [
        /*

        克隆 (重用) 现有 ContextMenuStrip 实例 参考：

        .net - C# - Duplicate ContextMenuStrip Items into another - Stack Overflow
        https://stackoverflow.com/questions/37884815/c-sharp-duplicate-contextmenustrip-items-into-another

        */

        b.Item("切换(&Q)"),

        b.Menu("字体(&F)",
        [
            b.Item(null),
            b.Separator(),

            b.Item("更改字体(&C)", (_, _) =>
            {
                var dialog = new PlainFontDialog(this, CountdownFont);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ChangeCountdownFont(dialog.Font);
                }
            }),

            b.Item("恢复默认(&R)", (_, _) => ChangeCountdownFont(DefaultValues.CountdownDefaultFont))
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

        b.Item("快捷键(&K)", (_, _) =>
        {
            if (DialogHotKey == null)
            {
                DialogHotKey = new();

                if (DialogHotKey.ShowDialog(this) == DialogResult.OK)
                {
                    RegisterHotKeys();
                }

                DialogHotKey = null;
            }
            else
            {
                DialogHotKey.ReActivate();
            }
        }),

        b.Separator(),
        b.Item("安装目录(&D)", (_, _) => Process.Start(App.ExecutableDir))
    ]);

    private void RegisterHotKeys()
    {
        var hks = AppConfig.HotKeys;

        if (!hks.IsNullOrEmpty())
        {
            if (!hksvc.IsNullOrEmpty())
            {
                foreach (var svc in hksvc)
                {
                    svc.Unregister();
                }
            }

            hksvc = new HotKeyService[Validator.HotKeyCount];

            hkevents ??=
            [
                _ =>
                {
                    IsHotKey1Activated = !IsHotKey1Activated;
                    Opacity = IsHotKey1Activated ? 0D : 1D;
                    ReActivate();
                },

                _ => MainCountdown.SwitchTo(SwitchOption.Previous),
                _ => MainCountdown.SwitchTo(SwitchOption.Next)
            ];

            for (int i = 0; i < Math.Min(Validator.HotKeyCount, hks.Length); i++)
            {
                var svc = new HotKeyService(hks[i], hkevents[i]);

                if (!svc.Register())
                {
                    MessageX.Warn($"快捷键 \"{Validator.GetHokKeyDescription(i)}\" 注册失败，可能被其他应用程序占用！");
                }

                hksvc[i] = svc;
            }
        }
    }

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
        CountdownMaxWidth = GetCurrentScreenRect().Width - 10;
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
            Display.Screen = 0;
            Validator.DemandConfig();
        }

        SelectedScreenRect = screens[ScreenIndex].WorkingArea;
    }

    private void SetBorderColor(bool enabled, COLORREF color)
    {
        Win32UI.SetBorderColor(Handle, color, enabled);
    }
}