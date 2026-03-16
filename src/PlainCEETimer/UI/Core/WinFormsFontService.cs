using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsFontService(AppForm form) : IUnifiedFontService
{
    public bool? ShowFontDialog(UnifiedFont font)
    {
        var dialog = new PlainFontDialog(form, font.Font2);
        var result = dialog.ShowDialog();

        if (result == true)
        {
            font.Font2 = dialog.Font;
        }

        return result;
    }

    public string GetFontDesc(UnifiedFont font)
    {
        return font.Font2.Format().Truncate(35);
    }
}