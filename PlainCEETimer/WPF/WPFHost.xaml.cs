using System.Windows;

namespace PlainCEETimer.WPF
{
    public partial class WPFHost
    {
        public WPFHost()
        {
            InitializeComponent();
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }
    }
}
