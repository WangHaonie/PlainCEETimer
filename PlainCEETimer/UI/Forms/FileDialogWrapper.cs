using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Forms
{
    internal sealed class FileDialogWrapper(string title) : AppForm
    {
        protected override void OnInitializing()
        {
            Text = nameof(FileDialogWrapper);
            TopMost = false;
            MinimizeBox = false;
            ControlBox = false;
            StartPosition = FormStartPosition.WindowsDefaultLocation;
            this.AddControls(b => [b.Label(title)]);
        }

        public static string CreateFilters(params FileFilter[] filters)
        {
            return string.Join("|", filters);
        }

        public static DialogResult ShowDialog(FileDialog dialog)
        {
            var wrapper = new FileDialogWrapper(dialog.Title);
            wrapper.Show();
            var result = dialog.ShowDialog(wrapper);
            wrapper.Close();
            return result;
        }
    }
}
