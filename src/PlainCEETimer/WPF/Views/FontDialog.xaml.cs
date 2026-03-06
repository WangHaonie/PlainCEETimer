using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.Models;
using PlainCEETimer.WPF.ViewModels;

namespace PlainCEETimer.WPF.Views
{
    /// <summary>
    /// Interaction logic for FontDialog.xaml
    /// </summary>
    public partial class FontDialog : AppWindow
    {
        public FontModel Font { get; private set; }

        private readonly FontDialogViewModel vm;

        public FontDialog(FontModel font = null)
        {
            InitializeComponent();
            vm = new FontDialogViewModel(font, MessageX);
            vm.ParseResult += fnt => Font = fnt;
            DataContext = vm;
        }

        protected override bool OnClosing()
        {
            return !vm.CanClose();
        }
    }
}
