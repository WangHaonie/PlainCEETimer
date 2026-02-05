using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules;

public class FileDialogHelper
{
    public static bool ShowDialog<TFileDialog>(string title, string fileName, out TFileDialog dialog, params FileFilter[] filters)
        where TFileDialog : FileDialog, new()
    {
        dialog = GetFileDialog<TFileDialog>(title, fileName, filters);
        var wrapper = new FileDialogWrapper(title);
        wrapper.Show();
        var result = dialog.ShowDialog(wrapper).AsBool();
        wrapper.Close();
        return result;
    }

    public static bool ShowDialog<TFileDialog>(string title, string fileName, IWin32Window parent, out TFileDialog dialog, params FileFilter[] filters)
        where TFileDialog : FileDialog, new()
    {
        dialog = GetFileDialog<TFileDialog>(title, fileName, filters);
        return dialog.ShowDialog(parent).AsBool();
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
}