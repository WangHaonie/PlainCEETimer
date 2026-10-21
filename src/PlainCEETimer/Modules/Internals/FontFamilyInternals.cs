using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Media;

internal static class FontFamilyInternals
{
    private static readonly FieldInfo s_fiFamily;
    private static readonly PropertyInfo s_piFirstFontFamily;
    private static readonly PropertyInfo s_piOrdinalName;
    private static readonly Type s_typePhysicalFontFamily;

    static FontFamilyInternals()
    {
        var fft = typeof(FontFamily);
        var ass = fft.Assembly;
        var pfft = ass.GetType("MS.Internal.FontFace.PhysicalFontFamily");
        var tifft = ass.GetType("MS.Internal.Text.TextInterface.FontFamily");
        pfft.RevealMember(out s_fiFamily, "_family");
        fft.RevealMember(out s_piFirstFontFamily, "FirstFontFamily");
        tifft.RevealMember(out s_piOrdinalName, "OrdinalName");
        s_typePhysicalFontFamily = pfft;
    }

    public static string GetFirstFontFamilyName(FontFamily instance)
    {
        if (instance != null)
        {
            var fff = s_piFirstFontFamily.GetValue(instance);

            if (fff?.GetType() == s_typePhysicalFontFamily)
            {
                var pff = s_fiFamily.GetValue(fff);

                if (pff != null)
                {
                    return (string)s_piOrdinalName.GetValue(pff);
                }
            }
        }

        return string.Empty;
    }
}