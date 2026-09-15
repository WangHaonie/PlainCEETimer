using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Core;
using PlainCEETimer.WPF.ViewModels;

namespace PlainCEETimer.UI.Forms;

public sealed class MainForm : AppForm
{
    protected override AppWindowStyle Params
        => AppWindowStyle.Special | AppWindowStyle.RoundCorner | AppWindowStyle.SuggestMaxWidth;

    private MainViewModel vm;
    private Font CountdownFont;
    private Color CountdownForeColor;
    private string CountdownContent;

    protected override void OnInitializing()
    {
        Text = "高考倒计时";
        vm = ServiceHost.ServiceProvider.CreateViewModel<MainViewModel>()
            .Import<ICountdownService>((vm, s) => vm.CountdownService = s)
            .Import(MessageX, (vm, s) => vm.DialogService = s)
            .Import(new SystemBorderColorService(this), (vm, s) => vm.BorderColorService = s)
            .Import(new WinFormsWindowInitializer(this), (vm, s) => vm.WindowInitializer = s)
            .Import(new WinFormsWindowDragService(this), (vm, s) => vm.WindowDragService = s)
            .Import(new WinFormsWindowScreenChangeService(this), (vm, s) => vm.WindowScreenChangeService = s)
            .Import(new WinFormsWindowBounds(this), (vm, s) => vm.WindowBounds = s)
            .Import(new WinFormsWindowStyles(this), (vm, s) => vm.WindowStyles = s)
            .Import<ITrayIconLoader>((vm, s) => vm.TrayIconLoader = s)
            .Import(ScreenService, (vm, s) => vm.ScreenService = s)
            .Import(new WinFormsFontService(this), (vm, s) => vm.UnifiedFontService = s)
            .Build();

        vm.PropertyChanged += OnPropertyChanged;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        TextRenderer.DrawText(g, CountdownContent, CountdownFont,
            ClientRectangle, CountdownForeColor, TextFormatFlags.Left | TextFormatFlags.WordBreak);
    }

    protected override void ScaleParamters(bool isHighDpi, float dpi, float dpiRatio, float dpiRatioRel)
    {
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
        if (!vm.WndProc(ref m))
        {
            base.WndProc(ref m);
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(vm.Info):

                var i = vm.Info;

                if (i != null)
                {
                    CountdownForeColor = i.ForeColor;
                    BackColor = i.BackColor;
                    CountdownContent = i.Content;
                    using var dc = DeviceContext.CreateDC(this);
                    Size = TextRenderer.MeasureText(dc, CountdownContent, CountdownFont,
                        new(SuggestedMaxWidth, 0), TextFormatFlags.WordBreak);
                    Invalidate();
                }

                break;

            case nameof(vm.GdiFont):
                ApplyCountdownFont();
                break;
        }
    }

    private void ApplyCountdownFont()
    {
        CountdownFont = ScaleFont(vm.GdiFont);
    }
}
