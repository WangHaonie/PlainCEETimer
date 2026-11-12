using System;
using System.Collections.Generic;

namespace PlainCEETimer.Modules;

public sealed class SystemSetting<T> : ISystemSetting
{
    public T Value => value;

    public event EventHandler<SystemSettingChangedEventArgs<T>> Changed;

    private T value;
    private readonly Func<T> GetValue;
    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

    internal SystemSetting(Func<T> fnGetValue)
    {
        GetValue = fnGetValue;
        value = fnGetValue();
    }

    public void Refresh()
    {
        var v = GetValue();

        if (!Comparer.Equals(value, v))
        {
            var old = value;
            value = v;
            Changed?.Invoke(this, new(old, v));
        }
    }
}
