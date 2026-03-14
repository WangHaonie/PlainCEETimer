using System;
using System.ComponentModel;

namespace PlainCEETimer.UI.Core;

public interface IWindowDragService
{
    bool IsDragging { get; }

    event CancelEventHandler DragRequest;

    event EventHandler DragStart;

    event EventHandler<DragEndEventArgs> DragEnd;
}