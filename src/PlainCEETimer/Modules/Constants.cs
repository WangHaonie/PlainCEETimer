namespace PlainCEETimer.Modules;

public static class Constants
{
    public const string PhExamName = "{x}";
    public const string PhDays = "{d}";
    public const string PhCeilingDays = "{cd}";
    public const string PhDecimalDays = "{dd}";
    public const string PhHours = "{h}";
    public const string PhTotalHours = "{th}";
    public const string PhDecimalHours = "{dh}";
    public const string PhMinutes = "{m}";
    public const string PhTotalMinutes = "{tm}";
    public const string PhSeconds = "{s}";
    public const string PhTotalSeconds = "{ts}";
    public const string PhStart = "还有";
    public const string PhEnd = "结束还有";
    public const string PhPast = "已过去了";
    public const string PhRtP1 = $"开始{PhStart}";
    public const string PhRtP2 = PhEnd;
    public const string PhRtP3 = PhPast;
    public const string PhP1 = $"距离{PhExamName}{PhStart}{PhDays}天{PhHours}时{PhMinutes}分{PhSeconds}秒";
    public const string PhP2 = $"距离{PhExamName}{PhEnd}{PhDays}天{PhHours}时{PhMinutes}分{PhSeconds}秒";
    public const string PhP3 = $"距离{PhExamName}{PhPast}{PhDays}天{PhHours}时{PhMinutes}分{PhSeconds}秒";

    public static readonly string[] PhRuleTypes = [PhRtP1, PhRtP2, PhRtP3];
    public static readonly string[] PhAllPhases = [PhP1, PhP2, PhP3];
    public static readonly string[] AllPHs =
    [
        PhExamName,
        PhDays, PhDecimalDays, PhCeilingDays,
        PhHours, PhTotalHours, PhDecimalHours,
        PhMinutes, PhTotalMinutes,
        PhSeconds, PhTotalSeconds
    ];
}
