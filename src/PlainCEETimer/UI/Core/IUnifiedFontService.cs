namespace PlainCEETimer.UI.Core;

public interface IUnifiedFontService
{
    bool? ShowFontDialog(UnifiedFont font);

    string GetFontDesc(UnifiedFont font);
}
