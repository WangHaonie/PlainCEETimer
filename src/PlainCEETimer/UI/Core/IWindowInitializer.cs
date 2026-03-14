using System;

namespace PlainCEETimer.UI.Core;

public interface IWindowInitializer
{
    event EventHandler Initialize;
}