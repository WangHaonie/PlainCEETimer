using PlainCEETimer.Countdown;

namespace PlainCEETimer.UI.Core;

public class MainServiceHub
{
    public required ICountdownService CountdownService { get; init; }

    public required IDialogService DialogService { get; init; }

    public required IWindowInitializer WindowInitializer { get; init; }

    public required IWindowDragService WindowDragService { get; init; }

    public required IWindowBounds WindowBounds { get; init; }

    public required IWindowMessageService WindowMessageService { get; init; }

    public required IWindowStyles WindowStyles { get; init; }

    public required ITrayIconLoader TrayIconLoader { get; init; }

    public required IScreenService ScreenService { get; init; }

    public required IUnifiedFontService UnifiedFontService { get; init; }

    public IBorderColorService BorderColorService { get; init; }
}