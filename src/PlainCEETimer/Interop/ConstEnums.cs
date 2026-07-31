using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
[CompilerRemove]
public static class MONITOR_DPI_TYPE
{
    public const int EFFECTIVE = 0;
    public const int ANGULAR = 1;
    public const int RAW = 2;
    public const int DEFAULT = 0;
}

[NoConstants]
[CompilerRemove]
public static class MenuFlag
{
    public const int ByCommand = 0x0000;
    public const int ByPosition = 0x0400;
    public const int Bitmap = 0x0004;
    public const int Checked = 0x0008;
    public const int Disabled = 0x0002;
    public const int Enabled = 0x0000;
    public const int Grayed = 0x0001;
    public const int MenuBarBreak = 0x0020;
    public const int MenuBreak = 0x0040;
    public const int OwnerDraw = 0x0100;
    public const int Popup = 0x0010;
    public const int Separator = 0x0800;
    public const int String = 0x0000;
    public const int Unchecked = 0x0000;
}

[NoConstants]
[CompilerRemove]
public static class TrackPopupMenu
{
    public const int LeftAlign = 0x0000;
    public const int RightAlign = 0x0008;
    public const int HorizontalCenterAlign = 0x0004;
    public const int BottomAlign = 0x0020;
    public const int TopAlign = 0x0000;
    public const int VerticalCenterAlign = 0x0010;
    public const int NoNotify = 0x0080;
    public const int ReturnCmd = 0x0100;
    public const int LeftButton = 0x0000;
    public const int RightButton = 0x0002;
    public const int Horizontal = 0x0000;
    public const int Vertical = 0x0040;
    public const int Default = LeftAlign | TopAlign | RightButton | Vertical;
}

[NoConstants]
[CompilerRemove]
public static class ShowWindowCommand
{
    public const int Normal = 1;
    public const int Maximize = 3;
    public const int NoActivate = 4;
    public const int Minimize = 7;
}

[NoConstants]
[CompilerRemove]
public static class RDW
{
    public const uint Common = Invalidate | Erase | UpdateNow;

    public const uint Invalidate = 0x0001;
    public const uint InternalPaint = 0x0002;
    public const uint Erase = 0x0004;
    public const uint Validate = 0x0008;
    public const uint NoInternalPaint = 0x0010;
    public const uint NoErase = 0x0020;
    public const uint NoChildren = 0x0040;
    public const uint AllChildren = 0x0080;
    public const uint UpdateNow = 0x0100;
    public const uint EraseNow = 0x0200;
    public const uint Frame = 0x0400;
    public const uint NoFrame = 0x0800;
}
