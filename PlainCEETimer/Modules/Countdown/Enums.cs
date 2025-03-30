using System;

namespace PlainCEETimer.Modules.Countdown
{
    public enum ExamState
    {
        NotStarted,
        InProgress,
        Ended
    }

    /// <summary>
    /// 表示倒计时要显示哪些阶段
    /// </summary>
    [Flags]
    public enum CountdownMode
    {
        /// <summary>
        /// 只显示 P1 阶段。
        /// </summary>
        Mode1 = 0b001,

        /// <summary>
        /// 显示 P1 和 P2 阶段。
        /// </summary>
        Mode2 = Mode1 | 0b010,

        /// <summary>
        /// 显示全部阶段。
        /// </summary>
        Mode3 = Mode2 | 0b100
    }
}
