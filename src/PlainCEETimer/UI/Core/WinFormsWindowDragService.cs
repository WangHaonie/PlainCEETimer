using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.WPF.Extensions;

namespace PlainCEETimer.UI.Core;

public class WinFormsWindowDragService : IWindowDragService
{
    public bool IsDragging => drag;

    public event CancelEventHandler DragRequest;

    public event EventHandler DragStart;

    public event EventHandler<DragEndEventArgs> DragEnd;

    private bool drag;
    private Point LastLocation;
    private Point LastMouseLocation;
    private readonly AppForm form;

    public WinFormsWindowDragService(AppForm appForm)
    {
        form = appForm;
        form.MouseDown += Form_MouseDown;
        form.MouseMove += Form_MouseMove;
        form.MouseUp += Form_MouseUp;
    }

    private void Form_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            var cea = new CancelEventArgs();
            DragRequest?.Invoke(this, cea);

            if (!cea.Cancel)
            {
                form.Cursor = Cursors.SizeAll;
                LastLocation = form.Location;
                LastMouseLocation = e.Location;
                DragStart?.Invoke(this, EventArgs.Empty);
                drag = true;
            }
        }
    }

    private void Form_MouseMove(object sender, MouseEventArgs e)
    {
        if (drag)
        {
            var mpos = Cursor.Position;
            form.SetLocation(mpos.X - LastMouseLocation.X, mpos.Y - LastMouseLocation.Y);
        }
    }

    private void Form_MouseUp(object sender, MouseEventArgs e)
    {
        if (drag)
        {
            form.Cursor = Cursors.Default;
            drag = false;
            DragEnd?.Invoke(this, new(LastLocation.ToDouble(), form.Location.ToDouble()));
        }
    }
}