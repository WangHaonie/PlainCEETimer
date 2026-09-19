using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.SourceGenerators;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;
using PlainCEETimer.UI.Extensions;
using PlainCEETimer.WPF.Converters;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.WPF.ViewModels;

public sealed partial class ImmersiveViewModel : ObservableObject, ISupportInitialize, IImmersiveServiceHub, IDisposable
{
    [ObservableProperty]
    public partial string Content { get; private set; }

    [ObservableProperty]
    public partial Brush ForeBrush { get; private set; }

    [ObservableProperty]
    public partial Brush BackBrush { get; private set; }

    [ObservableProperty]
    public partial FontModel Font { get; private set; }

    [BackingField("m_countdown")]
    public required partial ICountdownService CountdownService { get; set; }

    [BackingField("m_styles")]
    public required partial IWindowStyles WindowStyles { get; set; }

    [BackingField(MemberNames.MessageX)]
    public required partial IDialogService DialogService { get; set; }

    [BackingField(MemberNames.Initializer)]
    public required partial IWindowInitializer WindowInitializer { get; set; }

    private bool isFullScreen;
    private MenuItem MenuItemFullScreen;
    private MenuItemBuilder DefaultMenuBuilder;
    private readonly CountdownManager m_helper;
    private readonly ColorToBrushConverter m_cbConverter;

    public ImmersiveViewModel()
    {
        m_cbConverter = new();
        m_helper = CountdownManager.Instance;
        m_helper.CountdownFontChanged += OnCountdownFontChanged;
        Font = m_helper.CountdownFont;
    }

    public void BeginInit()
    {
        return;
    }

    public void EndInit()
    {
        m_countdown.CountdownUpdated += OnCountdownUpdated;

        if (m_countdown.CurrentInfo != null)
        {
            UpdateCountdown(m_countdown.CurrentInfo);
        }

        DefaultMenuBuilder = b =>
        [
            MenuItemFullScreen = b.Item("全屏(&F)", (_, _) => ToggleFullScreen())
        ];

        Initializer.Initialize += (_, _) =>
        {
            MessageX.Owner.AttachContextMenuEx(DefaultMenuBuilder, out _);
        };
    }

    public void Dispose()
    {
        m_countdown.CountdownUpdated -= OnCountdownUpdated;
        m_helper.CountdownFontChanged -= OnCountdownFontChanged;
        GC.SuppressFinalize(this);
    }

    private void OnCountdownUpdated(object sender, CountdownBasicInfo e)
    {
        UpdateCountdown(e);
    }

    private void OnCountdownFontChanged(FontModel font)
    {
        Font = font;
    }

    private void UpdateCountdown(CountdownBasicInfo info)
    {
        Content = info.Content ?? string.Empty;
        ForeBrush = m_cbConverter.Convert(info.ForeColor.ToColor());
        BackBrush = m_cbConverter.Convert(info.BackColor.ToColor());
    }

    [RelayCommand]
    private void ToggleFullScreen()
    {
        MenuItemFullScreen.Checked = isFullScreen = !isFullScreen;
        m_styles.ToggleScreen();
    }

    [RelayCommand]
    private void ExitFullScreen()
    {
        if (isFullScreen)
        {
            ToggleFullScreen();
        }
    }

    ~ImmersiveViewModel()
    {
        Dispose();
    }
}
