using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Http;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.Modules.Update;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Dialogs;
using PlainCEETimer.UI.Extensions;
using PlainCEETimer.UI.Forms;
using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.ViewModels;
using WFColor = System.Drawing.Color;
using WFContextMenu = System.Windows.Forms.ContextMenu;
using WFDialogResult = System.Windows.Forms.DialogResult;
using WFMenuItem = System.Windows.Forms.MenuItem;
using WFMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using WFRectagle = System.Drawing.Rectangle;

namespace PlainCEETimer.WPF.Views;

public partial class MainWindow : AppWindow
{
    public static bool UniTopMost { get; private set; } = true;

    protected override AppWindowStyle Params => AppWindowStyle.Special | AppWindowStyle.RoundCorner;

    private int CurrentTheme;
    private int ExamIndex;
    private bool IsDragging;
    private bool IsDraggable;
    private bool IsPPTService;
    private bool IsHotKey1Activated;
    private bool BorderUseAccentColor;
    private bool TrayIconReopen;
    private CountdownPosition CountdownPos;
    private Point LastLocation;
    private WFRectagle ScreenRect;
    private BorderColorObject BorderColor;
    private AppConfig AppConfig;
    private DisplayObject Display;
    private GeneralObject General;
    private MemoryCleaner MemCleaner;
    private AboutForm FormAbout;
    private SettingsForm FormSettings;
    private HotKeyDialog DialogHotKey;
    private NotifyIcon TrayIcon;
    private MenuItemBuilder MainContextMenuItemBuilder;
    private HotKeyService[] hksvc;
    private WFMenuItem MenuItemFontName;
    private PagedContextMenu MenuSwitchExams;
    private string[] ExamItems;
    private WFContextMenu ContextMenuMain;
    private EventHandler<HotKeyPressEventArgs>[] hkevents;
    private Exam[] Exams;
    private readonly MainViewModel vm;
    private bool ShowTrayIcon;
    private bool ShowTrayText;
    private int ScreenIndex;
    private bool IsWpf;
    private const int PptsvcThreshold = 1;

    public MainWindow()
    {
        vm = new() { View = this };
        DataContext = vm;
        InitializeComponent();

        SystemEvents.DisplaySettingsChanged += (_, _) =>
        {
            RefreshScreen();
            SetCountdownAutoWrap();
            ApplyLocation();
        };
    }

    protected override void OnInitialized(EventArgs e)
    {
        RefreshSettings();
        ConfigValidator.ValidateNeeded = false;

        new Action(() =>
        {
            Startup.Initialize();
            Startup.RefreshTaskState();
        }).Start();

        new NetworkedAction(() => Updater.Instance.CheckForUpdate(false, this)).Invoke();

        base.OnInitialized(e);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        VerifyLocation();
        base.OnRenderSizeChanged(sizeInfo);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (IsDraggable)
        {
            LastLocation = Location;
            IsDragging = true;
            DragMove();

            if (Location != LastLocation)
            {
                KeepOnScreen();
                CompatibleWithPPTService();
                SetCountdownAutoWrap();
                SaveLocation();
            }

            IsDragging = false;
        }

        base.OnMouseLeftButtonDown(e);
    }

    protected override bool OnClosing()
    {
        return true;
    }

    protected override void OnClosed(EventArgs e)
    {
        vm.Countdown.Destory();
        base.OnClosed(e);
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

        if (m.Msg == WM_DWMCOLORIZATIONCOLORCHANGED && BorderUseAccentColor)
        {
            vm.BorderColor = ThemeManager.GetAccentColor(m.WParam).ToColor();
        }

        base.WndProc(ref m);
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
        vm.RunCountdown();
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

        Exams = AppConfig.Exams.ArrayWhere(e => !e.Excluded);
        ExamIndex = AppConfig.Exam;

        if (IsDraggable)
        {
            Location = AppConfig.DipLocation;
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

    private void CompatibleWithPPTService()
    {
        if (IsPPTService)
        {
            var screenRect = ScreenRect;
            var screenRectX = screenRect.X;

            if (Top == Px2DipY(screenRect.Y) && Left == Px2DipX(screenRectX))
            {
                Left = Px2DipX(screenRectX + PptsvcThreshold);
            }
        }
    }

    private void LoadContextMenu()
    {
        MainContextMenuItemBuilder ??= b =>
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

                b.Item("更改字体(&C)", (_, _) => {}),

                b.Item("恢复默认(&R)", (_, _) => {})
            ]),

            b.Separator(),

            b.Item("设置(&S)", (_, _) =>
            {
                if (FormSettings == null || FormSettings.IsDisposed)
                {
                    FormSettings = new();

                    FormSettings.DialogEnd += dr =>
                    {
                        if (dr == WFDialogResult.OK)
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

                    if (DialogHotKey.ShowDialog(this) == WFDialogResult.OK)
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
        ];

        ContextMenuMain = new(MainContextMenuItemBuilder(new()));
        MenuItemFontName = ContextMenuMain.MenuItems[1].MenuItems[0];
        MenuItemFontName.Enabled = false;
        //ChangeCountdownFont(AppConfig.Font);
        NativeContextMenu = ContextMenuMain;

        if (MenuSwitchExams == null)
        {
            MenuSwitchExams = new();

            MenuSwitchExams.ItemClick += (_, _) =>
            {
                var index = MenuSwitchExams.SelectedIndex;
                vm.Countdown.SwitchTo(SwitchOption.ByIndex, index);
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
            var length = Exams.Length;
            ExamItems = new string[length];
            var max = General.Truncate;
            var no = General.No;

            for (int i = 0; i < length; i++)
            {
                var sb = new StringBuilder(32);
                var e = Exams[i];
                sb.Clear();

                if (no)
                {
                    sb.Append(i + 1).Append(". ");
                }

                ExamItems[i] = sb.Append(e.Name.Truncate(max))
                .Append('\t')
                .Append(e.Start.Format())
                .ToString();
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
                    if (MessageX.Warn("由于系统限制，重新开关托盘图标需要重启应用程序后方可正常显示。\n\n是否立即重启？", MessageButtons.YesNo) == WFDialogResult.Yes)
                    {
                        App.Exit(true);
                    }

                    return;
                }

                var tmp = new WFContextMenu(MainContextMenuItemBuilder(new()));
                var mi = tmp.MenuItems;

                for (int i = 0; i < 3; i++)
                {
                    mi.RemoveAt(0);
                }

                TrayIcon = new()
                {
                    Visible = true,
                    Text = Title,
                    Icon = App.AppIcon,

                    ContextMenu = tmp.AddItems(b =>
                    [
                        b.Separator(),
                        b.Item("显示界面(&X)", (_, _) => WindowManager.Current.OnActivateRequested()).Default(),

                        b.Menu("关闭(&C)",
                        [
                            b.Item("重启(&R)", (_, _) => App.Exit(true)),
                            b.Item("退出(&Q)", (_, _) => App.Exit())
                        ])
                    ])
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

    private void TrayIcon_MouseClick(object sender, WFMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            WindowManager.Current.OnActivateRequested();
        }
    }

    internal void UpdateTrayIconText(string content)
    {
        try
        {
            if (TrayIcon != null)
            {
                var text = content.Truncate(60);
                TrayIcon.Text = text;
            }
        }
        catch { }
    }

    internal void SwitchToExam(int index, bool sw)
    {
        if (index >= 0 && index != ExamIndex)
        {
            if (sw)
            {
                MenuSwitchExams.SelectedIndex = index;
            }

            ExamIndex = index;
            AppConfig.Exam = index;
            ConfigValidator.DemandConfig();
        }
    }

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

            hksvc = new HotKeyService[ConfigValidator.HotKeyCount];

            hkevents ??=
            [
                (_, _) =>
                {
                    IsHotKey1Activated = !IsHotKey1Activated;
                    Hide(IsHotKey1Activated);

                    if (IsHotKey1Activated)
                    {
                        ReActivate();
                    }
                },

                (_, _) => vm.Countdown.SwitchTo(SwitchOption.Previous),
                (_, _) => vm.Countdown.SwitchTo(SwitchOption.Next)
            ];

            for (int i = 0; i < Math.Min(ConfigValidator.HotKeyCount, hks.Length); i++)
            {
                var svc = new HotKeyService(hks[i], hkevents[i]);

                if (!svc.Register())
                {
                    MessageX.Warn($"快捷键 \"{ConfigValidator.GetHokKeyDescription(i)}\" 注册失败，可能被其他应用程序占用！");
                }

                hksvc[i] = svc;
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
            ConfigValidator.DemandConfig();
        }

        ScreenRect = screens[ScreenIndex].WorkingArea;
    }

    private void SetCountdownAutoWrap()
    {
        TextBlockCountdown.MaxWidth = Px2DipX(GetCurrentScreenRect().Width - 10);
    }

    private void ApplyStyle()
    {
        var topmost = General.TopMost;
        BorderColor = General.BorderColor;
        Topmost = false;
        Topmost = topmost;
        WindowManager.Current.OnTopMostChanged(General.UniTopMost);
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

        var newIsWpf = Display.UseWPF;

        if (!ConfigValidator.ValidateNeeded && IsWpf != newIsWpf)
        {
            MessageX.Warn("需要重启程序以更换主窗口渲染方式！");
            App.Exit(true);
            return;
        }

        IsWpf = newIsWpf;
        var newTheme = AppConfig.Dark;

        if (!ConfigValidator.ValidateNeeded && ThemeManager.IsThemeChanged(CurrentTheme, newTheme))
        {
            MessageX.Warn("由于更改了应用主题设置，需要立即重启倒计时！");
            App.Exit(true);
            return;
        }

        CurrentTheme = newTheme;
    }

    private void SetBorderColor(bool enabled, WFColor color)
    {
        Win32UI.SetBorderColor(Handle, color, enabled);
    }

    private void ApplyLocation()
    {
        if (!IsDraggable)
        {
            var w = Dip2PxX(Width);
            var h = Dip2PxY(Height);

            switch (CountdownPos)
            {
                case CountdownPosition.LeftCenter:
                    SetLocation(ScreenRect.X, ScreenRect.Y + (ScreenRect.Height - h) / 2);
                    break;
                case CountdownPosition.BottomLeft:
                    SetLocation(ScreenRect.X, ScreenRect.Bottom - h);
                    break;
                case CountdownPosition.TopCenter:
                    SetLocation(ScreenRect.X + (ScreenRect.Width - w) / 2, ScreenRect.Y);
                    break;
                case CountdownPosition.Center:
                    SetLocation(ScreenRect.X + (ScreenRect.Width - w) / 2, ScreenRect.Y + (ScreenRect.Height - h) / 2);
                    break;
                case CountdownPosition.BottomCenter:
                    SetLocation(ScreenRect.X + (ScreenRect.Width - w) / 2, ScreenRect.Bottom - h);
                    break;
                case CountdownPosition.TopRight:
                    SetLocation(ScreenRect.Right - w, ScreenRect.Y);
                    break;
                case CountdownPosition.RightCenter:
                    SetLocation(ScreenRect.Right - w, ScreenRect.Y + (ScreenRect.Height - h) / 2);
                    break;
                case CountdownPosition.BottomRight:
                    SetLocation(ScreenRect.Right - w, ScreenRect.Bottom - h);
                    break;
                default:
                    SetLocation(IsPPTService ? ScreenRect.X + PptsvcThreshold : ScreenRect.X, ScreenRect.Y);
                    break;
            }
        }
    }

    private void VerifyLocation()
    {
        if (!IsDragging)
        {
            ApplyLocation();
            var p = Location;

            if (KeepOnScreen() != p)
            {
                SaveLocation();
            }
        }
    }

    private void SaveLocation()
    {
        AppConfig.DipLocation = Location;
        ConfigValidator.DemandConfig();
    }
}
