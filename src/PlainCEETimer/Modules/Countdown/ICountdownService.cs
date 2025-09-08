using System;

namespace PlainCEETimer.Modules.Countdown;

public interface ICountdownService : IDisposable
{
    CountdownStartInfo StartInfo { get; set; }

    event EventHandler<ExamSwitchedEventArgs> ExamSwitched;
    event EventHandler<CountdownUpdatedEventArgs> CountdownUpdated;

    void Start();
    void SwitchToExam(int index);
    void Refresh();
}