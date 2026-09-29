using System;

namespace PlainCEETimer.Countdown;

public interface ICountdownService : IDisposable
{
    bool ShouldDispose { get; }

    CountdownBasicInfo CurrentInfo { get; }

    event ExamSwitchedEventHandler ExamSwitched;

    event CountdownUpdatedEventHandler CountdownUpdated;

    void SetRecipient(CountdownRecipient recipient, bool alive);

    void Start(CountdownStartInfo startInfo);

    void SwitchTo(SwitchOption option, int index = 0);

    void ForceRefresh();
}
