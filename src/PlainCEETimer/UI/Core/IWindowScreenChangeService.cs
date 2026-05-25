using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public interface IWindowScreenChangeService : IDisposable
{
    Screen Current { get; }

    event EventHandler<ScreenChangedEventArgs> ScreenChanged;
}
