using System.Windows.Data;

namespace PlainCEETimer.Modules;

public interface IValueConverter<TIn, TOut> : IValueConverter
{
    TOut Convert(TIn value);

    TIn ConvertBack(TOut value);
}
