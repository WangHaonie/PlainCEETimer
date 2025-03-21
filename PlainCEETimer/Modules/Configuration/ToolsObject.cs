using PlainCEETimer.Forms;
using System;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class ToolsObject
    {
        public int NtpServer
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
    }
}
