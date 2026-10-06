using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PlainCEETimer.Modules;

namespace PlainCEETimer.WPF.Converters;

[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
public class ColorToBrushConverter : IValueConverter, IValueConverter<Color, SolidColorBrush>
{
    private readonly ConcurrentDictionary<Color, SolidColorBrush> m_brushCache = new();

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
        return m_brushCache.GetOrAdd(value, static color =>
        {
            var b = new SolidColorBrush(color);
            b.Freeze();
            return b;
        });
    }

    public Color ConvertBack(SolidColorBrush value)
    {
        return value.Color;
    }
}
