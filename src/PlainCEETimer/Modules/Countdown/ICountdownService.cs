using System;

namespace PlainCEETimer.Modules.Countdown;

public interface ICountdownService : IDisposable
{
    CountdownStartInfo StartInfo { get; set; }

    event ExamSwitchedEventHandler ExamSwitched;
    event CountdownUpdatedEventHandler CountdownUpdated;

    void Start();
    void SwitchToExam(int index);
}
