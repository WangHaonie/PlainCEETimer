using System.Windows.Media;

namespace PlainCEETimer.WPF.Models;

public class FontFamilyWrapper(string name)
{
    public string Name => _name;

    public FontFamily Value => field ??= new(_name);

    private readonly string _name = name.Trim();
}