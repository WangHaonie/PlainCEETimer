using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Countdown;

public class DefaultCountdownService : ICountdownService
{
    public event ExamSwitchedEventHandler ExamSwitched;
    public event CountdownUpdatedEventHandler CountdownUpdated;

    private int ExamIndex;
    private int LastExamIndex = -2;
    private int ExamsCount;
    private int AutoSwitchInterval;
    private bool IsRunning;
    private bool EnableAutoSwitch;
    private bool CanUseCustomText;
    private bool CanStart;
    private bool CanUseRules;
    private bool CanUpdateRules;
    private bool sp_Flag;
    private int Mode;
    private int sp_Mode;
    private CountdownFormat Format;
    private CountdownFormat sp_Format;
    private CountdownPhase Phase = CountdownPhase.None;
    private Timer MainTimer;
    private Timer AutoSwitchTimer;
    private Exam CurrentExam;
    private ExamSettings CurrentSettings;
    private Exam[] Exams;
    private CountdownRule DefaultRule;
    private CountdownRule[] CustomRules;
    private CountdownRule[] sp_Rules;
    private CountdownRule[] GlobalRules;
    private CountdownRule[] sp_DefaultRules;
    private CountdownRule[] CurrentRules;
    private readonly SynchronizationContext CurrentContext;
    private readonly MatchEvaluator DefaultMatchEvaluator;
    private readonly Regex CountdownRegEx = new(Validator.RegexPhPatterns, RegexOptions.Compiled);
    private readonly string[] PhCountdown = new string[12];
    private readonly string[] DefaultTexts = [Ph.Start, Ph.End, Ph.Past];
    private static readonly ColorPair DefaultColor = ThemeManager.ShouldUseDarkMode ? new(Color.White, Color.Black) : new(Color.Black, Color.White);

    public DefaultCountdownService()
    {
        CurrentContext = SynchronizationContext.Current;

        DefaultMatchEvaluator = m =>
        {
            var key = m.Value;
            var i = Validator.GetPhIndex(key);

            if (i < 0)
            {
                return key;
            }

            return PhCountdown[i];
        };
    }

    public void Start(CountdownStartInfo startInfo)
    {
        SetStartInfo(startInfo);
        InternalStart();
    }

    public void SwitchTo(SwitchOption option, int index = 0)
    {
        ExamIndex = option switch
        {
            SwitchOption.Next => (ExamIndex + 1) % ExamsCount,
            SwitchOption.Previous => (ExamIndex - 1 + ExamsCount) % ExamsCount,
            _ => index
        };

        InternalStart();
    }

    public void ForceRefresh()
    {
        CountdownCallback(null);
    }

    public void Dispose()
    {
        StopAutoSwitchTimer();
        StopMainTimer();
        GC.SuppressFinalize(this);
    }

    private void SetStartInfo(CountdownStartInfo value)
    {
        AutoSwitchInterval = value.AutoSwitchInterval;
        ExamIndex = value.ExamIndex;
        EnableAutoSwitch = value.AutoSwitch;
        Mode = value.Mode;
        Format = value.Format;
        GlobalRules = value.GlobalRules;
        Exams = value.Exams;
        ExamsCount = Exams.Length;
        CustomRules = value.CustomRules;
        CanUpdateRules = true;
    }

    private void InternalStart()
    {
        UpdateExams();
        OnExamSwitched();
        TryStartMainTimer();
        ResetAutoSwitchTimer();
    }

    private void TryStartMainTimer()
    {
        if (!IsRunning)
        {
            MainTimer = new(CountdownCallback, null, 0, 1000);
            IsRunning = true;
        }

        CountdownCallback(null);
    }

    private void ResetAutoSwitchTimer()
    {
        StopAutoSwitchTimer();

        if (CanStart && EnableAutoSwitch && ExamsCount > 1)
        {
            AutoSwitchTimer = new(AutoSwitchCallback, null, IsRunning ? AutoSwitchInterval : 5000, AutoSwitchInterval);
        }
    }

    private void UpdateExams()
    {
        CurrentExam = GetCurrentExam(Exams, ref ExamIndex);
        CurrentSettings = CurrentExam.Settings;

        if (sp_Flag = CurrentSettings.IsEnabled())
        {
            sp_Mode = CurrentSettings.Mode;
            sp_Format = CurrentSettings.Format;
        }

        CanStart = !string.IsNullOrWhiteSpace(CurrentExam.Name) && (CurrentExam.End > CurrentExam.Start || (sp_Flag ? sp_Mode : Mode) == 0);
        CanUseCustomText = sp_Flag ? sp_Format == CountdownFormat.Custom : Format == CountdownFormat.Custom;
    }

    private Exam GetCurrentExam(Exam[] exams, ref int index)
    {
        var length = exams.Length;
        var newIndex = index;

        if (length == 0)
        {
            newIndex = -1;
        }
        else if (newIndex == -1 || length <= index)
        {
            newIndex = 0;
        }

        index = newIndex;
        return index < 0 ? new() : exams[index];
    }

    private void AutoSwitchCallback(object state)
    {
        do
        {
            ExamIndex = (ExamIndex + 1) % ExamsCount;
            UpdateExams();
        }
        while (!TestExam(CurrentExam, out _, out _) && ExamIndex != ExamsCount - 1);

        TryStartMainTimer();
        OnExamSwitched();
    }

    private void CountdownCallback(object state)
    {
        if (CanStart && TestExam(CurrentExam, out var phase, out var span))
        {
            SetPhase(phase);

            for (int i = 0; i < 12; i++)
            {
                PhCountdown[i] = TranslatePh(i, span, phase);
            }

            ApplyCustomRule((int)phase, span);
        }
        else
        {
            StopMainTimer();
            OnCountdownUpdated("欢迎使用高考倒计时", DefaultColor);
        }
    }

    private bool TestExam(Exam exam, out CountdownPhase phase, out TimeSpan span)
    {
        var t = DateTime.Now;
        var s = exam.Start;
        var e = exam.End;
        var m = sp_Flag ? sp_Mode : Mode;

        if (m >= 0 && t < s)
        {
            phase = CountdownPhase.P1;
            span = s - t;
            return true;
        }

        if (m >= 1 && t < e)
        {
            phase = CountdownPhase.P2;
            span = e - t;
            return true;
        }

        if (m >= 2 && t > e)
        {
            phase = CountdownPhase.P3;
            span = t - e;
            return true;
        }

        phase = CountdownPhase.None;
        span = default;
        return false;
    }

    private void SetPhase(CountdownPhase phase)
    {
        if (CanUpdateRules || Phase != phase)
        {
            CurrentRules =
            [..
                (sp_Flag ? sp_Rules : CustomRules)
                .Where(r => r.Phase == phase)
                .OrderByDescending(x => x)
            ];

            DefaultRule = (sp_Flag ? sp_DefaultRules : GlobalRules)[(int)phase];
            CanUseRules = CanUseCustomText && CurrentRules.Length != 0;
            Phase = phase;
            CanUpdateRules = false;
        }
    }

    private void ApplyCustomRule(int phase, TimeSpan span)
    {
        if (CanUseCustomText)
        {
            if (CanUseRules)
            {
                foreach (var rule in CurrentRules)
                {
                    if (phase == 2 ? (span >= rule.Tick) : (span <= rule.Tick))
                    {
                        OnCountdownUpdated(CountdownRegEx.Replace(rule.Text, DefaultMatchEvaluator), rule.Colors);
                        return;
                    }
                }
            }

            OnCountdownUpdated(CountdownRegEx.Replace(DefaultRule.Text, DefaultMatchEvaluator), DefaultRule.Colors);
        }
        else
        {
            OnCountdownUpdated(CountdownRegEx.Replace(GetDefaultText(), DefaultMatchEvaluator), DefaultRule.Colors);
        }
    }

    private string GetDefaultText() => (sp_Flag ? sp_Format : Format) switch
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

    private string TranslatePh(int i, TimeSpan span, CountdownPhase phase) => i switch
    {
        0 => CurrentExam.Name,
        1 => span.Days.ToString(),
        2 => Math.Ceiling(span.TotalDays).ToString(),
        3 => span.TotalDays.ToString("0.0"),
        4 => span.Hours.ToString("00"),
        5 => Math.Truncate(span.TotalHours).ToString(),
        6 => span.TotalHours.ToString("0.0"),
        7 => span.Minutes.ToString("00"),
        8 => span.TotalMinutes.ToString("0"),
        9 => span.Seconds.ToString("00"),
        10 => span.TotalSeconds.ToString("0"),
        11 => CanUseCustomText ? string.Empty : DefaultTexts[(int)phase],
        _ => string.Empty
    };

    private void OnExamSwitched()
    {
        if (ExamIndex != LastExamIndex)
        {
            CurrentContext.Post(_ => ExamSwitched?.Invoke(this, new(ExamIndex)), null);
            LastExamIndex = ExamIndex;
        }

        if (CanUpdateRules = sp_Flag)
        {
            sp_Rules = CurrentSettings.Rules ?? [];
            sp_DefaultRules = CurrentSettings.GlobalRules ?? GlobalRules;
        }
    }

    private void OnCountdownUpdated(string content, ColorPair colors)
    {
        CurrentContext.Post(_ => CountdownUpdated?.Invoke(this, new(content, colors.Fore, colors.Back)), null);
    }

    private void StopAutoSwitchTimer()
    {
        AutoSwitchTimer.Destory();
    }

    private void StopMainTimer()
    {
        MainTimer.Destory();
        IsRunning = false;
    }

    ~DefaultCountdownService()
    {
        Dispose();
    }
}
