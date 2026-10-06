namespace PlainCEETimer.Countdown;

public readonly struct CountdownBuildResult(string content, ColorPair colors)
{
    public string Content => content;

    public ColorPair Colors => colors;
}
