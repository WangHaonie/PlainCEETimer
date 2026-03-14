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
            CountdownService = new DefaultCountdownService(),
            DialogService = MessageX,
            BorderColorService = new WinFormsBorderColorService(this),
            WindowInitializer = new WinFormsWindowInitializer(this),
            WindowDragService = new WinFormsWindowDragService(this),
            WindowBounds = new WinFormsWindowBounds(this),
            WindowMessageService = new WindowMessageService(base.WndProc),
            WindowStyles = new WinFormsWindowStyles(this),
            TrayIconLoader = new AppTrayIconLoader(),
            ScreenService = new ScreenHelper(this),
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
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var prop = e.PropertyName;

        switch (prop.Length)
        {
            case 4:
                if (prop == MainViewModel.ModelPropName)
                {
                    var m = vm.CountdownModel;
                    var mi = m.BasicInfo;

                    if (mi != null)
                    {
                        CountdownForeColor = mi.ForeColor;
                        BackColor = mi.BackColor;
                        CountdownContent = mi.Content;
                        Size = TextRenderer.MeasureText(CountdownContent, CountdownFont, new(CountdownMaxWidth, 0), TextFormatFlags.WordBreak);
                        Invalidate();
                    }

                    CountdownFont = m.Font;
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
}