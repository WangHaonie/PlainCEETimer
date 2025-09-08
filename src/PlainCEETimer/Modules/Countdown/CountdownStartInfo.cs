namespace PlainCEETimer.Modules.Countdown;

public sealed class CountdownStartInfo
{
    public int AutoSwitchInterval { get; set; }
    public int ExamIndex { get; set; }
    public string[] GlobalCustomText { get; set; }
    public CountdownOption Options { get; set; }
    public CountdownMode Mode { get; set; }
    public CountdownField Field { get; set; }
    public ColorPair[] GlobalColors { get; set; }
    public Exam[] Exams { get; set; }
    public CustomRule[] CustomRules { get; set; }
}
