using PlainCEETimer.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.WPF.Base;
using System;
using System.Windows;
using System.Windows.Interop;

namespace PlainCEETimer.WPF.Controls
{
    public class AppWindow<TViewModel> : Window where TViewModel : ViewModelBase, new()
    {
        protected TViewModel ViewModel { get; set; } = new TViewModel();

        public bool IsClosed { get; private set; }

        public IntPtr Handle
        {
            get
            {
                if (field == IntPtr.Zero)
                {
                    field = new WindowInteropHelper(this).Handle;
                }

                return field;
            }
        } = IntPtr.Zero;

        public AppWindow()
        {
            DataContext = ViewModel;
            FontFamily = new("Segoe UI, Microsoft YaHei");
            SetUniTopMost(null, null);
            App.UniTopMostStateChanged += SetUniTopMost;
        }

        public void ReActivate()
        {
            var tmp = Topmost;
            Topmost = true;
            WindowState = WindowState.Normal;
            Show();
            Activate();
            Topmost = tmp;
        }

        private void SetUniTopMost(object sender, EventArgs e)
        {
            Topmost = MainForm.UniTopMost;
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;
            App.UniTopMostStateChanged -= SetUniTopMost;
            base.OnClosed(e);
        }
    }
}
