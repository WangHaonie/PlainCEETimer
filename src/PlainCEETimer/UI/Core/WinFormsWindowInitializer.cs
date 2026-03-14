using System;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsWindowInitializer : IWindowInitializer
{
    public event EventHandler Initialize;

    public WinFormsWindowInitializer(AppForm appForm)
    {
        appForm.Shown += (_, e) => Initialize?.Invoke(this, e);
    }
}