using System;
using PlainCEETimer.WPF.Controls;

namespace PlainCEETimer.UI.Core;

public class WPFWindowInitializer : IWindowInitializer
{
    public event EventHandler Initialize;

    public WPFWindowInitializer(AppWindow appWindow)
    {
        appWindow.Loaded += (_, e) => Initialize?.Invoke(this, e);
    }
}