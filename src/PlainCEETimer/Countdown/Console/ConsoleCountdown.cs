using System;
using System.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;

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
        Countdown.SetRecipient(CountdownRecipient.Console, true);

        try
        {
            Console.Title(App.AppName).Anchor();
            CountdownManager.EnsureService();
            ExitEvent.Wait();
        }
        finally
        {
            System.Console.CancelKeyPress -= Console_CancelKeyPress;
            Countdown.SetRecipient(CountdownRecipient.Console, false);
            Countdown.CountdownUpdated -= Countdown_CountdownUpdated;
            Countdown.ExamSwitched -= Countdown_ExamSwitched;
            Console.AnchorEnd().ResetColor().WriteLine();
            ExitEvent.Dispose();
        }
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
