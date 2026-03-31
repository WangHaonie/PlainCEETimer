using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI;

namespace PlainCEETimer.Countdown;

public class DefaultCountdownService(SynchronizationContext context = null) : ICountdownService
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
    private bool CanUpdateToken;
    private int Mode;
    private string DefaultText;
    private CountdownFormat Format;
    private CountdownPhase Phase = CountdownPhase.None;
    private Timer MainTimer;
    private Timer AutoSwitchTimer;
    private Exam CurrentExam;
    private ExamSettings Settings;
    private ColorPair DefaultColor;
    private Exam[] Exams;
    private CountdownStartInfo Info;
    private CountdownRule DefaultRule;
    private PhParsedTokenCollection CurrentTokens;
    private CountdownRule[] CustomRules;
    private CountdownRule[] GlobalRules;
    private CountdownRule[] CurrentRules;
    private CountdownRule[] DefaultRules;
    private volatile bool IsDisposing;
    private readonly object SyncObject = new();
    private readonly string[] PhHints = [Ph.Start, Ph.End, Ph.Past];
    private readonly StringBuilder ContentBuilder = new(512);
    private readonly SynchronizationContext CurrentContext = context ?? SynchronizationContext.Current;

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
        IsDisposing = true;
        StopAutoSwitchTimer();
        StopMainTimer();
        GC.SuppressFinalize(this);
    }

    private void SetStartInfo(CountdownStartInfo value)
    {
        AutoSwitchInterval = value.AutoSwitchInterval;
        ExamIndex = value.ExamIndex;
        EnableAutoSwitch = value.AutoSwitch;
        Exams = value.Exams;
        ExamsCount = Exams.Length;
        Info = value;
        DefaultRules = value.DefaultRules;
        DefaultColor = value.DefaultColor;
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
        Settings = CurrentExam.Settings;

        if (Settings.IsEnabled())
        {
            Mode = Settings.Mode;
            Format = Settings.Format;
            CustomRules = Settings.Rules ?? [];
            GlobalRules = Settings.DefRules ?? Info.GlobalRules;
        }
        else
        {
            Mode = Info.Mode;
            Format = Info.Format;
            CustomRules = Info.CustomRules ?? [];
            GlobalRules = Info.GlobalRules ?? DefaultRules;
        }

        CanStart = !string.IsNullOrWhiteSpace(CurrentExam.Name) && (CurrentExam.End > CurrentExam.Start || Mode == 0);
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

        CanUpdateRules = true;
        CanUpdateToken = true;
    }

    private void AutoSwitchCallback(object state)
    {
        if (!IsDisposing)
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
    }

    private void CountdownCallback(object state)
    {
        if (!IsDisposing)
        {
            if (CanStart && TestExam(CurrentExam, out var phase, out var span))
            {
                SetPhase(phase);
                ApplyCustomRule((int)phase, span);
            }
            else
            {
                StopMainTimer();
                OnCountdownUpdated("欢迎使用高考倒计时", DefaultColor);
            }
        }
    }

    private bool TestExam(Exam exam, out CountdownPhase phase, out TimeSpan span)
    {
        var t = DateTime.Now;
        var s = exam.Start;
        var e = exam.End;

        if (Mode >= 0 && t < s)
        {
            phase = CountdownPhase.P1;
            span = s - t;
            return true;
        }

        if (Mode >= 1 && t < e)
        {
            phase = CountdownPhase.P2;
            span = e - t;
            return true;
        }

        if (Mode >= 2 && t > e)
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
            CurrentRules = CustomRules
                .ArrayWhere(r => r.Phase == phase)
                .ArrayOrderDescending(true);

            DefaultRule = GlobalRules[(int)phase];
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
                        OnCountdownUpdated(BuildContent(rule.Text, span, phase), rule.Colors);
                        return;
                    }
                }
            }

            OnCountdownUpdated(BuildContent(DefaultRule.Text, span, phase), DefaultRule.Colors);
        }
        else
        {
            OnCountdownUpdated(BuildContent(DefaultText, span, phase), DefaultRules[phase].Colors);
        }
    }

    private string BuildContent(string format, TimeSpan span, int phase)
    {
        lock (SyncObject)
        {
            if (CanUpdateToken)
            {
                CurrentTokens = PhTokenParser.Parse(format);
                CanUpdateToken = false;
            }

            var length = CurrentTokens.Count;
            ContentBuilder.Clear();

            for (int i = 0; i < length; i++)
            {
                ContentBuilder.Append(TranslatePh(CurrentTokens[i], span, phase));
            }

            return ContentBuilder.ToString();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string TranslatePh(PhParsedToken format, TimeSpan span, int phase) => format.Token switch
    {
        PhToken.ExamName => CurrentExam.Name,
        PhToken.Days => span.Days.ToString(),
        PhToken.DecimalDays => Math.Ceiling(span.TotalDays).ToString(),
        PhToken.CeilingDays => span.TotalDays.ToString("0.0"),
        PhToken.Hours => span.Hours.ToString("00"),
        PhToken.TotalHours => Math.Truncate(span.TotalHours).ToString(),
        PhToken.DecimalHours => span.TotalHours.ToString("0.0"),
        PhToken.Minutes => span.Minutes.ToString("00"),
        PhToken.TotalMinutes => span.TotalMinutes.ToString("0"),
        PhToken.Seconds => span.Seconds.ToString("00"),
        PhToken.TotalSeconds => span.TotalSeconds.ToString("0"),
        PhToken.Hint => CanUseCustomText ? string.Empty : PhHints[phase],
        _ => format.Value,
    };

    private void OnExamSwitched()
    {
        if (ExamIndex != LastExamIndex)
        {
            CurrentContext.Post(_ => ExamSwitched?.Invoke(this, new(ExamIndex)), null);
            LastExamIndex = ExamIndex;
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

    private static Exam GetCurrentExam(Exam[] exams, ref int index)
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

    ~DefaultCountdownService()
    {
        Dispose();
    }
}
