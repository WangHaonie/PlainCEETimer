﻿using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
{
    public sealed class ComboBoxEx : ComboBox
    {
        private bool Calculated;

        public ComboBoxEx()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            FlatStyle = FlatStyle.System;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = ThemeManager.DarkFore;
                BackColor = ThemeManager.DarkBack;
                ThemeManager.FlushControl(this, NativeStyle.CFD);
            }

            base.OnHandleCreated(e);
        }

        protected override void OnDropDown(EventArgs e)
        {
            /*
             
            DropDown 自适应大小 参考:

            c# - Auto-width of ComboBox's content - Stack Overflow
            https://stackoverflow.com/a/16435431/21094697

            c# - ComboBox auto DropDownWidth regardless of DataSource type - Stack Overflow
            https://stackoverflow.com/a/69350288/21094697
             
             */

            if (!Calculated)
            {
                int w = 0;

                foreach (var Item in Items)
                {
                    w = Math.Max(w, TextRenderer.MeasureText(GetItemText(Item), Font).Width);
                }

                DropDownWidth = w + ThemeManager.VerticalScrollBarWidth;
                Calculated = true;
            }

            base.OnDropDown(e);
        }
    }
}
