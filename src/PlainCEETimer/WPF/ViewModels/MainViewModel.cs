using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.Models;
using PlainCEETimer.WPF.Views;

namespace PlainCEETimer.WPF.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public partial CornerRadius CornerRadius { get; set; } = new(8);

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

    internal MainWindow View { get; set; }

    internal ICountdownService Countdown => MainCountdown;

    private ICountdownService MainCountdown;

    internal void RunCountdown()
    {
        var a = App.AppConfig;
        var g = a.General;
        var d = a.Display;

        if (MainCountdown == null)
        {
            MainCountdown = new DefaultCountdownService();
            MainCountdown.ExamSwitched += (_, e) => View.SwitchToExam(e.Index, true);

            MainCountdown.CountdownUpdated += (_, e) =>
            {
                var content = e.Content;
                var fore = e.ForeColor.ToColor();
                var back = e.BackColor.ToColor();
                Content = content;
                Foreground = fore;
                Background = back;

                if (g.TrayText)
                {
                    View.UpdateTrayIconText(content);
                }

                var b = g.BorderColor;
                var type = b.Type;

                if (b.Enabled && type is 1 or 2)
                {
                    BorderColor = type == 1 ? fore : back;
                }
            };
        }

        MainCountdown.Start(new()
        {
            AutoSwitchInterval = GetAutoSwitchInterval(g.Interval),
            ExamIndex = a.Exam,
            GlobalRules = a.GlobalRules,
            AutoSwitch = g.AutoSwitch,
            Mode = d.Mode,
            Format = d.Format,
            Exams = a.Exams.ArrayWhere(x => !x.Excluded),
            CustomRules = a.CustomRules,
            DefaultRules = DefaultValues.GlobalDefaultRules,
            DefaultColor = DefaultValues.GlobalDefaultColor
        });
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

    public void ChangeCountdownFont(FontModel newFont)
    {
        Font = newFont;
        View.UpdateFontNameItem(newFont.ToString().Truncate(35));

        if (!ConfigValidator.ValidateNeeded)
        {
            MainCountdown.ForceRefresh();
            App.AppConfig.Display.Font = newFont;
            ConfigValidator.DemandConfig();
        }
    }
}
