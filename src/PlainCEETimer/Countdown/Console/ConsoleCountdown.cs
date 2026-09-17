using System;
using System.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Countdown.Console;

public sealed class ConsoleCountdown
{
    private readonly ICountdownService Countdown = CountdownManager.Instance.CountdownService;
    private readonly ConsoleHelper Console = ConsoleHelper.Instance;
    private readonly ManualResetEventSlim ExitEvent = new(false);
    private readonly object SyncObject = new();

    private bool IsRendering;

    public void Launch()
    {
        Countdown.CountdownUpdated += CountdownUpdated;
        System.Console.CancelKeyPress += OnCancelKeyPress;

        try
        {
            Console.Anchor();
            StartCountdown();
            ExitEvent.Wait();
        }
        finally
        {
            System.Console.CancelKeyPress -= OnCancelKeyPress;
            Countdown.CountdownUpdated -= CountdownUpdated;
            Console.AnchorEnd().ResetColor().WriteLine();
            ExitEvent.Dispose();
        }
    }

    private void StartCountdown()
    {
        var config = App.Current.AppConfig;
        var general = config.General;
        var display = config.Display;
        var exams = config.Exams.ArrayWhere(e => !e.Excluded).ArrayOrder();

        Countdown.Start(new()
        {
            AutoSwitchInterval = ConfigValidator.GetAutoSwitchInterval(general.Interval),
            ExamIndex = config.Exam,
            GlobalRules = config.GlobalRules,
            AutoSwitch = general.AutoSwitch,
            Mode = display.Mode,
            Format = display.Format,
            Exams = exams,
            CustomRules = config.CustomRules,
            DefaultRules = DefaultValues.GlobalDefaultRules,
            DefaultColor = DefaultValues.GlobalDefaultColor
        });
    }

    private void CountdownUpdated(object sender, CountdownBasicInfo info)
    {
        lock (SyncObject)
        {
            if (!IsRendering)
            {
                IsRendering = true;
                Console.Clear()
                    .Color(info.ForeColor, info.BackColor)
                    .Write(info.Content);
                IsRendering = false;
            }
        }
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        ExitEvent.Set();
    }
}
