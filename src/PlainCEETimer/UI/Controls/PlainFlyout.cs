using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public abstract class PlainFlyout : AppForm
{
    protected sealed override bool SuppressAutoPosition => true;

    protected virtual Point Offset => Point.Empty;

    protected abstract Control OwnerControl { get; }

    private bool fromParent;
    private AppForm m_parentForm;
    private BlurOverlay BlurOverlayMain;

    public new void Show()
    {
        BlurOverlayMain.CaptureCurrent();
        BlurOverlayMain.Show();
        Show(m_parentForm);
    }

    protected override void OnInitializing()
    {
        m_parentForm = OwnerControl.FindParentForm();
        m_parentForm.LocationChanged += ParentForm_LocationChanged;
        m_parentForm.VisibleChanged += ParentForm_VisibleChanged;

        m_parentForm.AddControls(b =>
        [
            BlurOverlayMain = b.New<BlurOverlay>(0, 0, null),
        ]);
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);

        if (!fromParent)
        {
            UpdateLocation();
        }
    }

    protected sealed override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateLocation();
    }

    protected override void OnClosed()
    {
        m_parentForm.LocationChanged -= ParentForm_LocationChanged;
        m_parentForm.VisibleChanged -= ParentForm_VisibleChanged;
        BlurOverlayMain.Hide();
    }

    private void ParentForm_LocationChanged(object sender, EventArgs e)
    {
        fromParent = true;
        UpdateLocation();
        fromParent = false;
    }

    private void ParentForm_VisibleChanged(object sender, EventArgs e)
    {
        Hide(!m_parentForm.IsVisible);
    }

    private void UpdateLocation()
    {
        var offset = Offset;
        Location = OwnerControl.LocationToScreen(offset.X, offset.Y);
    }
}
