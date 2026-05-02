using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.UI;

public class FileDialogHelper
{
    public static bool? ShowDialog<TFileDialog>(string title, string fileName, out TFileDialog dialog, params FileFilter[] filters)
        where TFileDialog : FileDialog, new()
    {
        dialog = GetFileDialog<TFileDialog>(title, fileName, filters);
        var wrapper = new FileDialogWrapper(title);
        wrapper.Show();
        var result = ShowDialog(dialog, wrapper);
        wrapper.Close();
        return result;
    }

    public static bool? ShowDialog<TFileDialog>(string title, string fileName, IWin32Window parent, out TFileDialog dialog, params FileFilter[] filters)
        where TFileDialog : FileDialog, new()
    {
        dialog = GetFileDialog<TFileDialog>(title, fileName, filters);
        return ShowDialog(dialog, parent);
    }

    private static TFileDialog GetFileDialog<TFileDialog>(string title, string fileName, params FileFilter[] filters)
        where TFileDialog : FileDialog, new()
    {
        return new TFileDialog
        {
            Title = title,
            FileName = fileName,
            Filter = string.Join("|", filters)
        };
    }

    private static bool? ShowDialog<TFileDialog>(TFileDialog dialog, IWin32Window parent)
        where TFileDialog : FileDialog
    {
        using (new DpiAwarenessContextScope(DpiAwarenessContext.PerMonitorV2))
        {
            return dialog.ShowDialog(parent).AsBoolean();
        }
    }
}