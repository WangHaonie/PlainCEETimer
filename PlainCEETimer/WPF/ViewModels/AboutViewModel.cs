using PlainCEETimer.Modules;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PlainCEETimer.WPF.ViewModels
{
    public sealed class AboutViewModel
    {
        [Reactive]
        public BitmapSource Logo { get; set; } = Imaging.CreateBitmapSourceFromHIcon(App.AppIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
}
