using System;
using System.ComponentModel;
using System.Windows.Input;
using PlainCEETimer.WPF.Controls;

namespace PlainCEETimer.UI.Core;

public class WPFWindowDragService : IWindowDragService
{
    public bool IsDragging => drag;

    public event CancelEventHandler DragRequest;

    public event EventHandler DragStart;

    public event EventHandler<DragEndEventArgs> DragEnd;

    private bool drag;
    private readonly AppWindow window;

    public WPFWindowDragService(AppWindow appWindow)
    {
        window = appWindow;
        window.MouseLeftButtonDown += Window_MouseLeftButtonDown;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var cea = new CancelEventArgs();
        DragRequest?.Invoke(this, cea);

        if (!cea.Cancel)
        {
            var p = window.Location;
            DragStart?.Invoke(this, EventArgs.Empty);
            drag = true;
            window.Cursor = Cursors.SizeAll;
            window.DragMove();
            window.Cursor = null;
            DragEnd?.Invoke(this, new(p, window.Location));
        }
    }
}