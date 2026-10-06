using System;
using System.Collections.Generic;
using System.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Countdown;

[NoConstants]
public class DefaultCountdownService : ICountdownService
{
    public CountdownBasicInfo CurrentInfo { get; private set; }

    public int CurrentIndex => ExamIndex;

    public bool ShouldDispose { get; internal set; } = true;

    public event ExamSwitchedEventHandler ExamSwitched;
    public event CountdownUpdatedEventHandler CountdownUpdated;

    public const string WelcomeText = "欢迎使用高考倒计时";

    private int ExamIndex;
    private int LastExamIndex = -2;
    private int ExamsCount;
    private int AutoSwitchInterval;
    private bool IsRunning;
    private bool EnableAutoSwitch;
    private bool CanStart;
    private Timer MainTimer;
    private Timer AutoSwitchTimer;
    private Exam CurrentExam;
    private ColorPair DefaultColor;
    private Exam[] Exams;
    private CountdownStartInfo Info;
    private DefaultCountdownBuilder CurrentEvaluator;
    private volatile bool IsDisposing;
    private volatile bool IsAlive;
    private readonly object SyncObject = new();
    private readonly ActionInvoker<int> OnExamSwitchedInvoker;
    private readonly ActionInvoker<string, ColorPair> OnCountdownUpdatedInvoker;
    private readonly HashSet<CountdownRecipient> Recipients = [];

    public DefaultCountdownService()
    {
        OnExamSwitchedInvoker = new(i => ExamSwitched?.Invoke(this, new(i)));

        OnCountdownUpdatedInvoker = new((s, cp) =>
        {
            var e = new CountdownBasicInfo(s, cp.Fore, cp.Back);
            CurrentInfo = e;
            CountdownUpdated?.Invoke(this, e);
        });
    }

    public void SetRecipient(CountdownRecipient recipient, bool alive)
    {
        lock (SyncObject)
        {
            if (alive ? Recipients.Add(recipient) : Recipients.Remove(recipient))
            {
                IsAlive = Recipients.Count > 0;
            }
            else
            {
                return;
            }
        }

        UpdateRunningState();
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

    internal void Start(CountdownStartInfo startInfo)
    {
        SetStartInfo(startInfo);
        InternalStart();
    }

    private void SetStartInfo(CountdownStartInfo value)
    {
        AutoSwitchInterval = value.AutoSwitchInterval;
        ExamIndex = value.ExamIndex;
        EnableAutoSwitch = value.AutoSwitch;
        Exams = value.Exams;
        ExamsCount = Exams.Length;
        Info = value;
        DefaultColor = value.DefaultColor;
    }

    private void InternalStart()
    {
        UpdateExams();
        OnExamSwitched();
        UpdateRunningState();
    }

    private void UpdateRunningState()
    {
        if (!IsDisposing)
        {
            if (!IsAlive)
            {
                StopMainTimer();
                StopAutoSwitchTimer();
            }
            else if (Info != null)
            {
                TryStartMainTimer();
                ResetAutoSwitchTimer();
            }
        }
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
        CurrentEvaluator = new(CurrentExam, Info);
        CanStart = CurrentEvaluator.CanStart;
    }

    private void AutoSwitchCallback(object state)
    {
        if (!IsDisposing && IsAlive)
        {
            var i = ExamIndex;

            do
            {
                ExamIndex = (ExamIndex + 1) % ExamsCount;
                UpdateExams();
            }
            while (!CurrentEvaluator.TestExam(DateTime.Now, out _, out _) && ExamIndex != i);

            TryStartMainTimer();
            OnExamSwitched();
        }
    }

    private void CountdownCallback(object state)
    {
        if (!IsDisposing && IsAlive)
        {
            if (CurrentEvaluator != null && CurrentEvaluator.TryBuild(DateTime.Now, out var content, out var colors))
            {
                OnCountdownUpdated(content, colors);
            }
            else
            {
                StopMainTimer();
                OnCountdownUpdated(WelcomeText, DefaultColor);
            }
        }
    }

    private void OnExamSwitched()
    {
        if (ExamIndex != LastExamIndex)
        {
            SafeExecutionContext.Post(OnExamSwitchedInvoker.WithArgs(ExamIndex));
            LastExamIndex = ExamIndex;
        }
    }

    private void OnCountdownUpdated(string content, ColorPair colors)
    {
        SafeExecutionContext.Post(OnCountdownUpdatedInvoker.WithArgs(content, colors));
    }

    private void StopAutoSwitchTimer()
    {
        AutoSwitchTimer.Destroy();
    }

    private void StopMainTimer()
    {
        MainTimer.Destroy();
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