namespace PlainCEETimer.Countdown;

/// <summary>
/// 存储倒计时所用到的占位符。
/// </summary>
public static class Ph
{
    public const string ExamName = "{x}";
    public const string Days = "{d}";
    public const string DecimalDays = "{dd}";
    public const string CeilingDays = "{cd}";
    public const string Hours = "{h}";
    public const string TotalHours = "{th}";
    public const string DecimalHours = "{dh}";
    public const string Minutes = "{m}";
    public const string TotalMinutes = "{tm}";
    public const string Seconds = "{s}";
    public const string TotalSeconds = "{ts}";

    public const string Start = "还有";
    public const string End = "结束还有";
    public const string Past = "已过去了";

    public const string RtP1 = $"开始{Start}";
    public const string RtP2 = End;
    public const string RtP3 = Past;

    public const string P1 = $"距离{ExamName}{Start}{Days}天{Hours}时{Minutes}分{Seconds}秒";
    public const string P2 = $"距离{ExamName}{End}{Days}天{Hours}时{Minutes}分{Seconds}秒";
    public const string P3 = $"距离{ExamName}{Past}{Days}天{Hours}时{Minutes}分{Seconds}秒";

    public static string[] RuleTypes => field ??= [RtP1, RtP2, RtP3];

    public static string[] FormatPhs => field ??=
    [
        ExamName,
        Days, DecimalDays, CeilingDays,
        Hours, TotalHours, DecimalHours,
        Minutes, TotalMinutes,
        Seconds, TotalSeconds
    ];

    public static string[] ComboBoxFormatItems => field ??=
    [
        "默认",
        "仅总天数",
        "仅总天数 (一位小数)",
        "仅总天数 (向上取整)",
        "仅总小时",
        "仅总小时 (一位小数)",
        "仅总分钟",
        "仅总秒数",
        "自定义"
    ];

    public static string[] ComboBoxEndItems => field ??=
    [
        "<程序欢迎信息>",
        "考试还有多久结束",
        "考试还有多久结束 和 已过去了多久"
    ];
}
