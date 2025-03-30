using PlainCEETimer.Modules.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace PlainCEETimer.Modules.Countdown
{
    public class DefaultCountdown : ICountdownProvider
    {
        public event CountdownUpdatedHandler CountdownUpdated;

        public ExamInfoObject[] Exams { get; set; }
        public ColorSetObject[] GlobalColors { get; set; }
        public string[] GlobalTexts { get; set; }
        public CustomRuleObject[] Rules { get; set; }
        public int ExamIndex { get; set; }
        public CountdownMode Mode { get; set; }
        public CountdownState State { get; set; }
        public bool AutoSwitch { get; set; }
        public int AutoSwitchInterval { get; set; }
        public bool UseCustomText { get; set; }
        public bool IsRunning { get; private set; }

        private string CurrentName;
        private CountdownPhase CurrentPhase = (CountdownPhase)3;
        private DateTime CurrentStart;
        private DateTime CurrentEnd;
        private IEnumerable<CustomRuleObject> CurrentRule;
        private Timer InnerTimer;
        private Timer AutoSwitchHandler;
        private readonly SynchronizationContext SyncContext;

        private static readonly StringBuilder Builder = new();
        private static readonly string[] DefaultTexts = [Placeholders.PH_START, Placeholders.PH_LEFT, Placeholders.PH_PAST];

        public DefaultCountdown()
        {
            SyncContext = SynchronizationContext.Current;
        }

        public void Start()
        {
            if (UpdateExam() && !IsRunning && InnerTimer == null)
            {
                InnerTimer = new(Main, null, 0, 1000);

                if (AutoSwitch && AutoSwitchHandler == null)
                {
                    AutoSwitchHandler = new(AutoSwitchExam, null, 5000, 5000);
                }

                IsRunning = true;
            }
        }

        private bool UpdateExam()
        {
            if (Exams == null || (GlobalColors == null && GlobalColors.Length < 4))
            {
                return false;
            }

            var Index = ExamIndex;
            ExamIndex = Index < Exams.Length ? Index : 0;

            var CurrentExam = Exams[ExamIndex];
            CurrentName = CurrentExam.Name;
            CurrentStart = CurrentExam.Start;
            CurrentEnd = CurrentExam.End;

            if (Mode == CountdownMode.Mode1)
            {
                return true;
            }

            if (CurrentStart >= CurrentEnd)
            {
                return false;
            }

            return true;
        }

        public void JumpTo(int index)
        {
            if (AutoSwitchHandler != null)
            {
                ExamIndex = index;
                Stop(false, true, false);
                Start();
            }
        }

        public void Dispose()
        {
            Stop(ShowWelcome: false);
            GC.SuppressFinalize(this);
        }

        private void Main(object state)
        {
            var Now = DateTime.Now;

            if (Mode == CountdownMode.Mode1 && Now < CurrentStart)
            {
                SetPhase(CountdownPhase.P1);
                ApplyCustomRule(0, CurrentStart - Now);
            }
            else if (Mode == CountdownMode.Mode2 && Now < CurrentEnd)
            {
                SetPhase(CountdownPhase.P2);
                ApplyCustomRule(1, CurrentEnd - Now);
            }
            else if (Mode == CountdownMode.Mode3 && Now > CurrentEnd)
            {
                SetPhase(CountdownPhase.P3);
                ApplyCustomRule(2, Now - CurrentEnd);
            }
            else
            {
                Stop();
            }
            Report();
        }

        private void AutoSwitchExam(object state)
        {
            ExamIndex = (ExamIndex + 1) % Exams.Length;
            Start();
            Report();
        }

        private void Report([CallerMemberName] string Name = "")
        {
            Debug.WriteLine(Name + DateTime.Now.ToString("fffffff"));
        }

        private void SetPhase(CountdownPhase phase)
        {
            if (CurrentPhase != phase)
            {
                if (UseCustomText)
                {
                    CurrentRule = Rules.Where(x => x.Phase == phase);
                }

                CurrentPhase = phase;
            }
        }

        private void ApplyCustomRule(int Phase, TimeSpan Span)
        {
            if (UseCustomText)
            {
                if (CurrentRule?.Count() > 0)
                {
                    foreach (var Rule in CurrentRule)
                    {
                        if (Phase == 2 ? (Span >= Rule.Tick) : (Span <= Rule.Tick + new TimeSpan(0, 0, 0, 1)))
                        {
                            OnCountdownUpdated(SetCustomRule(Span, Rule.Text), new(Rule.Fore, Rule.Back));
                            return;
                        }
                    }
                }

                OnCountdownUpdated(SetCustomRule(Span, GlobalTexts[Phase]), GlobalColors[Phase]);
                return;
            }

            OnCountdownUpdated(GetCountdown(Span, DefaultTexts[Phase]), GlobalColors[Phase]);
        }

        private string GetCountdown(TimeSpan Span, string Hint) => State switch
        {
            CountdownState.DaysOnly => string.Format("距离{0}{1}{2}天", CurrentName, Hint, Span.Days),
            CountdownState.DaysOnlyWithCeiling => string.Format("距离{0}{1}{2}天", CurrentName, Hint, Span.Days + 1),
            CountdownState.HoursOnly => string.Format("距离{0}{1}{2:0}小时", CurrentName, Hint, Span.TotalHours),
            CountdownState.MinutesOnly => string.Format("距离{0}{1}{2:0}分钟", CurrentName, Hint, Span.TotalMinutes),
            CountdownState.SecondsOnly => string.Format("距离{0}{1}{2:0}秒", CurrentName, Hint, Span.TotalSeconds),
            _ => string.Format("距离{0}{1}{2}天{3:00}时{4:00}分{5:00}秒", CurrentName, Hint, Span.Days, Span.Hours, Span.Minutes, Span.Seconds)
        };

        private string SetCustomRule(TimeSpan ExamSpan, string Custom)
        {
            Builder.Clear();
            Builder.Append(Custom);
            Builder.Replace(Placeholders.PH_EXAMNAME, CurrentName);
            Builder.Replace(Placeholders.PH_DAYS, $"{ExamSpan.Days}");
            Builder.Replace(Placeholders.PH_HOURS, $"{ExamSpan.Hours:00}");
            Builder.Replace(Placeholders.PH_MINUTES, $"{ExamSpan.Minutes:00}");
            Builder.Replace(Placeholders.PH_SECONDS, $"{ExamSpan.Seconds:00}");
            Builder.Replace(Placeholders.PH_CEILINGDAYS, $"{ExamSpan.Days + 1}");
            Builder.Replace(Placeholders.PH_TOTALHOURS, $"{ExamSpan.TotalHours:0}");
            Builder.Replace(Placeholders.PH_TOTALMINUTES, $"{ExamSpan.TotalMinutes:0}");
            Builder.Replace(Placeholders.PH_TOTALSECONDS, $"{ExamSpan.TotalSeconds:0}");
            return Builder.ToString();
        }

        private void OnCountdownUpdated(string text, ColorSetObject colors)
        {
            SyncContext.Post(_ => CountdownUpdated?.Invoke(text, colors.Fore, colors.Back), null);
        }

        private void Stop(bool StopMain = true, bool StopAutoSwitch = true, bool ShowWelcome = true)
        {
            if (StopMain)
            {
                IsRunning = false;
                InnerTimer?.Dispose();
                InnerTimer = null;
            }

            if (StopAutoSwitch && AutoSwitch && AutoSwitchHandler != null)
            {
                AutoSwitchHandler.Dispose();
                AutoSwitchHandler = null;
            }

            if (ShowWelcome)
            {
                OnCountdownUpdated("欢迎使用高考倒计时", GlobalColors[3]);
            }
        }

        ~DefaultCountdown() => Dispose();
    }
}
