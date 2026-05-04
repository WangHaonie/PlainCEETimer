using System.Windows.Forms;
using System.Windows.Threading;
using PlainCEETimer.Countdown;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;
using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.ViewModels;

namespace PlainCEETimer.WPF.Views;

public sealed partial class MainWindow : AppWindow
{
    protected override AppWindowStyle Params => AppWindowStyle.Special | AppWindowStyle.RoundCorner;

    private readonly MainViewModel vm;

    public MainWindow()
    {
        vm = new(new()
        {
            CountdownService = new DefaultCountdownService(new DispatcherSynchronizationContext()),
            DialogService = MessageX,
            BorderColorService = new SystemBorderColorService(this),
            WindowInitializer = new WPFWindowInitializer(this),
            WindowDragService = new WPFWindowDragService(this),
            WindowBounds = new WPFWindowBounds(this),
            WindowMessageService = new WindowMessageService(base.WndProc),
            WindowStyles = new WPFWindowStyles(this),
            TrayIconLoader = new AppTrayIconLoader(),
            ScreenService = ScreenService,
            UnifiedFontService = new WPFFontService(this)
        });

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
        vm.WndProc(ref m);
    }
}
