using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsFontService(AppForm form) : IUnifiedFontService
{
    public bool? ShowFontDialog(UnifiedFont font)
    {
        var dialog = new PlainFontDialog(form, font.GdiFont);
        var result = dialog.ShowDialog();

        if (result == true)
        {
            font.GdiFont = dialog.Font;
        }

        return result;
    }

    public string GetFontDesc(UnifiedFont font)
    {
        return font.GdiFont.Format().Truncate(35);
    }
}