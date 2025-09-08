using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Countdown;

public class DefaultCountdownService : ICountdownService
{
    public CountdownStartInfo StartInfo
    {
        get;
        set => SetStartInfo(ref field, value);
    }

    public event ExamSwitchedEventHandler ExamSwitched;
    public event CountdownUpdatedEventHandler CountdownUpdated;

    private int ExamIndex;
    private int AutoSwitchInterval;
    private bool IsRunning;
    private bool EnableAutoSwitch;
    private bool CanUseCustomText;
    private bool CanStart;
    private bool CanUseRules;
    private string ExamName;
    private string[] GlobalText;
    private DateTime ExamStart;
    private DateTime ExamEnd;
    private ColorPair[] GlobalColors;
    private CountdownMode Mode;
    private CountdownField SelectedField;
    private CountdownOption Options;
    private CountdownPhase Phase = CountdownPhase.None;
    private Timer MainTimer;
    private Timer AutoSwitchTimer;
    private Exam[] Exams;
    private CustomRule[] CustomRules;
    private CustomRule[] Rules;
    private readonly MatchEvaluator DefaultMatchEvaluator;
    private readonly Dictionary<string, string> PhCountdown = new(12);
    private readonly Regex CountdownRegEx = new(Validator.RegexPhPatterns, RegexOptions.Compiled);
    private readonly string[] DefaultTexts = [Constants.PhStart, Constants.PhEnd, Constants.PhPast];

    public DefaultCountdownService()
    {
        DefaultMatchEvaluator = m =>
        {
            var key = m.Value;
            return PhCountdown.TryGetValue(key, out string value) ? value : key;
        };
    }

    public void Start()
    {
        TryStartMainTimer();
        ResetAutoSwitch();
    }

    public void SwitchToExam(int index)
    {
        ExamIndex = index;
        UpdateExams();
        TryStartMainTimer();
        ResetAutoSwitch();
    }

    public void Dispose()
    {
        MainTimer.Destory();

        if (!CanStart)
        {
            AutoSwitchTimer.Destory();
        }

        IsRunning = false;
    }

    private void SetStartInfo(ref CountdownStartInfo field, CountdownStartInfo value)
    {
        AutoSwitchInterval = value.AutoSwitchInterval;
        ExamIndex = value.ExamIndex;
        GlobalText = value.GlobalCustomText;
        Options = value.Options;
        CanUseCustomText = CheckOptions(CountdownOption.UseCustomText);
        EnableAutoSwitch = CheckOptions(CountdownOption.EnableAutoSwitch);
        Mode = value.Mode;
        SelectedField = value.Field;
        GlobalColors = value.GlobalColors;
        Exams = value.Exams;
        CustomRules = value.CustomRules;
        UpdateExams();
        field = value;
    }

    private void TryStartMainTimer()
    {
        if (!IsRunning)
        {
            MainTimer = new(CountdownCallback, null, 0, 1000);
            IsRunning = true;
        }
    }

    private void ResetAutoSwitch()
    {
        AutoSwitchTimer.Destory();

        if (CanStart && EnableAutoSwitch && Exams.Length > 1)
        {
            AutoSwitchTimer = new(AutoSwitchCallback, null, AutoSwitchInterval, AutoSwitchInterval);
        }
    }

    private void UpdateExams()
    {
        var current = GetCurrentExam(Exams, ref ExamIndex);
        ExamName = current.Name;
        ExamStart = current.Start;
        ExamEnd = current.End;
        CanStart = !string.IsNullOrWhiteSpace(ExamName) && (ExamEnd > ExamStart || Mode == CountdownMode.Mode1);
        CountdownCallback(null);
    }

    private bool CheckOptions(CountdownOption option)
    {
        return (Options & option) == option;
    }

    private void SetPhase(CountdownPhase phase)
    {
        if (Phase != phase)
        {
            Rules =
            [..
                CustomRules
                .Where(r => r.Phase == phase)
                .OrderByDescending(x => x)
            ];

            CanUseRules = CanUseCustomText && Rules.Length != 0;
            Phase = phase;
        }
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
        OnExamSwitched(index);
        return index < 0 ? new() : exams[index];
    }

    private void AutoSwitchCallback(object state)
    {
        ExamIndex = (ExamIndex + 1) % Exams.Length;
        UpdateExams();
        TryStartMainTimer();
        OnExamSwitched(ExamIndex);
    }

    private void CountdownCallback(object state)
    {
        if (CanStart)
        {
            var now = DateTime.Now;

            if (Mode >= CountdownMode.Mode1 && now < ExamStart)
            {
                SetPhase(CountdownPhase.P1);
                ApplyCustomRule(0, ExamStart - now);
                return;
            }

            if (Mode >= CountdownMode.Mode2 && now < ExamEnd)
            {
                SetPhase(CountdownPhase.P2);
                ApplyCustomRule(1, ExamEnd - now);
                return;
            }

            if (Mode >= CountdownMode.Mode3 && now > ExamEnd)
            {
                SetPhase(CountdownPhase.P3);
                ApplyCustomRule(2, now - ExamEnd);
                return;
            }
        }

        Dispose();
        OnCountdownUpdated("欢迎使用高考倒计时", GlobalColors[3]);
    }

    private void ApplyCustomRule(int phase, TimeSpan span)
    {
        PhCountdown[Constants.PhExamName] = ExamName;
        PhCountdown[Constants.PhDays] = $"{span.Days}";
        PhCountdown[Constants.PhCeilingDays] = $"{span.Days + 1}";
        PhCountdown[Constants.PhDecimalDays] = $"{span.TotalDays:0.0}";
        PhCountdown[Constants.PhHours] = $"{span.Hours:00}";
        PhCountdown[Constants.PhDecimalHours] = $"{span.TotalHours:0.0}";
        PhCountdown[Constants.PhTotalHours] = $"{Math.Truncate(span.TotalHours)}";
        PhCountdown[Constants.PhMinutes] = $"{span.Minutes:00}";
        PhCountdown[Constants.PhTotalMinutes] = $"{span.TotalMinutes:0}";
        PhCountdown[Constants.PhSeconds] = $"{span.Seconds:00}";
        PhCountdown[Constants.PhTotalSeconds] = $"{span.TotalSeconds:0}";

        if (CanUseCustomText)
        {
            if (CanUseRules)
            {
                foreach (var rule in Rules)
                {
                    if (phase == 2 ? (span >= rule.Tick) : (span <= rule.Tick))
                    {
                        OnCountdownUpdated(CountdownRegEx.Replace(rule.Text, DefaultMatchEvaluator), rule.Colors);
                        return;
                    }
                }
            }

            OnCountdownUpdated(CountdownRegEx.Replace(GlobalText[phase], DefaultMatchEvaluator), GlobalColors[phase]);
        }
        else
        {
            PhCountdown["{ht}"] = DefaultTexts[phase];
            OnCountdownUpdated(CountdownRegEx.Replace(GetDefaultText(), DefaultMatchEvaluator), GlobalColors[phase]);
        }
    }

    private string GetDefaultText() => SelectedField switch
    {
        CountdownField.DaysOnly => "距离{x}{ht}{d}天",
        CountdownField.DaysOnlyOneDecimal => "距离{x}{ht}{dd}天",
        CountdownField.DaysOnlyCeiling => "距离{x}{ht}{cd}天",
        CountdownField.HoursOnly => "距离{x}{ht}{th}小时",
        CountdownField.HoursOnlyOneDecimal => "距离{x}{ht}{dh}小时",
        CountdownField.MinutesOnly => "距离{x}{ht}{tm}分钟",
        CountdownField.SecondsOnly => "距离{x}{ht}{ts}秒",
        _ => "距离{x}{ht}{d}天{h}时{m}分{s}秒"
    };

    private void OnExamSwitched(int index)
    {
        ExamSwitched?.Invoke(this, new(index));
    }

    private void OnCountdownUpdated(string content, ColorPair colors)
    {
        CountdownUpdated?.Invoke(this, new(content, colors.Fore, colors.Back));
    }
}
