namespace PlainCEETimer.Modules
{
    public static class ConfigPolicy
    {
        public const int MaxExamNameLength = 10;
        public const int MinExamNameLength = 2;
        public const int MaxFontSize = 36;
        public const int MinFontSize = 10;
        public const double MaxTick = 5662310399D; // 65535d 23h 59m 59s
        public const double MinTick = 1D; // 1s
        public const long MaxDate = 3155063615990000000L; // 9998-12-31 23:59:59
        public const long MinDate = 552877920000000000L; // 1753-01-01 00:00:00
        public const int MaxCustomTextLength = 800;
        public const char ValueSeparator = ',';
        public const string ValueSeparatorString = ", ";
    }
}
