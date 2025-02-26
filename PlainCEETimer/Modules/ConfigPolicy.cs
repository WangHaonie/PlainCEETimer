using System;

namespace PlainCEETimer.Modules
{
    public static class ConfigPolicy
    {
        public static int MinExamNameLength => 2;
        public static int MaxExamNameLength => 10;
        public static int MaxCustomTextLength => 50;
        public static int MinFontSize => 10;
        public static int MaxFontSize => 28;
        public static char ValueSeparator => ',';
        public static string ValueSeparatorString => ", ";
        public static TimeSpan TsMaxAllowed => new(65535, 23, 59, 59);
        public static TimeSpan TsMinAllowed => new(0, 0, 0, 1);
    }
}
