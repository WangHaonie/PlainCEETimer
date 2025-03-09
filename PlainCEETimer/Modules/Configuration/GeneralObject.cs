using PlainCEETimer.Forms;
using System;
using System.ComponentModel;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class GeneralObject
    {
        public ExamInfoObject[] ExamInfo
        {
            get => field ?? [];
            set
            {
                if (value == null)
                {
                    field = [];
                }
                else
                {
                    if (MainForm.ValidateNeeded)
                    {
                        Array.Sort(value);
                    }

                    field = value;
                }
            }
        }

        public int ExamIndex
        {
            get => field;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (value < 0 || value > ExamInfo.Length)
                    {
                        throw new Exception();
                    }
                }

                field = value;
            }
        }

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
