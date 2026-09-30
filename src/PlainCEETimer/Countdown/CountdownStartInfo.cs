using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Linq;

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

    public static CountdownStartInfo FromConfig(AppConfig config)
    {
        var a = config;
        var g = config.General;
        var d = config.Display;

        return new()
        {
            AutoSwitchInterval = ConfigValidator.GetAutoSwitchInterval(g.Interval),
            ExamIndex = a.Exam,
            GlobalRules = a.GlobalRules,
            AutoSwitch = g.AutoSwitch,
            Mode = d.Mode,
            Format = d.Format,
            Exams = a.Exams.ArrayWhere(e => !e.Excluded).ArrayOrder(),
            CustomRules = a.CustomRules,
            DefaultRules = DefaultValues.GlobalDefaultRules,
            DefaultColor = DefaultValues.GlobalDefaultColor
        };
    }
}
