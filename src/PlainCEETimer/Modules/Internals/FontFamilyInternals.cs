using System.Reflection;

namespace System.Windows.Media;

internal static class FontFamilyInternals
{
    private static readonly FieldInfo m_fiFamily;
    private static readonly PropertyInfo m_piFirstFontFamily;
    private static readonly PropertyInfo m_piOrdinalName;
    private static readonly Type m_typePhysicalFontFamily;

    static FontFamilyInternals()
    {
        var fft = typeof(FontFamily);
        var ass = fft.Assembly;
        var pfft = ass.GetType("MS.Internal.FontFace.PhysicalFontFamily");
        var tifft = ass.GetType("MS.Internal.Text.TextInterface.FontFamily");
        var battr = BindingFlags.NonPublic | BindingFlags.Instance;

        m_fiFamily = pfft.GetField("_family", battr);
        m_piFirstFontFamily = fft.GetProperty("FirstFontFamily", battr);
        m_piOrdinalName = tifft.GetProperty("OrdinalName", battr);
        m_typePhysicalFontFamily = pfft;
    }

    public static string GetFirstFontFamilyName(FontFamily instance)
    {
        if (instance != null)
        {
            var fff = m_piFirstFontFamily.GetValue(instance);

            if (fff != null && fff.GetType() == m_typePhysicalFontFamily)
            {
                var pff = m_fiFamily.GetValue(fff);

                if (pff != null)
                {
                    return (string)m_piOrdinalName.GetValue(pff);
                }
            }
        }

        return string.Empty;
    }
}