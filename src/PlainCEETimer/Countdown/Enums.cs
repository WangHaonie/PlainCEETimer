namespace PlainCEETimer.Countdown;

public enum CountdownFormat
{
    Normal,
    DaysOnly,
    DaysOnlyOneDecimal,
    DaysOnlyCeiling,
    HoursOnly,
    HoursOnlyOneDecimal,
    MinutesOnly,
    SecondsOnly,
    Custom
}

public enum PhToken
{
    None,
    ExamName,
    Days,
    DecimalDays,
    CeilingDays,
    Hours,
    TotalHours,
    DecimalHours,
    Minutes,
    TotalMinutes,
    Seconds,
    TotalSeconds,
    Hint
}

public enum SwitchOption
{
    ByIndex,
    Next,
    Previous
}