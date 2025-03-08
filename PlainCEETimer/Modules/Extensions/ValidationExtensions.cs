using System;

namespace PlainCEETimer.Modules.Extensions
{
    public static class ValidationExtensions
    {
        public static bool IsValid(this DateTime dateTime)
            => dateTime >= new DateTime(1753, 1, 1, 0, 0, 0) || dateTime <= new DateTime(9998, 12, 31, 23, 59, 59);

        public static bool IsValid(this int ExamLength)
            => ExamLength <= ConfigPolicy.MaxExamNameLength && ExamLength >= ConfigPolicy.MinExamNameLength;
    }
}