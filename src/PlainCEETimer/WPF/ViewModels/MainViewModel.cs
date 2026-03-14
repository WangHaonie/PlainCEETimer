using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
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
using PlainCEETimer.UI.Core;
using PlainCEETimer.UI.Dialogs;
using PlainCEETimer.UI.Extensions;
using PlainCEETimer.UI.Forms;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.Models;
using PlainCEETimer.WPF.Modules;
using Colors = System.Windows.Media.Colors;
using Rect = System.Drawing.Rectangle;
using WFColor = System.Drawing.Color;

namespace PlainCEETimer.WPF.ViewModels;

public sealed partial class MainViewModel : ObservableObject, IConfirmClose
{
    [ObservableProperty]
    public partial Color Background { get; set; }

    [ObservableProperty]
    public partial Color Foreground { get; set; }

    [ObservableProperty]
    public partial Color BorderColor { get; set; }

    [ObservableProperty]
    public partial string Content { get; set; }

    [ObservableProperty]
    public partial FontModel Font { get; set; }

    [ObservableProperty]
    public partial double MaximumWidth { get; set; }

    public CountdownModel CountdownModel { get; private set; } = new();

    private int CurrentTheme;
    private int ScreenIndex;
    private int ExamIndex;
    private bool IsDraggable;
    private bool IsPPTService;
    private bool ShowTrayIcon;
    private bool ShowTrayText;
    private bool IsHotKey1Activated;
    private bool BorderUseAccentColor;
    private bool IsWpf;
    private string[] ExamItems;
    private AppConfig AppConfig;
    private DisplayObject Display;
    private GeneralObject General;
    private MenuItem MenuItemFontName;
    private PagedContextMenu MenuSwitchExams;
    private MemoryCleaner MemCleaner;
    private SettingsForm FormSettings;
    private AboutForm FormAbout;
    private HotKeyDialog DialogHotKey;
    private MenuItemBuilder ItemBuilder;
    private ContextMenu ContextMenuMain;
    private Exam[] Exams;
    private HotKeyService[] hksvc;
    private EventHandler<HotKeyPressEventArgs>[] hkevents;
    private BorderColorObject BorderColorObj;
    private Rect ScreenRect;
    private CountdownPosition CountdownPos;
    private readonly ICountdownService Countdown;
    private readonly IDialogService MessageX;
    private readonly IAppWindow Owner;
    private readonly IWindowInitializer Initializer;
    private readonly IWindowDragService DragService;
    private readonly IWindowBounds Bounds;
    private readonly ITrayIconLoader TrayIcon;
    private readonly IWindowStyles Styles;
    private readonly IScreenService Screen;
    private readonly IUnifiedFontService FontService;
    private readonly IBorderColorService BorderColorService;
    private readonly WndProcCallback DefWndProc;

    private const int PptsvcThreshold = 1;
    internal const string ModelPropName = "__<>";

    public MainViewModel(MainServiceHub services)
    {
        Countdown = services.CountdownService;
        MessageX = services.DialogService;
        Owner = MessageX.Owner;
        Initializer = services.WindowInitializer;
        DragService = services.WindowDragService;
        Bounds = services.WindowBounds;
        TrayIcon = services.TrayIconLoader;
        Styles = services.WindowStyles;
        Screen = services.ScreenService;
        DefWndProc = services.WindowMessageService.DefWndProc;
        FontService = services.UnifiedFontService;
        BorderColorService = services.BorderColorService;
        Initialize();
    }

    private void Initialize()
    {
        Initializer.Initialize += (_, _) =>
        {
            RefreshSettings();
            ConfigValidator.ValidateNeeded = false;

            new Action(() =>
            {
                Startup.Initialize();
                Startup.RefreshTaskState();
            }).Start();

            new NetworkedAction(() => Updater.Instance.CheckForUpdate(false, Owner)).Invoke();

            SystemEvents.DisplaySettingsChanged += (_, _) =>
            {
                RefreshScreen();
                SetCountdownAutoWrap();
                ApplyLocation();
            };
        };

        Bounds.SizeChanged += (_, _) =>
        {
            VerifyLocation();
        };

        DragService.DragRequest += (_, e) =>
        {
            e.Cancel = !IsDraggable;
        };

        DragService.DragEnd += (_, e) =>
        {
            if (e.LocationChanged)
            {
                Bounds.KeepOnScreen();
                CompatibleWithPPTService();
                SetCountdownAutoWrap();
                SaveLocation();
            }
        };

        ItemBuilder ??= b =>
        [
            b.Item("切换(&Q)"),

            b.Menu("字体(&F)",
            [
                b.Item(null).Disable(),
                b.Separator(),

                b.Item("更改字体(&C)", (_, _) =>
                {
                    var font = new UnifiedFont()
                    {
                        Font1 = Font,
                        Font2 = CountdownModel.Font
                    };

                    if (FontService.ShowFontDialog(font) == true)
                    {
                        ChangeCountdownFont(font);
                    }
                }),

                b.Item("恢复默认(&R)", (_, _) => ChangeCountdownFont(AppConfig.GetDefaultFont()))
            ]),

            b.Separator(),

            b.Item("设置(&S)", (_, _) =>
            {
                if (FormSettings == null || FormSettings.IsDisposed)
                {
                    FormSettings = new();

                    FormSettings.DialogEnd += result =>
                    {
                        if (result.AsBoolean() == true)
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

                    if (DialogHotKey.ShowDialog(Owner).AsBoolean() == true)
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

        Countdown.ExamSwitched += (_, e) =>
        {
            SwitchToExam(e.Index, true);
        };

        Countdown.CountdownUpdated += (_, e) =>
        {
            var fore = e.ForeColor;
            var back = e.BackColor;
            var content = e.Content;
            CountdownModel.BasicInfo = e;
            Content = content;
            Foreground = fore.ToColor();
            Background = back.ToColor();
            NotifyModelChanged();
            UpdateTrayText(content);
            UpdateBorderColor(fore, back);
        };
    }

    private void UpdateTrayText(string content)
    {
        if (ShowTrayText)
        {
            try
            {
                TrayIcon.Text = content.Truncate(60);
            }
            catch { }
        }
    }

    private void UpdateBorderColor(WFColor fore, WFColor back)
    {
        var type = BorderColorObj.Type;

        if (BorderColorObj.Enabled && type is 1 or 2)
        {
            SetBorderColor(true, type == 1 ? fore : back);
        }
    }

    private void SaveLocation()
    {
        AppConfig.Location = Bounds.Location;
        ConfigValidator.DemandConfig();
    }

    public void Cleanup()
    {
        Countdown.Destory();
    }

    public void VerifyLocation()
    {
        if (!DragService.IsDragging)
        {
            ApplyLocation();
            var p = Bounds.Location;

            if (Bounds.KeepOnScreen() != p)
            {
                SaveLocation();
            }
        }
    }

    internal void WndProc(ref Message m)
    {
        const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

        if (m.Msg == WM_DWMCOLORIZATIONCOLORCHANGED && BorderUseAccentColor)
        {
            SetBorderColor(true, ThemeManager.GetAccentColor(m.WParam));
        }

        DefWndProc(ref m);
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
        IsPPTService = Display.SeewoPptsvc;
        ScreenIndex = Display.Screen;
        CountdownPos = Display.Position;
        ShowTrayIcon = General.TrayIcon;
        ShowTrayText = General.TrayText;

        Exams = AppConfig.Exams.ArrayWhere(e => !e.Excluded);
        ExamIndex = AppConfig.Exam;

        if (IsDraggable)
        {
            Bounds.Location = AppConfig.Location;
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

    private void LoadContextMenu()
    {
        Owner.AttachContextMenuEx(ItemBuilder, out ContextMenuMain);
        MenuItemFontName = ContextMenuMain.MenuItems[1].MenuItems[0];
        ChangeCountdownFont(AppConfig.GetFont());

        if (MenuSwitchExams == null)
        {
            MenuSwitchExams = new();

            MenuSwitchExams.ItemClick += (_, _) =>
            {
                var index = MenuSwitchExams.SelectedIndex;
                Countdown.SwitchTo(SwitchOption.ByIndex, index);
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
        TrayIcon.Visible = ShowTrayIcon;

        TrayIcon.ContextMenu ??= ItemBuilder.Build()
            .RemoveRange(0, 3)
            .AddItems(b =>
            [
                b.Separator(),
                b.Item("显示界面(&X)", (_, _) => WindowManager.Current.OnActivateRequested()).AsDefault(),

                b.Menu("关闭(&C)",
                [
                    b.Item("重启(&R)", (_, _) => App.Exit(true)),
                    b.Item("退出(&Q)", (_, _) => App.Exit())
                ])
            ]);

        TrayIcon.Text = App.AppName;
        TrayIcon.Icon = App.AppIcon;
    }

    private void ApplyStyle()
    {
        var topmost = General.TopMost;
        BorderColorObj = General.BorderColor;
        Styles.TopMost = false;
        Styles.TopMost = topmost;
        WindowManager.Current.OnTopMostChanged(General.UniTopMost);
        Styles.ShowInTaskbar = !topmost;
        Styles.Opacity = General.Opacity / 100D;

        if (!BorderColorObj.Enabled)
        {
            SetBorderColor(false, default);
        }
        else
        {
            switch (BorderColorObj.Type)
            {
                case 0:
                    SetBorderColor(true, BorderColorObj.Color);
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

    private void RunCountdown()
    {
        Countdown.Start(new()
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
                    Styles.Opacity = IsHotKey1Activated ? 0D : 1D;

                    if (IsHotKey1Activated)
                    {
                        Owner.ReActivate();
                    }
                },

                (_, _) => Countdown.SwitchTo(SwitchOption.Previous),
                (_, _) => Countdown.SwitchTo(SwitchOption.Next)
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
            ConfigValidator.DemandConfig();
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

    private void ApplyLocation()
    {
        if (!IsDraggable)
        {
            Bounds.Location = CountdownPos switch
            {
                CountdownPosition.LeftCenter
                    => new(ScreenRect.X, ScreenRect.Y + (ScreenRect.Height - Bounds.Height) / 2),
                CountdownPosition.BottomLeft
                    => new(ScreenRect.X, ScreenRect.Bottom - Bounds.Height),
                CountdownPosition.TopCenter
                    => new(ScreenRect.X + (ScreenRect.Width - Bounds.Width) / 2, ScreenRect.Y),
                CountdownPosition.Center
                    => new(ScreenRect.X + (ScreenRect.Width - Bounds.Width) / 2, ScreenRect.Y + (ScreenRect.Height - Bounds.Height) / 2),
                CountdownPosition.BottomCenter
                    => new(ScreenRect.X + (ScreenRect.Width - Bounds.Width) / 2, ScreenRect.Bottom - Bounds.Height),
                CountdownPosition.TopRight
                    => new(ScreenRect.Right - Bounds.Width, ScreenRect.Y),
                CountdownPosition.RightCenter
                    => new(ScreenRect.Right - Bounds.Width, ScreenRect.Y + (ScreenRect.Height - Bounds.Height) / 2),
                CountdownPosition.BottomRight
                    => new(ScreenRect.Right - Bounds.Width, ScreenRect.Bottom - Bounds.Height),
                _
                    => new(IsPPTService ? ScreenRect.X + PptsvcThreshold : ScreenRect.X, ScreenRect.Y)
            };
        }
    }

    private void CompatibleWithPPTService()
    {
        if (IsPPTService)
        {
            var screenRect = ScreenRect;
            var screenRectX = screenRect.X;

            if (Bounds.Y == screenRect.Y && Bounds.X == screenRectX)
            {
                Bounds.X = screenRectX + PptsvcThreshold;
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

    public void ChangeCountdownFont(UnifiedFont font)
    {
        Font = font.Font1;
        CountdownModel.Font = font.Font2;
        NotifyModelChanged();
        UpdateFontNameItem(font);

        if (!ConfigValidator.ValidateNeeded)
        {
            Countdown.ForceRefresh();
            Display.Font = font.Font1;
            AppConfig.Font = font.Font2;
            ConfigValidator.DemandConfig();
        }
    }

    private void UpdateFontNameItem(UnifiedFont font)
    {
        MenuItemFontName.Text = FontService.GetFontDesc(font);
    }

    private void SetCountdownAutoWrap()
    {
        MaximumWidth = Screen.GetWorkingArea().Width - 10;
    }

    private void SetBorderColor(bool enabled, WFColor color)
    {
        BorderColor = enabled ? color.ToColor() : Colors.Gray;
        BorderColorService?.SetBorderColor(enabled, color);
    }

    private void NotifyModelChanged()
    {
        OnPropertyChanged(ModelPropName);
    }

    public bool CanClose()
    {
        return false;
    }
}
