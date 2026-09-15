using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;
using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.ViewModels;

namespace PlainCEETimer.WPF.Views;

public sealed partial class MainWindow : AppWindow
{
    protected override AppWindowStyle Params
        => AppWindowStyle.Special | AppWindowStyle.RoundCorner | AppWindowStyle.SuggestMaxWidth;

    private readonly MainViewModel vm;

    public MainWindow()
    {
        vm = ServiceHost.ServiceProvider.CreateViewModel<MainViewModel>()
            .Import<ICountdownService>((vm, s) => vm.CountdownService = s)
            .Import(MessageX, (vm, s) => vm.DialogService = s)
            .Import(new SystemBorderColorService(this), (vm, s) => vm.BorderColorService = s)
            .Import(new WPFWindowInitializer(this), (vm, s) => vm.WindowInitializer = s)
            .Import(new WPFWindowDragService(this), (vm, s) => vm.WindowDragService = s)
            .Import(new WPFWindowScreenChangeService(this), (vm, s) => vm.WindowScreenChangeService = s)
            .Import(new WPFWindowBounds(this), (vm, s) => vm.WindowBounds = s)
            .Import(new WPFWindowStyles(this), (vm, s) => vm.WindowStyles = s)
            .Import<ITrayIconLoader>((vm, s) => vm.TrayIconLoader = s)
            .Import(ScreenService, (vm, s) => vm.ScreenService = s)
            .Import(new WPFFontService(this), (vm, s) => vm.UnifiedFontService = s)
            .Build();

        DataContext = vm;
        InitializeComponent();
    }

    protected override bool OnClosing()
    {
        return !vm.CanClose();
    }

    protected override void OnClosed()
    {
        vm.Cleanup();
    }

    protected override void WndProc(ref Message m)
    {
        if (!vm.WndProc(ref m))
        {
            base.WndProc(ref m);
        }
    }
}
