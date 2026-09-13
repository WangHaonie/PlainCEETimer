using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PlainCEETimer.Countdown;
using PlainCEETimer.WPF.Converters;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.WPF.ViewModels;

public sealed partial class ImmersiveWindowViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    public partial string Content { get; private set; }

    [ObservableProperty]
    public partial Brush ForeBrush { get; private set; }

    [ObservableProperty]
    public partial Brush BackBrush { get; private set; }

    [ObservableProperty]
    public partial FontModel Font { get; private set; }

    private readonly ICountdownService m_countdown;
    private readonly CountdownHelper m_helper;
    private readonly ColorToBrushConverter m_cbConverter;

    public ImmersiveWindowViewModel(ICountdownService countdown)
    {
        m_countdown = countdown;
        m_helper = CountdownHelper.Instance;
        m_cbConverter = new();

        m_countdown.CountdownUpdated += OnCountdownUpdated;
        m_helper.CountdownFontChanged += OnCountdownFontChanged;

        if (m_countdown.CurrentInfo != null)
        {
            UpdateCountdown(m_countdown.CurrentInfo);
        }

        Font = m_helper.CountdownFont;
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

    public void Dispose()
    {
        m_countdown.CountdownUpdated -= OnCountdownUpdated;
        m_helper.CountdownFontChanged -= OnCountdownFontChanged;
        GC.SuppressFinalize(this);
    }

    ~ImmersiveWindowViewModel()
    {
        Dispose();
    }
}
