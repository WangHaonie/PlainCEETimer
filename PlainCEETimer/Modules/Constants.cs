﻿namespace PlainCEETimer.Modules
{
    public static class Constants
    {
        public const string Add = "添加(&A)";
        public const string Edit = "编辑(&E)";
        public const string Delete = "删除(&D)";
        public const string SelectAll = "全选(&Q)";

        public const string Switch = "切换(&Q)";
        public const string AddExamInfo = "请先添加考试信息";
        public const string Settings = "设置(&S)";
        public const string About = "关于(&A)";
        public const string InstallDir = "安装目录(&D)";

        public const string Show = "显示界面(&S)";
        public const string Close = "关闭(&C)";
        public const string Restart = "重启(&R)";
        public const string Quit = "退出(&Q)";

        public const string Light = "白底(&L)";
        public const string Dark = "黑底(&D)";

        public const string PH_EXAMNAME = "{x}";
        public const string PH_DAYS = "{d}";
        public const string PH_HOURS = "{h}";
        public const string PH_MINUTES = "{m}";
        public const string PH_SECONDS = "{s}";
        public const string PH_CEILINGDAYS = "{cd}";
        public const string PH_TOTALHOURS = "{th}";
        public const string PH_TOTALMINUTES = "{tm}";
        public const string PH_TOTALSECONDS = "{ts}";
        public const string PH_START = "还有";
        public const string PH_LEFT = "结束还有";
        public const string PH_PAST = "已过去了";
        public const string PH_RTP1 = $"开始{PH_START}";
        public const string PH_RTP2 = PH_LEFT;
        public const string PH_RTP3 = PH_PAST;
        public const string PH_P1 = $"距离{PH_EXAMNAME}{PH_START}{PH_DAYS}天{PH_HOURS}时{PH_MINUTES}分{PH_SECONDS}秒";
        public const string PH_P2 = $"距离{PH_EXAMNAME}{PH_LEFT}{PH_DAYS}天{PH_HOURS}时{PH_MINUTES}分{PH_SECONDS}秒";
        public const string PH_P3 = $"距离{PH_EXAMNAME}{PH_PAST}{PH_DAYS}天{PH_HOURS}时{PH_MINUTES}分{PH_SECONDS}秒";

        public const int ERROR_CANCELLED = 0x4C7;
    }
}
