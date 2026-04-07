using System.Windows.Media;

namespace PlainCEETimer.WPF.Models;

public class FontFamilyWrapper(string name)
{
    public string Name => name;

    public FontFamily Value => field ??= new(name);
}