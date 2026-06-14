using System.Reflection;
using System.Windows.Media;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.WPF.Models;

public class FontFamilyWrapper(string name)
{
    public string Name => _name;

    public string FirstFontFamilyName => field ??= ReflectGetFirstFamily();

    public FontFamily Value => field ??= new(_name);

    private readonly string _name = name.Compact();

    private string ReflectGetFirstFamily()
    {
        var ff = Value;
        var fft = typeof(FontFamily);
        var fff = fft.GetProperty("FirstFontFamily", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ff);

        if (fff != null)
        {
            var pfft = fft.Assembly.GetType("MS.Internal.FontFace.PhysicalFontFamily");

            if (fff.GetType() == pfft)
            {
                var pff = pfft.GetField("_family", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(fff);

                if (pff != null)
                {
                    return (string)fft.Assembly
                        .GetType("MS.Internal.Text.TextInterface.FontFamily")
                        .GetProperty("OrdinalName", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(pff);
                }
            }
        }

        return string.Empty;
    }
}