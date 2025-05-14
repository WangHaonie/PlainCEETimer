using System;
using System.ComponentModel;
using PlainCEETimer.Forms;

namespace PlainCEETimer.Modules.Configuration
{
    public class DisplayObject
    {
        public bool ShowXOnly { get; set; }

        public int X
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded && (value is < 0 or > 3))
                {
                    throw new Exception();
                }

                field = value;
            }
        }

        public bool Ceiling { get; set; }

        [DefaultValue(2)]
        public int EndIndex
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded && (value is < 0 or > 2))
                {
                    throw new Exception();
                }

                field = value;
            }
        } = 2;

        public bool CustomText { get; set; }

        public int ScreenIndex { get; set; }

        [DefaultValue(CountdownPosition.TopCenter)]
        public CountdownPosition Position { get; set; } = CountdownPosition.TopCenter;

        public bool Draggable { get; set; }

        public bool SeewoPptsvc { get; set; }
    }
}
