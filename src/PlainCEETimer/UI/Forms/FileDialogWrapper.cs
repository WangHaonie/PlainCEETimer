using System.Windows.Forms;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Forms;

internal sealed class FileDialogWrapper(string title) : AppForm
{
    private PlainLabel LabelMessage;

    protected override void OnInitializing()
    {
        Text = nameof(FileDialogWrapper);
        TopMost = false;
        MinimizeBox = false;
        ControlBox = false;
        StartPosition = FormStartPosition.WindowsDefaultLocation;
        this.AddControls(b => [LabelMessage = b.Label(title)]);
    }

    protected override void RunLayout(bool isHighDpi)
    {
        InitWindowSize(LabelMessage, 1, 3);
    }
}
