namespace PlainCEETimer.UI.Core;

public interface IMainServiceHub : IRequireCountdown, IRequireWindowStyles, IRequireDialog, IRequireWindowInitializer
{
    IWindowDragService WindowDragService { get; set; }

    IWindowScreenChangeService WindowScreenChangeService { get; set; }

    IWindowBounds WindowBounds { get; set; }

    ITrayIconLoader TrayIconLoader { get; set; }

    IScreenService ScreenService { get; set; }

    IUnifiedFontService UnifiedFontService { get; set; }

    IBorderColorService BorderColorService { get; set; }
}
