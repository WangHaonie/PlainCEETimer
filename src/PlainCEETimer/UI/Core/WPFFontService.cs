using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.Views;

namespace PlainCEETimer.UI.Core;

public class WPFFontService(AppWindow window) : IUnifiedFontService
{
    public bool? ShowFontDialog(UnifiedFont font)
    {
        var dialog = new FontDialog(font.Font1);
        var result = dialog.ShowDialog(window);

        if (result == true)
        {
            font.Font1 = dialog.Font;
        }

        return result;
    }

    public string GetFontDesc(UnifiedFont font)
    {
        return font.Font1.ToString().Truncate(35);
    }
}