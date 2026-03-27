using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
[CompilerRemove]
public static class NMHDR
{
    /// <summary>
    /// <code>HWND <see cref="hwndFrom"/></code>
    /// </summary>
    public const int hwndFrom = 0;

    /// <summary>
    /// <code>UINT_PTR <see cref="idFrom"/></code>
    /// </summary>
    public const int idFrom = 8;

    /// <summary>
    /// <code>UINT <see cref="code"/></code>
    /// </summary>
    public const int code = 16;
}

[NoConstants]
[CompilerRemove]
public static class NMCUSTOMDRAW
{
    /// <summary>
    /// <code><see cref="NMHDR"/> <see cref="hdr"/></code>
    /// </summary>
    public const int hdr = 0;

    /// <summary>
    /// <code>DWORD <see cref="dwDrawStage"/></code>
    /// </summary>
    public const int dwDrawStage = 24;

    /// <summary>
    /// <code>HDC <see cref="hdc"/></code>
    /// </summary>
    public const int hdc = 32;

    /// <summary>
    /// <code><see cref="RECT"/> <see cref="rc"/></code>
    /// </summary>
    public const int rc = 40;

    /// <summary>
    /// <code>DWORD_PTR <see cref="dwItemSpec"/></code>
    /// this is control specific, but it's how to specify an item.
    /// valid only with CDDS_ITEM bit set
    /// </summary>
    public const int dwItemSpec = 56;

    /// <summary>
    /// <code>UINT <see cref="uItemState"/></code>
    /// </summary>
    public const int uItemState = 64;

    /// <summary>
    /// <code>LPARAM <see cref="lItemlParam"/></code>
    /// </summary>
    public const int lItemlParam = 72;
}

[NoConstants]
[CompilerRemove]
public static class HDLAYOUT
{
    /// <summary>
    /// <code><see cref="RECT"/>* <see cref="prc"/></code>
    /// </summary>
    public const int prc = 0;

    /// <summary>
    /// <code><see cref="WINDOWPOS"/>* <see cref="pwpos"/></code>
    /// </summary>
    public const int pwpos = 8;
}

[NoConstants]
[CompilerRemove]
public static class WINDOWPOS
{
    /// <summary>
    /// <code>HWND <see cref="hwnd"/></code>
    /// </summary>
    public const int hwnd = 0;

    /// <summary>
    /// <code>HWND <see cref="hwndInsertAfter"/></code>
    /// </summary>
    public const int hwndInsertAfter = 8;

    /// <summary>
    /// <code><see cref="int"/> <see cref="x"/></code>
    /// </summary>
    public const int x = 16;

    /// <summary>
    /// <code><see cref="int"/> <see cref="y"/></code>
    /// </summary>
    public const int y = 20;

    /// <summary>
    /// <code><see cref="int"/> <see cref="x"/></code>
    /// </summary>
    public const int cx = 24;

    /// <summary>
    /// <code><see cref="int"/> <see cref="cy"/></code>
    /// </summary>
    public const int cy = 28;

    /// <summary>
    /// <code>UINT <see cref="flags"/></code>
    /// </summary>
    public const int flags = 32;
}

[NoConstants]
[CompilerRemove]
public static class CREATESTRUCT
{
    /// <summary>
    /// <code>LPVOID <see cref="lpCreateParams"/></code>
    /// </summary>
    public const int lpCreateParams = 0;

    /// <summary>
    /// <code>HINSTANCE <see cref="hInstance"/></code>
    /// </summary>
    public const int hInstance = 8;

    /// <summary>
    /// <code>HMENU <see cref="hMenu"/></code>
    /// </summary>
    public const int hMenu = 16;

    /// <summary>
    /// <code>HWND <see cref="hwndParent"/></code>
    /// </summary>
    public const int hwndParent = 24;

    /// <summary>
    /// <code><see cref="int"/> <see cref="cy"/></code>
    /// </summary>
    public const int cy = 32;

    /// <summary>
    /// <code><see cref="int"/> <see cref="x"/></code>
    /// </summary>
    public const int cx = 36;

    /// <summary>
    /// <code><see cref="int"/> <see cref="y"/></code>
    /// </summary>
    public const int y = 40;

    /// <summary>
    /// <code><see cref="int"/> <see cref="x"/></code>
    /// </summary>
    public const int x = 44;

    /// <summary>
    /// <code>LONG <see cref="style"/></code>
    /// </summary>
    public const int style = 48;

    /// <summary>
    /// <code>LPCWSTR <see cref="lpszName"/></code>
    /// </summary>
    public const int lpszName = 56;

    /// <summary>
    /// <code>LPCWSTR <see cref="lpszClass"/></code>
    /// </summary>
    public const int lpszClass = 64;

    /// <summary>
    /// <code>DWORD <see cref="dwExStyle"/></code>
    /// </summary>
    public const int dwExStyle = 72;
}