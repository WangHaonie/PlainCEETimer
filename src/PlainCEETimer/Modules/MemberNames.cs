using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.Modules;

[NoConstants]
[CompilerRemove]
public static class MemberNames
{
    public const string Countdown = nameof(Countdown);
    public const string MessageX = nameof(MessageX);
    public const string Initializer = nameof(Initializer);
    public const string DragService = nameof(DragService);
    public const string ScreenChangeService = nameof(ScreenChangeService);
    public const string Bounds = nameof(Bounds);
    public const string TrayIcon = nameof(TrayIcon);
    public const string Styles = nameof(Styles);
    public const string Screen = nameof(Screen);
    public const string FontService = nameof(FontService);
    public const string WindowBorderColor = nameof(WindowBorderColor);
}
