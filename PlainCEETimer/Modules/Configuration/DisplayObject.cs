using PlainCEETimer.Forms;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class DisplayObject
    {
        public bool ShowXOnly { get; set; }

        public int X
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (value is < 0 or > 3)
                    {
                        throw new Exception();
                    }
                }

                field = value;
            }
        }

        public bool Ceiling { get; set; }

        public int EndIndex
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (value is < 0 or > 2)
                    {
                        throw new Exception();
                    }
                }

                field = value;
            }
        }

        public bool CustomText { get; set; }

        public string[] CustomTexts
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    foreach (var Text in value)
                    {
                        CustomRuleHelper.VerifyText(Text);
                    }
                }

                field = value;
            }
        } = [Placeholders.PH_P1, Placeholders.PH_P2, Placeholders.PH_P3];

        public int ScreenIndex
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (value < 0 || value > Screen.AllScreens.Length)
                    {
                        throw new Exception();
                    }
                }

                field = value;
            }
        }

        [DefaultValue(CountdownPosition.TopCenter)]
        public CountdownPosition Position { get; set; } = CountdownPosition.TopCenter;

        public bool Draggable { get; set; }

        public bool SeewoPptsvc { get; set; }
    }
}
