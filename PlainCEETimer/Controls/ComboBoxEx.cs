using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class ComboBoxEx : ComboBox
    {
        protected override void OnDropDown(EventArgs e)
        {
            #region
            /*
             
            DropDown 自适应大小 参考:

            c# - Auto-width of ComboBox's content - Stack Overflow
            https://stackoverflow.com/a/16435431/21094697
            c# - ComboBox auto DropDownWidth regardless of DataSource type - Stack Overflow
            https://stackoverflow.com/a/69350288/21094697
             
             */
            int MaxWidth = 0;

            foreach (var Item in Items)
            {
                MaxWidth = Math.Max(MaxWidth, TextRenderer.MeasureText(GetItemText(Item), Font).Width);
            }

            DropDownWidth = MaxWidth + SystemInformation.VerticalScrollBarWidth;
            #endregion

            base.OnDropDown(e);
        }
    }
}
