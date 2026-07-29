using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class BlurOverlay : Panel
{
    private AppForm parent;
    private Bitmap screenshot;

    public BlurOverlay()
    {
        Visible = false;
        Dock = DockStyle.Fill;
    }

    public new void Show()
    {
        BitmapFilter.GaussianBlur(screenshot, 2);
        BackgroundImage = screenshot;
        BackgroundImageLayout = ImageLayout.Stretch;
        base.Show();
        BringToFront();
    }

    public new void Hide()
    {
        base.Hide();
    }

    public void CaptureCurrent()
    {
        parent ??= this.FindParentForm();
        screenshot.Destroy();
        var rc = parent.ClientRectangle;
        var bmp = new Bitmap(rc.Width, rc.Height);
        using var g = Graphics.FromImage(bmp);
        g.CopyFromScreen(parent.PointToScreen(rc.Location), Point.Empty, rc.Size);
        screenshot = bmp;
    }

    protected override void OnClick(EventArgs e)
    {
        SystemSounds.Beep.Play();
        base.OnClick(e);
    }
}
