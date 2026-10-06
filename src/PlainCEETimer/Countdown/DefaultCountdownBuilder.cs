using System;
using System.Collections.ObjectModel;
using System.Text;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI;

namespace PlainCEETimer.Countdown;

public sealed class DefaultCountdownBuilder
{
    public bool CanStart => !string.IsNullOrWhiteSpace(CurrentExam.Name) && (CurrentExam.End > CurrentExam.Start || Mode == 0);

    private bool CanUseRules;
    private bool CanUpdateRules = true;
    private string LastFormat;
    private CountdownPhase Phase = CountdownPhase.None;
    private CountdownRule[] CurrentRules;
    private CountdownRule DefaultRule;
    private ReadOnlyCollection<PhParsedToken> LastTokens;
    private readonly Exam CurrentExam;
    private readonly int Mode;
    private readonly CountdownFormat Format;
    private readonly bool CanUseCustomText;
    private readonly string DefaultText;
    private readonly CountdownRule[] CustomRules;
    private readonly CountdownRule[] GlobalRules;
    private readonly CountdownRule[] DefaultRules;
    private static readonly object SyncObject = new();
    private readonly string[] PhHints = [Ph.Start, Ph.End, Ph.Past];
    private readonly StringBuilder ContentBuilder = new(128);

    public DefaultCountdownBuilder(Exam exam, CountdownStartInfo info)
    {
        CurrentExam = exam;
        var settings = exam.Settings;

        if (settings.IsEnabled())
        {
            Mode = settings.Mode;
            Format = settings.Format;
            CustomRules = settings.Rules ?? [];
            GlobalRules = settings.DefRules ?? info.GlobalRules;
        }
        else
        {
            Mode = info.Mode;
            Format = info.Format;
            CustomRules = info.CustomRules ?? [];
            GlobalRules = info.GlobalRules ?? info.DefaultRules;
        }

        DefaultRules = info.DefaultRules;
        CanUseCustomText = Format == CountdownFormat.Custom;

        DefaultText = Format switch
        {
            CountdownFormat.DaysOnly => "距离{x}{ht}{d}天",
            CountdownFormat.DaysOnlyOneDecimal => "距离{x}{ht}{dd}天",
            CountdownFormat.DaysOnlyCeiling => "距离{x}{ht}{cd}天",
            CountdownFormat.HoursOnly => "距离{x}{ht}{th}小时",
            CountdownFormat.HoursOnlyOneDecimal => "距离{x}{ht}{dh}小时",
            CountdownFormat.MinutesOnly => "距离{x}{ht}{tm}分钟",
            CountdownFormat.SecondsOnly => "距离{x}{ht}{ts}秒",
            _ => "距离{x}{ht}{d}天{h}时{m}分{s}秒"
        };
    }

    public bool TestExam(DateTime now, out CountdownPhase phase, out TimeSpan span)
    {
        var s = CurrentExam.Start;
        var e = CurrentExam.End;

        if (Mode >= 0 && now < s)
        {
            phase = CountdownPhase.P1;
            span = s - now;
            return true;
        }

        if (Mode >= 1 && now < e)
        {
            phase = CountdownPhase.P2;
            span = e - now;
            return true;
        }

        if (Mode >= 2 && now > e)
        {
            phase = CountdownPhase.P3;
            span = now - e;
            return true;
        }

        phase = CountdownPhase.None;
        span = default;
        return false;
    }

    public bool TryBuild(DateTime now, out string content, out ColorPair colors)
    {
        lock (SyncObject)
        {
            if (CanStart && TestExam(now, out var phase, out var span))
            {
                SetPhase(phase);
                var result = ApplyCustomRule((int)phase, span);
                content = result.Content;
                colors = result.Colors;
                return true;
            }

            content = null;
            colors = default;
            return false;
        }
    }

    private void SetPhase(CountdownPhase phase)
    {
        if (CanUpdateRules || Phase != phase)
        {
            CurrentRules = CustomRules
                .ArrayWhere(r => r.Phase == phase)
                .ArrayOrderDescending();

            DefaultRule = GlobalRules[(int)phase];
            CanUseRules = CanUseCustomText && CurrentRules.Length != 0;
            Phase = phase;
            CanUpdateRules = false;
        }
    }

    private CountdownBuildResult ApplyCustomRule(int phase, TimeSpan span)
    {
        if (CanUseCustomText)
        {
            if (CanUseRules)
            {
                foreach (var rule in CurrentRules)
                {
                    if (phase == 2 ? (span >= rule.Tick) : (span <= rule.Tick))
                    {
                        return new(BuildContent(rule.Text, span, phase), rule.Colors);
                    }
                }
            }

            return new(BuildContent(DefaultRule.Text, span, phase), DefaultRule.Colors);
        }

        return new(BuildContent(DefaultText, span, phase), DefaultRules[phase].Colors);
    }

    private string BuildContent(string format, TimeSpan span, int phase)
    {
        if (format != LastFormat)
        {
            LastTokens = PhTokenParser.Parse(format);
            LastFormat = format;
        }

        var length = LastTokens.Count;
        ContentBuilder.Clear();

        for (int i = 0; i < length; i++)
        {
            ContentBuilder.Append(TranslatePh(LastTokens[i], span, phase));
        }

        return ContentBuilder.ToString();
    }

    private string TranslatePh(PhParsedToken format, TimeSpan span, int phase) => format.Token switch
    {
        PhToken.ExamName => CurrentExam.Name,
        PhToken.Days => span.Days.ToString(),
        PhToken.DecimalDays => span.TotalDays.Format(),
        PhToken.CeilingDays => Math.Ceiling(span.TotalDays).ToString(),
        PhToken.Hours => span.Hours.ToString("00"),
        PhToken.TotalHours => Math.Truncate(span.TotalHours).ToString(),
        PhToken.DecimalHours => span.TotalHours.Format(),
        PhToken.Minutes => span.Minutes.ToString("00"),
        PhToken.TotalMinutes => span.TotalMinutes.ToString("0"),
        PhToken.Seconds => span.Seconds.ToString("00"),
        PhToken.TotalSeconds => span.TotalSeconds.ToString("0"),
        PhToken.Hint => CanUseCustomText ? string.Empty : PhHints[phase],
        _ => format.Value,
    };
}