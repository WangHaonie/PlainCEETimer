using System.Reflection;

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
        var battr = BindingFlags.NonPublic | BindingFlags.Instance;

        s_fiFamily = pfft.GetField("_family", battr);
        s_piFirstFontFamily = fft.GetProperty("FirstFontFamily", battr);
        s_piOrdinalName = tifft.GetProperty("OrdinalName", battr);
        s_typePhysicalFontFamily = pfft;
    }

    public static string GetFirstFontFamilyName(FontFamily instance)
    {
        if (instance != null)
        {
            var fff = s_piFirstFontFamily.GetValue(instance);

            if (fff != null && fff.GetType() == s_typePhysicalFontFamily)
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