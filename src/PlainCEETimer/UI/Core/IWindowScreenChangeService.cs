using System;

namespace PlainCEETimer.UI.Core;

public interface IWindowScreenChangeService : IDisposable
{
    event EventHandler<ScreenChangedEventArgs> ScreenChanged;
}
