using System;
using System.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Countdown.Console;

public class ConsoleCountdown
{
    private readonly object SyncObject = new();
    private readonly ICountdownService Countdown = CountdownManager.Instance.CountdownService;
    private readonly ManualResetEventSlim ExitEvent = new(false);
    private readonly PlainConsole Console = PlainConsole.Instance;

    public void Launch()
    {
        System.Console.CancelKeyPress += Console_CancelKeyPress;
        Countdown.CountdownUpdated += Countdown_CountdownUpdated;
        Countdown.ExamSwitched += Countdown_ExamSwitched;

        try
        {
            Console.Title(App.AppName).Anchor();
            StartCountdown();
            ExitEvent.Wait();
        }
        finally
        {
            System.Console.CancelKeyPress -= Console_CancelKeyPress;
            Countdown.CountdownUpdated -= Countdown_CountdownUpdated;
            Countdown.ExamSwitched -= Countdown_ExamSwitched;
            Console.AnchorEnd().ResetColor().WriteLine();
            ExitEvent.Dispose();
        }
    }

    private void StartCountdown()
    {
        var a = App.Current.AppConfig;
        var g = a.General;
        var d = a.Display;
        var e = a.Exams.ArrayWhere(e => !e.Excluded).ArrayOrder();

        Countdown.Start(new()
        {
            AutoSwitchInterval = ConfigValidator.GetAutoSwitchInterval(g.Interval),
            ExamIndex = a.Exam,
            GlobalRules = a.GlobalRules,
            AutoSwitch = g.AutoSwitch,
            Mode = d.Mode,
            Format = d.Format,
            Exams = e,
            CustomRules = a.CustomRules,
            DefaultRules = DefaultValues.GlobalDefaultRules,
            DefaultColor = DefaultValues.GlobalDefaultColor
        });
    }

    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        ExitEvent.Set();
    }

    private void Countdown_CountdownUpdated(object sender, CountdownBasicInfo e)
    {
        lock (SyncObject)
        {
            Console.ResetColor().Clear()
                .Color(e.ForeColor, e.BackColor).Write(e.Content);
        }
    }

    private void Countdown_ExamSwitched(object sender, ExamSwitchedEventArgs e)
    {
        ConfigValidator.DemandConfig();
    }
}
