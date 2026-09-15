using System;

namespace PlainCEETimer.Countdown;

public interface ICountdownService : IDisposable
{
    bool Enabled { get; set; }

    bool ShouldDispose { get; }

    CountdownBasicInfo CurrentInfo { get; }

    event ExamSwitchedEventHandler ExamSwitched;

    event CountdownUpdatedEventHandler CountdownUpdated;

    void Start(CountdownStartInfo startInfo);

    void SwitchTo(SwitchOption option, int index = 0);

    void ForceRefresh();
}
