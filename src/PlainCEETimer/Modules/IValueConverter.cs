namespace PlainCEETimer.Modules;

public interface IValueConverter<TIn, TOut>
{
    TOut Convert(TIn value);

    TIn ConvertBack(TOut value);
}
