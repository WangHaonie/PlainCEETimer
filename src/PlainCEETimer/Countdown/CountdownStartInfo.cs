namespace PlainCEETimer.Countdown;

public sealed class CountdownStartInfo
{
    public int Mode { get; set; }

    public int AutoSwitchInterval { get; set; }

    public int ExamIndex { get; set; }

    public bool AutoSwitch { get; set; }

    public CountdownFormat Format { get; set; }

    public Exam[] Exams { get; set; }

    public CountdownRule[] CustomRules { get; set; }

    public CountdownRule[] GlobalRules { get; set; }
}
