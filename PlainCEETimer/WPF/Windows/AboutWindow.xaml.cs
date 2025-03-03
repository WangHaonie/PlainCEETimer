using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace PlainCEETimer.WPF.Windows
{
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            Closing += AppWindow_Closing;
        }

        private void ImageLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.CheckForUpdate();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AppWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !ViewModel.UpdateControlsEnabled;
        }
    }
}
