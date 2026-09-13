using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PlainCEETimer.Modules;

namespace PlainCEETimer.WPF.Converters;

[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
public class ColorToBrushConverter : IValueConverter, IValueConverter<Color, SolidColorBrush>
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color c)
        {
            return Convert(c);
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush br)
        {
            return ConvertBack(br);
        }

        return Binding.DoNothing;
    }

    public SolidColorBrush Convert(Color value)
    {
        var b = new SolidColorBrush(value);
        b.Freeze();
        return b;
    }

    public Color ConvertBack(SolidColorBrush value)
    {
        return value.Color;
    }
}
