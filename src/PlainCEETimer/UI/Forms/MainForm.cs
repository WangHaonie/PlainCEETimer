using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Core;
using PlainCEETimer.WPF.ViewModels;

namespace PlainCEETimer.UI.Forms;

public sealed class MainForm : AppForm
{
    protected override AppWindowStyle Params => AppWindowStyle.Special | AppWindowStyle.RoundCorner;

    private MainViewModel vm;
    private Font CountdownFont;
    private Color CountdownForeColor;
    private string CountdownContent;
    private int CountdownMaxWidth;

    protected override void OnInitializing()
    {
        Text = "高考倒计时";

        vm = new(new()
        {
            CountdownService = new DefaultCountdownService(new WindowsFormsSynchronizationContext()),
            DialogService = MessageX,
            BorderColorService = new SystemBorderColorService(this),
            WindowInitializer = new WinFormsWindowInitializer(this),
            WindowDragService = new WinFormsWindowDragService(this),
            WindowScreenChangeService = new WinFormsWindowScreenChangeService(this),
            WindowBounds = new WinFormsWindowBounds(this),
            WindowStyles = new WinFormsWindowStyles(this),
            TrayIconLoader = new AppTrayIconLoader(),
            ScreenService = ScreenService,
            UnifiedFontService = new WinFormsFontService(this)
        });

        vm.PropertyChanged += OnPropertyChanged;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        TextRenderer.DrawText(g, CountdownContent, CountdownFont, ClientRectangle, CountdownForeColor, TextFormatFlags.Left | TextFormatFlags.WordBreak);
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        ApplyCountdownFont();
    }

    protected override bool OnClosing(CloseReason closeReason)
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
        base.WndProc(ref m);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var prop = e.PropertyName;

        switch (prop.Length)
        {
            case 4:
                if (prop == nameof(vm.Info))
                {
                    var i = vm.Info;

                    if (i != null)
                    {
                        CountdownForeColor = i.ForeColor;
                        BackColor = i.BackColor;
                        CountdownContent = i.Content;
                        Size = TextRenderer.MeasureText(CountdownContent, CountdownFont, new(CountdownMaxWidth, 0), TextFormatFlags.WordBreak);
                        Invalidate();
                    }
                }
                break;

            case 7:
                if (prop == nameof(vm.GdiFont))
                {
                    ApplyCountdownFont();
                }
                break;

            default:
                if (prop == nameof(MainViewModel.MaximumWidth))
                {
                    CountdownMaxWidth = (int)vm.MaximumWidth;
                }
                break;
        }
    }

    private void ApplyCountdownFont()
    {
        CountdownFont = ScaleFont(vm.GdiFont);
    }
}
