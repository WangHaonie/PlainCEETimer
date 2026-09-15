using PlainCEETimer.Countdown;

namespace PlainCEETimer.UI.Core;

public interface IMainServiceHub
{
    ICountdownService CountdownService { get; set; }

    IDialogService DialogService { get; set; }

    IWindowInitializer WindowInitializer { get; set; }

    IWindowDragService WindowDragService { get; set; }

    IWindowScreenChangeService WindowScreenChangeService { get; set; }

    IWindowBounds WindowBounds { get; set; }

    IWindowStyles WindowStyles { get; set; }

    ITrayIconLoader TrayIconLoader { get; set; }

    IScreenService ScreenService { get; set; }

    IUnifiedFontService UnifiedFontService { get; set; }

    IBorderColorService BorderColorService { get; set; }
}
