using System;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsWindowInitializer(AppForm appForm) : IWindowInitializer
{
    public event EventHandler Initialize
    {
        add => appForm.Shown += value;
        remove => appForm.Shown -= value;
    }
}