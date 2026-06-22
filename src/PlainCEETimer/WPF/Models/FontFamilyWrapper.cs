using System.Windows.Media;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.WPF.Models;

public class FontFamilyWrapper(string name)
{
    public string Name => _name;

    public string FirstFontFamilyName => field ??= GetFirstFontFamilyName();

    public FontFamily Value => field ??= new(_name);

    private readonly string _name = name.Compact();

    private string GetFirstFontFamilyName()
    {
        return FontFamilyInternals.GetFirstFontFamilyName(Value);
    }
}
