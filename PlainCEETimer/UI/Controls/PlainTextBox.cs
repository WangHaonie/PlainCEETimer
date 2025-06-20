﻿using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
{
    public sealed class PlainTextBox : TextBox
    {
        public PlainTextBox()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = ThemeManager.DarkFore;
                BackColor = ThemeManager.DarkBack;
            }

            MaxLength = Validator.MaxCustomTextLength;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushControl(this, NativeStyle.CFD);
            }

            base.OnHandleCreated(e);
        }
    }
}
