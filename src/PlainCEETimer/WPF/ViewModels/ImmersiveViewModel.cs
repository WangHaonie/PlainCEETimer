using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlainCEETimer.Countdown;
using PlainCEETimer.Countdown.Immersive;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Annotations.SourceGenerators;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;
using PlainCEETimer.UI.Dialogs;
using PlainCEETimer.UI.Extensions;
using PlainCEETimer.WPF.Converters;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.Models;
using WFColor = System.Drawing.Color;

namespace PlainCEETimer.WPF.ViewModels;

[NoConstants]
public sealed partial class ImmersiveViewModel : ObservableObject, ISupportInitialize, IImmersiveServiceHub, IDisposable
{
    [ObservableProperty]
    public partial string Content { get; private set; }

    [ObservableProperty]
    public partial string PrevContent { get; private set; }

    [ObservableProperty]
    public partial string NextContent { get; private set; }

    [ObservableProperty]
    public partial Brush ForeBrush { get; private set; }

    [ObservableProperty]
    public partial Brush PrevForeBrush { get; private set; }

    [ObservableProperty]
    public partial Brush NextForeBrush { get; private set; }

    [ObservableProperty]
    public partial Brush BackBrush { get; private set; }

    [ObservableProperty]
    public partial FontModel Font { get; private set; }

    [ObservableProperty]
    public partial bool IsSidebarOpen { get; private set; }

    [ObservableProperty]
    public partial int SelectedIndex { get; set; }

    [ObservableProperty]
    public partial bool ShowPrevNext { get; private set; }

    [ObservableProperty]
    public partial double SidebarWidth { get; private set; }

    [ObservableProperty]
    public partial bool IsSidebarExpanded { get; private set; }

    [BackingField("m_countdown")]
    public required partial ICountdownService CountdownService { get; set; }

    [BackingField("m_styles")]
    public required partial IWindowStyles WindowStyles { get; set; }

    [BackingField(MemberNames.MessageX)]
    public required partial IDialogService DialogService { get; set; }

    [BackingField(MemberNames.Initializer)]
    public required partial IWindowInitializer WindowInitializer { get; set; }

    [BackingField("isFullScreen")]
    public partial bool IsFullScreen { get; }

    [BackingField("_examItems")]
    public partial ImmersiveExamItem[] ExamItems { get; }

    public event EventHandler ExamSwitched;

    private bool _SuppressSelect;
    private int examIndex;
    private MenuItem MenuItemFullScreen;
    private MenuItemBuilder DefaultMenuBuilder;
    private System.Threading.Timer MainTimer;
    private ImmersiveObject config;
    private volatile bool m_bDisposed;
    private readonly ActionInvoker RefreshAction;
    private readonly ImmersiveCountdownController m_controller;
    private readonly CountdownManager m_helper;
    private readonly ColorToBrushConverter m_cbConverter;

    public ImmersiveViewModel()
    {
        m_cbConverter = new();
        m_helper = CountdownManager.Instance;
        m_helper.CountdownFontChanged += OnCountdownFontChanged;
        Font = m_helper.CountdownFont;

        m_controller = new();
        var count = m_controller.Count;
        _examItems = new ImmersiveExamItem[count];

        for (int i = 0; i < count; i++)
        {
            _examItems[i] = new(i, m_controller.GetExamName(i));
        }

        RefreshAction = new(Refresh);
    }

    public void BeginInit()
    {
        return;
    }

    public void EndInit()
    {
        m_countdown.SetRecipient(CountdownRecipient.Immersive, true);
        m_countdown.ExamSwitched += OnExamSwitched;
        examIndex = m_countdown.CurrentIndex;
        LoadConfig();
        MainTimer = new(OnTimerCallback, null, 0, 1000);

        DefaultMenuBuilder = b =>
        [
            MenuItemFullScreen = b.Item("全屏(&F)", (_, _) => ToggleFullScreen()).With(x => x.Checked = isFullScreen),
            b.Separator(),

            b.Item("选项(&O)", (_, _) =>
            {
                var dialog = new ImmersiveOptionsDialog();

                if (dialog.ShowDialog(MessageX.Owner) == true)
                {

                }
            })
        ];

        Initializer.Initialize += (_, _) =>
        {
            MessageX.Owner.AttachContextMenuEx(DefaultMenuBuilder, out _);
        };
    }

    public void Dispose()
    {
        m_bDisposed = true;
        MainTimer?.Destroy();

        if (m_countdown != null)
        {
            m_countdown.ExamSwitched -= OnExamSwitched;
            m_countdown.SetRecipient(CountdownRecipient.Immersive, false);
        }

        m_helper.CountdownFontChanged -= OnCountdownFontChanged;
        GC.SuppressFinalize(this);
    }

    public void LoadConfig()
    {
        config = App.Current.AppConfig.Immersive;
        ShowPrevNext = config.PrevNext;
        SidebarWidth = config.SidebarWidth;
        Refresh();
    }

    public void SetFullScreen(bool enabled)
    {
        if (isFullScreen != enabled)
        {
            ToggleFullScreen();
        }
    }

    public void SaveSidebarWidth(double width)
    {
        if (Math.Abs(config.SidebarWidth - width) > 0.5D)
        {
            config.SidebarWidth = width;
            ConfigValidator.DemandConfig();
        }
    }

    private void OnTimerCallback(object state)
    {
        if (!m_bDisposed)
        {
            SafeExecutionContext.Post(RefreshAction);
        }
    }

    private void OnExamSwitched(object sender, ExamSwitchedEventArgs e)
    {
        examIndex = e.Index;
        Refresh();
        ExamSwitched?.Invoke(this, EventArgs.Empty);
    }

    private void OnCountdownFontChanged(FontModel font)
    {
        Font = font;
    }

    partial void OnSelectedIndexChanged(int value)
    {
        if (_SuppressSelect || m_countdown == null)
        {
            return;
        }

        if (value >= 0 && value < _examItems.Length && value != examIndex)
        {
            m_countdown.SwitchTo(SwitchOption.ByIndex, value);
        }
    }

    partial void OnIsSidebarOpenChanged(bool value)
    {
        if (value)
        {
            RefreshItems();
        }
    }

    private void Refresh()
    {
        var index = examIndex;
        WFColor back;

        if (m_controller.TryBuild(index, out var content, out var colors))
        {
            back = colors.Back;
            ForeBrush = m_cbConverter.Convert(colors.Fore.ToColor());
            Content = content;
        }
        else
        {
            var def = m_controller.DefaultColor;
            back = def.Back;
            ForeBrush = m_cbConverter.Convert(def.Fore.ToColor());
            Content = DefaultCountdownService.WelcomeText;
        }

        BackBrush = m_cbConverter.Convert(back.ToColor());

        var pn = m_cbConverter.Convert(ContrastTint(back));
        PrevForeBrush = pn;
        NextForeBrush = pn;

        PrevContent = ShowPrevNext ? GetPNContent(index, -1) : string.Empty;
        NextContent = ShowPrevNext ? GetPNContent(index, 1) : string.Empty;

        RefreshItemsIfVisible();

        _SuppressSelect = true;
        SelectedIndex = index;
        _SuppressSelect = false;
    }

    private void RefreshItemsIfVisible()
    {
        if (IsSidebarOpen)
        {
            RefreshItems();
        }
    }

    private void RefreshItems()
    {
        for (int i = 0; i < _examItems.Length; i++)
        {
            var item = _examItems[i];
            var colors = m_controller.DefaultColor;

            if (m_controller.TryBuild(i, out var content, out var evaluated))
            {
                item.Content = content;
                colors = evaluated;
            }
            else
            {
                item.Content = DefaultCountdownService.WelcomeText;
            }

            item.ForeBrush = m_cbConverter.Convert(colors.Fore.ToColor());
            item.BackBrush = m_cbConverter.Convert(colors.Back.ToColor());
        }
    }

    private string GetPNContent(int index, int offset)
    {
        return m_controller.TryGetPN(index, offset, out var i) ? GetExamContent(i) : string.Empty;
    }

    private string GetExamContent(int index)
    {
        return m_controller.TryBuild(index, out var content, out _) ? content : DefaultCountdownService.WelcomeText;
    }

    private static Color ContrastTint(WFColor back)
    {
        var lum = (0.2126 * back.R + 0.7152 * back.G + 0.0722 * back.B) / 255D;
        var t = (byte)(lum > 0.5D ? 0 : 255);
        return Color.FromArgb(255, t, t, t);
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        if (IsSidebarExpanded)
        {
            IsSidebarExpanded = false;
            return;
        }

        IsSidebarOpen = !IsSidebarOpen;
    }

    [RelayCommand]
    private void ToggleSidebarExpand()
    {
        IsSidebarExpanded = !IsSidebarExpanded;

        if (IsSidebarExpanded)
        {
            IsSidebarOpen = true;
        }
    }

    [RelayCommand]
    private void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        MenuItemFullScreen?.Checked = isFullScreen;
        m_styles.ToggleScreen();
    }

    [RelayCommand]
    private void HandleEscape()
    {
        if (IsSidebarExpanded)
        {
            IsSidebarExpanded = false;
        }
        else if (isFullScreen)
        {
            ToggleFullScreen();
        }
        else if (IsSidebarOpen)
        {
            IsSidebarOpen = false;
        }
    }

    ~ImmersiveViewModel()
    {
        Dispose();
    }
}
