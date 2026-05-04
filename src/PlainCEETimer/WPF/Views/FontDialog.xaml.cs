using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.Models;
using PlainCEETimer.WPF.ViewModels;

namespace PlainCEETimer.WPF.Views;

public sealed partial class FontDialog : AppWindow
{
    public FontModel Font { get; private set; }

    private readonly FontDialogViewModel vm;

    public FontDialog(FontModel font = null)
    {
        vm = new(font, MessageX);

        vm.ParseResult += fnt =>
        {
            Font = fnt;
            DialogResult = fnt != null;
        };

        DataContext = vm;
        InitializeComponent();
    }

    protected override bool OnClosing()
    {
        return !vm.CanClose();
    }
}