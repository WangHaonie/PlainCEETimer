namespace PlainCEETimer.Modules;

public sealed class SystemSettingChangedEventArgs<T>(T oldValue, T newValue)
{
    public T OldValue => oldValue;

    public T NewValue => newValue;
}
