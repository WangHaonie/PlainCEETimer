using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
[CompilerRemove]
public static class WinUser
{
    public const int HCBT_CREATEWND = 3;
    public const int HCBT_DESTROYWND = 4;

    public const int HTCAPTION = 2;

    public const int HWND_TOP = 0;
    public const int HWND_TOPMOST = -1;
    public const int HWND_MESSAGE = -3;

    public const int UIS_SET = 1;
    public const int UISF_HIDEFOCUS = 0x1;

    public const int SC_SIZE = 0xF000;
    public const int SC_MOVE = 0xF010;
    public const int SC_MINIMIZE = 0xF020;
    public const int SC_MAXIMIZE = 0xF030;
    public const int SC_CLOSE = 0xF060;
    public const int SC_RESTORE = 0xF120;

    public const int EN_CHANGE = 0x0300;
    public const int EM_SETMARGINS = 0x00D3;
    public const int EC_RIGHTMARGIN = 0x0002;

    public const int MOD_NOREPEAT = 0x4000;

    public const int PM_REMOVE = 0x0001;

    public const int SM_CXEDGE = 45;
    public const int SM_CYEDGE = 46;

    public const int WM_NULL = 0;
    public const int WM_FIRST = (int)(0U - 0U);
    public const int WM_CREATE = 0x0001;
    public const int WM_HOTKEY = 0x0312;
    public const int WM_CLOSE = 0x0010;
    public const int WM_MOVE = 0x0003;
    public const int WM_SIZE = 0x0005;
    public const int WM_SETFOCUS = 0x0007;
    public const int WM_SETREDRAW = 0x000B;
    public const int WM_ACTIVATE = 0x0006;
    public const int WM_WINDOWPOSCHANGING = 0x0046;
    public const int WM_CONTEXTMENU = 0x007B;
    public const int WM_COMMAND = 0x0111;
    public const int WM_PASTE = 0x0302;
    public const int WM_REFLECT = 0x2000;
    public const int WM_PARENTNOTIFY = 0x0210;
    public const int WM_SYSCOMMAND = 0x0112;
    public const int WM_SETTINGCHANGE = 0x001A;
    public const int WM_SYSCOLORCHANGE = 0x0015;
    public const int WM_THEMECHANGED = 0x031A;
    public const int WM_CHANGEUISTATE = 0x0127;
    public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
    public const int WM_DPICHANGED = 0x02E0;
    public const int WM_DPICHANGED_BEFOREPARENT = 0x02E2;
    public const int WM_SETCURSOR = 0x0020;
    public const int WM_ERASEBKGND = 0x0014;
    public const int WM_PAINT = 0x000F;
    public const int WM_KEYDOWN = 0x0100;
    public const int WM_NOTIFY = 0x004E;
    public const int WM_USER = 0x0400;
    public const int WM_DESTROY = 0x0002;
    public const int WM_INITDIALOG = 0x0110;
    public const int WM_TIMER = 0x0113;
    public const int WM_CTLCOLORDLG = 0x0136;
    public const int WM_CTLCOLOREDIT = 0x0133;
    public const int WM_CTLCOLORSTATIC = 0x0138;
    public const int WM_CTLCOLORLISTBOX = 0x0134;
    public const int WM_CTLCOLORBTN = 0x0135;
    public const int WM_GETFONT = 0x0031;
    public const int WM_WINDOWPOSCHANGED = 0x0047;
    public const int WM_MOUSEMOVE = 0x0200;
    public const int WM_MOUSEWHEEL = 0x020A;
    public const int WM_LBUTTONDOWN = 0x0201;
    public const int WM_LBUTTONUP = 0x0202;
    public const int WM_LBUTTONDBLCLK = 0x0203;
    public const int WM_NCLBUTTONDOWN = 0x00A1;
    public const int WM_NCRBUTTONDOWN = 0x00A4;
    public const int WM_NCMBUTTONDOWN = 0x00A7;
    public const int WM_NCXBUTTONDOWN = 0x00AB;
    public const int WM_NCLBUTTONDBLCLK = 0x00A3;
    public const int WM_NCRBUTTONDBLCLK = 0x00A6;
    public const int WM_NCMBUTTONDBLCLK = 0x00A9;
    public const int WM_NCXBUTTONDBLCLK = 0x00AD;
    public const int WM_MOVING = 0x0216;
    public const int WM_SIZING = 0x0214;

    public const int WS_CHILD = 0x40000000;
    public const int WS_MINIMIZE = 0x20000000;
    public const int WS_BORDER = 0x00800000;
    public const int WS_EX_TOPMOST = 0x00000008;
    public const int WS_EX_CLIENTEDGE = 0x00000200;
    public const int WS_EX_COMPOSITED = 0x02000000;

    public const int GA_ROOT = 2;

    public const int GWL_STYLE = -16;

    public const int WINEVENT_OUTOFCONTEXT = 0x0000;

    public const int OBJID_WINDOW = 0x00000000;

    public const int CHILDID_SELF = 0;

    public const int EVENT_SYSTEM_FOREGROUND = 0x0003;
    public const int EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

    public const int SWP_NOACTIVATE = 0x0010;
}

[NoConstants]
[CompilerRemove]
public static class CommCtrl
{
    public const int CDRF_DODEFAULT = 0x00000000;
    public const int CDRF_NOTIFYITEMDRAW = 0x00000020;

    public const int CDDS_PREPAINT = 0x00000001;
    public const int CDDS_ITEM = 0x00010000;
    public const int CDDS_ITEMPREPAINT = CDDS_ITEM | CDDS_PREPAINT;

    public const int NM_FIRST = (int)(0U - 0U);
    public const int NM_CLICK = NM_FIRST - 2;
    public const int NM_DBLCLK = NM_FIRST - 3;
    public const int NM_CUSTOMDRAW = NM_FIRST - 12;

    public const int BS_SPLITBUTTON = 0x0000000C;
    public const int BCN_FIRST = unchecked((int)(0U - 1250U));
    public const int BCN_DROPDOWN = BCN_FIRST + 0x0002;

    public const int HDM_FIRST = 0x1200;
    public const int HDM_LAYOUT = HDM_FIRST + 5;

    public const int HKM_SETHOTKEY = WinUser.WM_USER + 1;
    public const int HKM_GETHOTKEY = WinUser.WM_USER + 2;
    public const int HKM_SETRULES = WinUser.WM_USER + 3;
    public const int HKCOMB_NONE = 0x0001;
    public const int HKCOMB_S = 0x0002;

    public const int PBM_SETSTATE = WinUser.WM_USER + 16;
    public const int PBST_NORMAL = 0x0001;
    public const int PBST_ERROR = 0x0002;
    public const int PBST_PAUSED = 0x0003;

    public const int TTN_FIRST = unchecked((int)(0U - 520U));
    public const int TTN_GETDISPINFOW = TTN_FIRST - 10;
    public const int TTM_SETMAXTIPWIDTH = WinUser.WM_USER + 24;

    public const int LVM_FIRST = 0x1000;
    public const int LVM_GETHEADER = LVM_FIRST + 31;
    public const int LVM_GETTOOLTIPS = LVM_FIRST + 78;
    public const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
    public const int LVS_EX_CHECKBOXES = 0x00000004;

    public const int DTM_FIRST = 0x1000;
    public const int DTM_GETMONTHCAL = DTM_FIRST + 8;
}

[NoConstants]
[CompilerRemove]
public static class BOOL
{
    public const int FALSE = 0;
    public const int TRUE = 1;
}

[NoConstants]
[CompilerRemove]
public static class Dlgs
{
    public const int grp2 = 0x0431;
}

[NoConstants]
[CompilerRemove]
public static class WinGdi
{
    public const int TRANSPARENT = 1;

    public const int LOGPIXELSX = 88;
}

[NoConstants]
[CompilerRemove]
public static class WinNls
{
    public const int MUI_LANGUAGE_NAME = 0x8;
}

[NoConstants]
[CompilerRemove]
public static class WinError
{
    public const int ERROR_CANCELLED = 1223;
}

[NoConstants]
[CompilerRemove]
public static class Natives
{
    public const int ASBT_AUTO = 0;
    public const int ASBT_NONE = 1;
    public const int ASBT_ACRYLIC = 3;
    public const int ASBT_MICA = 2;
    public const int ASBT_MICAALT = 4;
    public const int ASBT_AERO = 5;
}
