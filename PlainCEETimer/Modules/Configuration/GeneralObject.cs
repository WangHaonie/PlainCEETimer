using System.ComponentModel;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class GeneralObject
    {
        public bool AutoSwitch { get; set; }

        public int Interval { get; set; }

        [DefaultValue(true)]
        public bool TrayIcon { get; set; } = true;

        public bool TrayText { get; set; }

        public bool MemClean { get; set; }

        [DefaultValue(true)]
        public bool TopMost { get; set; } = true;

        [DefaultValue(true)]
        public bool UniTopMost { get; set; } = true;

        [DefaultValue(true)]
        public bool WCCMS { get; set; } = true;
    }
}
