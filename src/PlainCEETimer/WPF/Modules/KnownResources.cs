using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.WPF.Modules;

[NoConstants]
[CompilerRemove]
public static class KnownResources
{
    public const string WindowBorderCornerRadius = nameof(WindowBorderCornerRadius);

    public const string AppFontFamily = nameof(AppFontFamily);
}
