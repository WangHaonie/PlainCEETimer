using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PlainCEETimer.WPF.Converters;

public class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color c)
        {
            var b = new SolidColorBrush(c);
            b.Freeze();
            return b;
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
