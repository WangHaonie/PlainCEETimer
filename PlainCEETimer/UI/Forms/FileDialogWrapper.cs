using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Forms
{
    internal sealed class FileDialogWrapper : AppForm
    {
        protected override void OnInitializing()
        {
            Text = nameof(FileDialogWrapper);
            TopMost = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.WindowsDefaultLocation;
            this.AddControls(b => [b.Label("FileDialogWrapper - 高考倒计时")]);
        }

        public static string CreateFilters(params FileFilter[] filters)
        {
            return string.Join("|", filters);
        }

        public static DialogResult ShowDialog(FileDialog dialog)
        {
            var wrapper = new FileDialogWrapper();
            wrapper.Show();
            var result = dialog.ShowDialog(wrapper);
            wrapper.Close();
            return result;
        }
    }
}
