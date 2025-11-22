namespace PlainCEETimer.Countdown;

public sealed class CountdownStartInfo
{
    public int AutoSwitchInterval { get; set; }

    public int ExamIndex { get; set; }

    public CountdownOption Options { get; set; }

    public CountdownMode Mode { get; set; }

    public CountdownField Field { get; set; }

    public Exam[] Exams { get; set; }

    public CustomRule[] CustomRules { get; set; }

    public CustomRule[] GlobalRules { get; set; }
}
