using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.WPF.Converters;

public class ColorOpacityToBrushConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (!values.IsNullOrEmpty())
        {
            if (values[0] is Color c)
            {
                if (values.Length > 1 && values[1] is double opacity)
                {
                    c.A = (byte)(c.A * opacity.Clamp(0D, 1D));
                }

                var b = new SolidColorBrush(c);
                b.Freeze();
                return b;
            }
        }

        return Brushes.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
