using PlainCEETimer.Countdown;

namespace PlainCEETimer.UI.Core;

public interface IRequireCountdown
{
    ICountdownService CountdownService { get; set; }
}
