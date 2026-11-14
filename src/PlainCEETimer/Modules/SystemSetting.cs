using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Annotations.SourceGenerators;

namespace PlainCEETimer.Modules;

public sealed partial class SystemSetting<T> : ISystemSetting
{
    [BackingField("value")]
    public partial T Value { get; }

    public event EventHandler<SystemSettingChangedEventArgs<T>> Changed;

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
