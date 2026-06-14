using System.Drawing;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.UI.Core;

public class UnifiedFont
{
    public FontModel DxFont { get; set; }

    public Font GdiFont { get; set; }

    internal void Sync(bool refGdi)
    {
        if (refGdi)
        {
            DxFont = FontModel.FromGdiFont(GdiFont);
        }
        else
        {
            var font = DxFont.ToGdiFont();

            if (font != null)
            {
                GdiFont = font;
            }
        }
    }
}