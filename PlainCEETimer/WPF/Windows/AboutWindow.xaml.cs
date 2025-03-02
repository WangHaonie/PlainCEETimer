using PlainCEETimer.Modules;
using PlainCEETimer.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PlainCEETimer.WPF.Windows
{
    public sealed partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            InitializeReactive();
        }

        private void InitializeReactive()
        {
            ViewModel = new AboutViewModel();

            this.WhenActivated(Disposables =>
            {
                this.OneWayBind(ViewModel, VM => VM.Logo, V => V.Icon).DisposeWith(Disposables);
            });
        }
    }
}