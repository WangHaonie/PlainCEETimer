namespace PlainCEETimer.UI;

public class TopMostStateChangedEventArgs(bool topmost)
{
    public bool IsTopMost { get; } = topmost;
}