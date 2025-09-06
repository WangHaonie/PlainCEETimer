using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules;

public delegate void CountdownChangedEventHandler(object sender, CountdownChangedEventArgs e);

public class ExamChangedEventArgs(ExamInfoObject exam, int index) : EventArgs
{
    public ExamInfoObject Exam => exam;
    public int ExamIndex => index;
}

public class CountdownChangedEventArgs(string content, Color fore, Color back) : EventArgs
{
    public string Content => content;
    public Color ForeColor => fore;
    public Color BackColor => back;
}

public class CountdownStartInfo
{
    public string[] CountdownText { get; set; }
    public bool UseCustomText { get; set; }
    public CustomRuleObject[] CustomRules { get; set; }
    public bool ShowFieldOnly { get; set; }
    public int CountdownField { get; set; }
    public ExamInfoObject[] Exams { get; set; }
    public int CountdownMode { get; set; }
    public ColorSetObject[] CountdownColors { get; set; }
    public int ExamIndex { get; set; }
}

public class DefaultCountdownService
{
    public CountdownStartInfo StartInfo
    {
        get;
        set
        {
            field = value;
            UpdateStartInfo(value);
        }
    }

    public bool IsCountdownReady { get; private set; }

    private void UpdateStartInfo(CountdownStartInfo info)
    {
        GlobalTexts = info.CountdownText;
        UseCustomText = info.UseCustomText;
        CustomRules = info.CustomRules;
        IsShowXOnly = info.ShowFieldOnly;
        ShowXOnlyIndex = info.CountdownField;
        Exams = info.Exams;
        CountdownColors = info.CountdownColors;
        ExamIndex = info.ExamIndex;

        var endIndex = info.CountdownMode;
        Mode = endIndex == 2 ? CountdownMode.Mode3 : (endIndex is 1 or 2 ? CountdownMode.Mode2 : CountdownMode.Mode1);

        if (ExamIndex >= Exams.Length)
        {
            ExamIndex = 0;
        }

        try
        {
            CurrentExam = Exams[ExamIndex];
        }
        catch
        {
            ExamIndex = -1;
            CurrentExam = new();
        }

        ExamName = CurrentExam.Name;
        ExamStart = CurrentExam.Start;
        ExamEnd = CurrentExam.End;
        IsCountdownReady = !string.IsNullOrWhiteSpace(ExamName) && (ExamEnd > ExamStart || Mode == CountdownMode.Mode1);
        SelectedState = IsShowXOnly ? (CountdownState)(ShowXOnlyIndex + 1) : CountdownState.Normal;

        if (IsCountdownReady && CurrentPhase != CountdownPhase.None)
        {
            RefreshCustomRules();
        }
    }

    public event CountdownChangedEventHandler CountdownChanged;

    private string ExamName;
    private bool IsShowXOnly;
    private bool UseCustomText;
    private bool IsCountdownRunning;
    private ExamInfoObject CurrentExam;
    private ExamInfoObject[] Exams;
    private int ExamIndex;
    private DateTime Now;
    private int ShowXOnlyIndex;
    private bool CanUseRules;
    private CustomRuleObject[] CurrentRules;
    private CountdownState SelectedState;
    private DateTime ExamEnd;
    private string[] GlobalTexts;
    private DateTime ExamStart;
    private ColorSetObject[] CountdownColors;
    private CountdownMode Mode;
    private CustomRuleObject[] CustomRules;
    private CountdownPhase CurrentPhase = CountdownPhase.None;

    private System.Threading.Timer CountdownTimer;
    private readonly string[] DefaultText = [Constants.PhStart, Constants.PhEnd, Constants.PhPast];
    private readonly MatchEvaluator DefaultMatchEvaluator;
    private readonly Dictionary<string, string> PhCountdown = new(12);
    private readonly Regex CountdownRegEx = new(Validator.RegexPhPatterns, RegexOptions.Compiled);

    public DefaultCountdownService()
    {
        DefaultMatchEvaluator = m =>
        {
            var key = m.Value;
            return PhCountdown.TryGetValue(key, out string value) ? value : key;
        };
    }

    private string GetDefaultText() => SelectedState switch
    {
        CountdownState.DaysOnly => "距离{x}{ht}{d}天",
        CountdownState.DaysOnlyOneDecimal => "距离{x}{ht}{dd}天",
        CountdownState.DaysOnlyCeiling => "距离{x}{ht}{cd}天",
        CountdownState.HoursOnly => "距离{x}{ht}{th}小时",
        CountdownState.HoursOnlyOneDecimal => "距离{x}{ht}{dh}小时",
        CountdownState.MinutesOnly => "距离{x}{ht}{tm}分钟",
        CountdownState.SecondsOnly => "距离{x}{ht}{ts}秒",
        _ => "距离{x}{ht}{d}天{h}时{m}分{s}秒"
    };

    public void Start()
    {
        if (!IsCountdownRunning)
        {
            IsCountdownRunning = true;
            CountdownTimer = new(CountdownCallback, null, 0, 1000);
        }
    }

    public void Refresh()
    {
        CountdownCallback(null);
    }

    private void CountdownCallback(object state)
    {
        if (IsCountdownReady)
        {
            Now = DateTime.Now;

            if (Mode >= CountdownMode.Mode1 && Now < ExamStart)
            {
                SetPhase(CountdownPhase.P1);
                ApplyCustomRule(0, ExamStart - Now);
                return;
            }

            if (Mode >= CountdownMode.Mode2 && Now < ExamEnd)
            {
                SetPhase(CountdownPhase.P2);
                ApplyCustomRule(1, ExamEnd - Now);
                return;
            }

            if (Mode >= CountdownMode.Mode3 && Now > ExamEnd)
            {
                SetPhase(CountdownPhase.P3);
                ApplyCustomRule(2, Now - ExamEnd);
                return;
            }
        }

        CountdownTimer.Dispose();
        OnCountdownChanged("欢迎使用高考倒计时", CountdownColors[3]);
        IsCountdownRunning = false;
    }

    private void RefreshCustomRules()
    {
        CurrentRules =
        [..
            CustomRules
            .Where(r => r.Phase == CurrentPhase)
            .OrderByDescending(x => x)
        ];

        CanUseRules = UseCustomText && CurrentRules.Length != 0;
    }

    private void SetPhase(CountdownPhase phase)
    {
        if (CurrentPhase != phase)
        {
            CurrentPhase = phase;
            RefreshCustomRules();
        }
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

        if (UseCustomText)
        {
            if (CanUseRules)
            {
                foreach (var rule in CurrentRules)
                {
                    if (phase == 2 ? (span >= rule.Tick) : (span <= rule.Tick))
                    {
                        OnCountdownChanged(CountdownRegEx.Replace(rule.Text, DefaultMatchEvaluator), rule.Colors);
                        return;
                    }
                }
            }

            OnCountdownChanged(CountdownRegEx.Replace(GlobalTexts[phase], DefaultMatchEvaluator), CountdownColors[phase]);
        }
        else
        {
            PhCountdown["{ht}"] = DefaultText[phase];
            OnCountdownChanged(CountdownRegEx.Replace(GetDefaultText(), DefaultMatchEvaluator), CountdownColors[phase]);
        }
    }

    private void OnCountdownChanged(string content, ColorSetObject colors)
    {
        CountdownChanged?.Invoke(this, new(content, colors.Fore, colors.Back));
    }
}