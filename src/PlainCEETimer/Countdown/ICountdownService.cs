using System;

namespace PlainCEETimer.Countdown;

public interface ICountdownService : IDisposable
{
    event ExamSwitchedEventHandler ExamSwitched;

    event CountdownUpdatedEventHandler CountdownUpdated;

    void Start(CountdownStartInfo startInfo);

    void SwitchTo(SwitchOption option, int index = 0);

    void ForceRefresh();
}
