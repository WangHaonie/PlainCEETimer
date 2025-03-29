using System;

namespace PlainCEETimer.Modules.Extensions
{
    public static class ValidationExtensions
    {
        public static bool IsValid(this DateTime ExamTime)
            => ExamTime.Ticks is >= ConfigPolicy.MinDate and <= ConfigPolicy.MaxDate;

        public static bool IsValid(this int ExamLength)
            => ExamLength is >= ConfigPolicy.MinExamNameLength and <= ConfigPolicy.MaxExamNameLength;
    }
}