using System;
using System.Drawing;

namespace PlainCEETimer.UI.Core;

public interface ITrayIconLoader : IHasContextMenu, IDisposable
{
    string Text { get; set; }

    Icon Icon { get; set; }

    bool Visible { get; set; }
}