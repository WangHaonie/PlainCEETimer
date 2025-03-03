using PlainCEETimer.Modules;
using PlainCEETimer.WPF.Base;
using System.Threading.Tasks;

namespace PlainCEETimer.WPF.ViewModels
{
    public sealed class AboutViewModel : ViewModelBase
    {
        public bool UpdateControlsEnabled
        {
            get => field;
            private set => SetField(ref field, value);
        } = true;

        public string AppVersion
        {
            get => field;
            set => SetField(ref field, value);
        } = $"版本 v{App.AppVersion} x64 ({App.AppBuildDate})";

        public string AppLicensing => "Licensed under the GNU GPL, v3.";

        public string AppCopyright => App.CopyrightInfo;

        public void CheckForUpdate()
        {
            if (UpdateControlsEnabled)
            {
                var OriginalVersionString = AppVersion;
                UpdateControlsEnabled = false;
                AppVersion = "正在检查更新, 请稍候...";
                Task.Run(() => new Updater().CheckForUpdate(false, null)).ContinueWith(t =>
                {
                    UpdateControlsEnabled = true;
                    AppVersion = OriginalVersionString;
                });
            }
        }
    }
}
