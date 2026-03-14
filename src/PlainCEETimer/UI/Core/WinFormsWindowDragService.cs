using System;
using System.ComponentModel;
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
    private readonly AppForm form;

    public WinFormsWindowDragService(AppForm appForm)
    {
        form = appForm;
        form.MouseDown += Form_MouseDown;
    }

    private void Form_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            var cea = new CancelEventArgs();
            DragRequest?.Invoke(this, cea);

            if (!cea.Cancel)
            {
                var p = form.Location.ToDouble();
                DragStart?.Invoke(this, EventArgs.Empty);
                drag = true;
                form.Cursor = Cursors.SizeAll;
                form.DragMove();
                form.Cursor = Cursors.Default;
                drag = false;
                DragEnd?.Invoke(this, new(p, form.Location.ToDouble()));
            }
        }
    }
}