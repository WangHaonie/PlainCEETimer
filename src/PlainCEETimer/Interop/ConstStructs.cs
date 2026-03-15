using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
[CompilerRemove]
public static class NMHDR
{
    public const int hwndFrom = 0;
    public const int idFrom = 8;
    public const int code = 16;
}

[NoConstants]
[CompilerRemove]
public static class NMCUSTOMDRAW
{
    public const int hdr = 0;
    public const int dwDrawStage = 24;
    public const int hdc = 32;
    public const int rc = 40;
    public const int dwItemSpec = 56;
    public const int uItemState = 64;
    public const int lItemlParam = 72;
}

[NoConstants]
[CompilerRemove]
public static class HDLAYOUT
{
    public const int prc = 0;
    public const int pwpos = 8;
}

[NoConstants]
[CompilerRemove]
public static class WINDOWPOS
{
    public const int hwnd = 0;
    public const int hwndInsertAfter = 8;
    public const int x = 16;
    public const int y = 20;
    public const int cx = 24;
    public const int cy = 28;
    public const int flags = 32;
}

[NoConstants]
[CompilerRemove]
public static class CREATESTRUCT
{
    public const int lpCreateParams = 0;
    public const int hInstance = 8;
    public const int hMenu = 16;
    public const int hwndParent = 24;
    public const int cy = 32;
    public const int cx = 36;
    public const int y = 40;
    public const int x = 44;
    public const int style = 48;
    public const int lpszName = 56;
    public const int lpszClass = 64;
    public const int dwExStyle = 72;
}