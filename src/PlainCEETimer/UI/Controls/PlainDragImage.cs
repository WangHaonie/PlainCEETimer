using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

[NoConstants]
public sealed class PlainDragImage : PlainDropDown
{
    protected override AppWindowStyle Params
    {
        get
        {
            var p = base.Params;
            p |= AppWindowStyle.Special;
            return p;
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= WinUser.WS_EX_NOACTIVATE;
            return cp;
        }
    }

    protected override bool ShowWithoutActivation => true;

    protected override bool UseStyles => false;

    private readonly Point m_offset;
    private readonly Bitmap m_img;

    private const double ImageOpacity = 0.6;

    private PlainDragImage(Control source, Point hotSpot)
    {
        m_offset = hotSpot;
        m_img = CaptureControl(source);
        Opacity = ImageOpacity;
        ClientSize = m_img.Size;
        Text = "DragImage - 高考倒计时";
        source.Disposed += (_, _) => Stop();
    }

    public new void Move(Point p)
    {
        ApplyPosition(p, WinUser.SWP_NOSIZE);
    }

    public void Stop()
    {
        Close();
    }

    public static PlainDragImage Start(Control source, Point hotSpot)
    {
        var di = new PlainDragImage(source, hotSpot);
        di.ShowAt(Cursor.Position);
        return di;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.DrawImage(m_img, 0, 0);
    }

    protected override void Dispose(bool disposing)
    {
        m_img.Destroy();
        base.Dispose(disposing);
    }

    private void ShowAt(Point p)
    {
        ApplyPosition(p, WinUser.SWP_SHOWWINDOW);
    }

    private void ApplyPosition(Point p, int uFlags)
    {
        Win32UI.SetWindowPos(Handle, WinUser.HWND_TOPMOST,
            p.X - m_offset.X, p.Y - m_offset.Y, m_img.Width, m_img.Height, WinUser.SWP_NOACTIVATE | uFlags);
    }

    private static Bitmap CaptureControl(Control c)
    {
        var sz = c.ClientSize;
        var cx = Math.Max(1, sz.Width);
        var cy = Math.Max(1, sz.Height);
        var bm = new Bitmap(cx, cy, PixelFormat.Format32bppRgb);

        if (PrintWindow(c.Handle, bm))
        {
            return bm;
        }

        c.DrawToBitmap(bm, new(0, 0, cx, cy));
        return bm;
    }

    private static bool PrintWindow(IntPtr hWnd, Bitmap image)
    {
        using var g = Graphics.FromImage(image);
        return Win32UI.PrintWindow(hWnd, g.GetHdc(), WinUser.PW_CLIENTONLY);
    }
}
