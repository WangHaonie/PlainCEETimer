namespace PlainCEETimer.Countdown;

public sealed class CountdownStartInfo
{
    public int Mode { get; init; }

    public int AutoSwitchInterval { get; init; }

    public int ExamIndex { get; init; }

    public bool AutoSwitch { get; init; }

    public CountdownFormat Format { get; init; }

    public required ColorPair DefaultColor { get; init; }

    public required Exam[] Exams { get; init; }

    public required CountdownRule[] CustomRules { get; init; }

    public required CountdownRule[] GlobalRules { get; init; }

    public required CountdownRule[] DefaultRules { get; init; }
}
