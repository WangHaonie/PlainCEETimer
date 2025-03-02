using PlainCEETimer.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.WPF.Base;
using System;

namespace PlainCEETimer.WPF.ViewModels
{
    public sealed class AboutViewModel : ViewModelBase
    {
        public bool TopMost
        {
            get => field;
            set => SetField(ref field, value);
        }

        public string AppVersion => $"v{App.AppVersion} {App.AppBuildDate}";

        public string AppLicensing => "Licensed under the GNU GPL, v3.";

        public string AppCopyright => App.CopyrightInfo;

        public AboutViewModel()
        {
            App.UniTopMostStateChanged += App_UniTopMostStateChanged;
            App_UniTopMostStateChanged(null, EventArgs.Empty);
        }

        private void App_UniTopMostStateChanged(object sender, EventArgs e)
        {
            TopMost = MainForm.UniTopMost;
        }
    }
}
