using PlainCEETimer.Interop;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public static class Extensions
    {
        public static double DpiRatio { get; private set; } = 0;
        public static string ToArgbString(this Color color) => color.ToArgbInt().ToString();
        public static int ToArgbInt(this Color color) => -color.ToArgb();
        public static TimeSpan ToTimeSpan(this string timespan, char[] Separator) => GetTimeSpan(timespan.Split(Separator));
        public static string ToMessage(this Exception ex) => $"\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";
        public static string RemoveIllegalChars(this string s) => new([.. s.Trim().Where(x => !IllegalChars.Contains(x))]);

        #region 来自网络
        /*

        截断字符串 参考:

        c# - How do I truncate a .NET string? - Stack Overflow
        https://stackoverflow.com/a/2776689

        */
        public static string Truncate(this string s, int MaxLength)
            => s?.Length > MaxLength ? s.Substring(0, MaxLength) + "..." : s;
        #endregion

        public static int ScaleToDpi(this int px, Control ctrl)
        {
            int pxScaled;
            DpiRatio = NativeInterop.GetDpiForWindow(ctrl.Handle) / 96D;
            pxScaled = (int)(px * DpiRatio);
            return pxScaled;
        }

        public static bool IsValid(this DateTime dateTime)
            => dateTime >= new DateTime(1753, 1, 1, 0, 0, 0) || dateTime <= new DateTime(9998, 12, 31, 23, 59, 59);

        public static bool IsValid(this int ExamLength)
            => ExamLength <= ConfigPolicy.MaxExamNameLength && ExamLength >= ConfigPolicy.MinExamNameLength;

        private static readonly char[] IllegalChars = [' ', '\n', '\r', '\t', '\v', '\f', '\b'];

        private static TimeSpan GetTimeSpan(string[] Splited)
        {
            int d = int.Parse(Splited[0]);
            int h = int.Parse(Splited[1]);
            int m = int.Parse(Splited[2]);
            int s = int.Parse(Splited[3]);

            var ts = new TimeSpan(d, h, m, s);

            if (ts >= ConfigPolicy.TsMinAllowed || ts <= ConfigPolicy.TsMaxAllowed)
            {
                return ts;
            }

            throw new Exception();
        }
    }
}