using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
[CompilerRemove]
public static class NativeConstants
{
    public const int HWND_MESSAGE = -3;

    public const int UIS_SET = 1;
    public const int UISF_HIDEFOCUS = 0x1;

    public const int PBM_SETSTATE = 0x0410;
    public const int PBST_NORMAL = 1;
    public const int PBST_ERROR = 2;
    public const int PBST_PAUSED = 3;

    public const int NM_CUSTOMDRAW = WM.FIRST - 12;
    public const int CDDS_PREPAINT = 0x00000001;
    public const int CDRF_NOTIFYITEMDRAW = 0x00000020;
    public const int CDDS_ITEM = 0x00010000;
    public const int CDDS_ITEMPREPAINT = CDDS_ITEM | CDDS_PREPAINT;
    public const int CDRF_DODEFAULT = 0x00000000;

    public const int TTN_FIRST = unchecked((int)(0U - 520U));
    public const int TTN_GETDISPINFOW = TTN_FIRST - 10;
    public const int TTM_SETMAXTIPWIDTH = WM.USER + 24;

    public const int LVM_FIRST = 0x1000;
    public const int LVM_GETTOOLTIPS = LVM_FIRST + 78;
    public const int LVM_GETHEADER = LVM_FIRST + 31;

    public const int grp2 = 0x0431;

    public const int HCBT_CREATEWND = 3;
    public const int HCBT_DESTROYWND = 4;
    public const int TRANSPARENT = 1;

    public const int BCN_FIRST = unchecked((int)(0U - 1250U));
    public const int BCN_DROPDOWN = BCN_FIRST + 0x0002;
    public const int BS_SPLITBUTTON = 0x0000000C;

    public const int SC_MOVE = 0xF010;
    public const int HTCAPTION = 2;

    public const int HDM_FIRST = 0x1200;
    public const int HDM_LAYOUT = HDM_FIRST + 5;

    public const int EN_CHANGE = 0x0300;
    public const int EM_SETMARGINS = 0x00D3;
    public const int EC_RIGHTMARGIN = 0x0002;

    public const int HKM_SETHOTKEY = WM.USER + 1;
    public const int HKM_GETHOTKEY = WM.USER + 2;
    public const int HKM_SETRULES = WM.USER + 3;
    public const int HKCOMB_NONE = 0x0001;
    public const int HKCOMB_S = 0x0002;

    public const ushort MOD_NOREPEAT = 0x4000;
}

[NoConstants]
[CompilerRemove]
public static class WM
{
    public const int FIRST = (int)(0U - 0U);
    public const int CREATE = 0x0001;
    public const int HOTKEY = 0x0312;
    public const int CLOSE = 0x0010;
    public const int CONTEXTMENU = 0x007B;
    public const int COMMAND = 0x0111;
    public const int PASTE = 0x0302;
    public const int PARENTNOTIFY = 0x0210;
    public const int SYSCOMMAND = 0x0112;
    public const int CHANGEUISTATE = 0x0127;
    public const int DWMCOLORIZATIONCOLORCHANGED = 0x0320;
    public const int SETCURSOR = 0x0020;
    public const int ERASEBKGND = 0x0014;
    public const int PAINT = 0x000F;
    public const int KEYDOWN = 0x0100;
    public const int NOTIFY = 0x004E;
    public const int USER = 0x0400;
    public const int DESTROY = 0x0002;
    public const int INITDIALOG = 0x0110;
    public const int CTLCOLORDLG = 0x0136;
    public const int CTLCOLOREDIT = 0x0133;
    public const int CTLCOLORSTATIC = 0x0138;
    public const int CTLCOLORLISTBOX = 0x0134;
    public const int CTLCOLORBTN = 0x0135;
    public const int GETFONT = 0x0031;
}

[NoConstants]
[CompilerRemove]
public static class ERROR
{
    public const int CANCELLED = 1223;
}

[NoConstants]
[CompilerRemove]
public static class WS
{
    public const int EX_COMPOSITED = 0x02000000;
    public const int EX_CLIENTEDGE = 0x00000200;
    public const int BORDER = 0x00800000;
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
    public const int Minimize = 7;
}
