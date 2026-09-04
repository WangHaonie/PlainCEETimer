using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public abstract class PlainCommonDialog : CommonDialog, IThemeAware
{
    private sealed class ColorDlgNativeWindow : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WinUser.WM_MOUSEMOVE:
                case WinUser.WM_LBUTTONDOWN:
                case WinUser.WM_LBUTTONUP:
                case WinUser.WM_LBUTTONDBLCLK:
                case WinUser.WM_COMMAND:
                case WinUser.WM_PAINT:
                    Win32UI.PnHookSysColorBrush();
                    base.WndProc(ref m);
                    Win32UI.PnUnhookSysColorBrush();
                    return;
            }

            base.WndProc(ref m);
        }
    }

    private sealed class FontDlgNativeWindow : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WinUser.WM_PAINT:
                    Win32UI.PnHookSysColor();
                    base.WndProc(ref m);
                    Win32UI.PnUnhookSysColor();
                    return;
            }

            base.WndProc(ref m);
        }
    }

    private sealed class GroupBoxNativeWindow : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WinUser.WM_PAINT:
                    Win32UI.PnHookThemedPaint();
                    base.WndProc(ref m);
                    Win32UI.PnUnhookThemedPaint();
                    return;
            }

            base.WndProc(ref m);
        }
    }

    private sealed class ColorDlgStaticColorCurrentNativeWindow : NativeWindow
    {
        internal IntPtr m_hParent;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WinUser.WM_NCHITTEST:
                    m.Result = (nint)WinUser.HTCLIENT;
                    return;

                case WinUser.WM_SETCURSOR:

                    if (m.LParam.LoWord == WinUser.HTCLIENT)
                    {
                        Cursor.Current = Cursors.Help;
                        m.Result = (nint)1;
                        return;
                    }

                    break;

                case WinUser.WM_RBUTTONDOWN:
                case WinUser.WM_LBUTTONDBLCLK:
                    SetExistingColor(m_hParent);
                    m.Result = IntPtr.Zero;
                    return;
            }

            base.WndProc(ref m);
        }

        private static void SetExistingColor(IntPtr hDlg)
        {
            if (Clipboard.ContainsText())
            {
                var color = String2Color(Clipboard.GetText());

                if (!color.IsEmpty)
                {
                    const int COLOR_RED = 706;
                    const int COLOR_GREEN = 707;
                    const int COLOR_BLUE = 708;

                    Win32UI.SetDlgItemInt(hDlg, COLOR_RED, color.R, true);
                    Win32UI.SetDlgItemInt(hDlg, COLOR_GREEN, color.G, true);
                    Win32UI.SetDlgItemInt(hDlg, COLOR_BLUE, color.B, true);
                }
            }
        }

        private static Color String2Color(string s)
        {
            var result = Color.Empty;
            String2ColorCore(s, ref result);

            if (!result.IsEmpty)
            {
                goto ret;
            }

            if (!string.IsNullOrEmpty(s))
            {
                var start = -1;
                var end = -1;
                var length = s.Length;

                for (int i = 0; i < length; i++)
                {
                    if (char.IsDigit(s[i]))
                    {
                        start = i;
                        break;
                    }
                }

                for (int i = length - 1; i >= 0; i--)
                {
                    if (char.IsDigit(s[i]))
                    {
                        end = i;
                        break;
                    }
                }

                if (start != -1 && end != -1 && end >= start)
                {
                    s = s.Substring(start, end - start + 1);
                    String2ColorCore(s, ref result);
                    goto ret;
                }
            }

        ret:
            return result;
        }

        private static void String2ColorCore(string s, ref Color color)
        {
            try
            {
                color = ColorTranslator.FromHtml(s);
            }
            catch { }
        }
    }

    private class IAppWindowWrapper(IntPtr hWnd) : IAppWindow
    {
        public bool InvokeRequired => false;

        public IDialogService MessageX => new AppMessageBox(this);

        public ContextMenu ContextMenu { get; set; }

        public IntPtr Handle => hWnd;

        public object Invoke(Delegate method, params object[] args)
        {
            return method.DynamicInvoke(args);
        }

        public IAsyncResult BeginInvoke(Delegate method, params object[] args)
        {
            return Task.Run(() => method.DynamicInvoke(args));
        }

        public void ReActivate()
        {
            return;
        }

        void IThemeAware.UpdateTheme(bool useDark, bool init)
        {
            return;
        }
    }

    private bool UseDark;
    private IntPtr Handle;
    private IntPtr MsgBoxHandle;
    private HOOKPROC CBTHookProc;
    private ColorDlgNativeWindow cdnw;
    private FontDlgNativeWindow fdnw;
    private GroupBoxNativeWindow gpnw;
    private ColorDlgStaticColorCurrentNativeWindow cdsccnw;
    private ThemeHelper themeHelper;
    private readonly bool IsFont;
    private readonly bool IsColor;
    private readonly string Text;
    private readonly AppForm Owner;
    private readonly IntPtr hBrush = Win32UI.CreateSolidBrush(BackCrColor);
    private static FnMessageBoxW fnMessageBox;
    private static readonly COLORREF BackCrColor = Colors.DarkBackText;
    private static readonly COLORREF ForeCrColor = Colors.DarkForeText;

    protected PlainCommonDialog(AppForm owner, string dialogTitle)
    {
        Owner = owner;
        Text = dialogTitle;
        IsFont = this is PlainFontDialog;
        IsColor = this is PlainColorDialog;
    }

    public new bool? ShowDialog()
    {
        using (new DpiAwarenessContextScope(AppParams.EnableCommDlgPMv2
            ? DpiAwarenessContext.PerMonitorV2 : DpiAwarenessContext.System))
        {
            return ShowDialog(Owner).AsBoolean();
        }
    }

    protected abstract bool StartDialog(IntPtr hWndOwner);

    protected sealed override bool RunDialog(IntPtr hwndOwner)
    {
        try
        {
            return StartDialog(hwndOwner);
        }
        catch
        {
            return false;
        }
    }

    protected sealed override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        return msg switch
        {
            WinUser.WM_INITDIALOG
                => WmInitDialog(hWnd),
            WinUser.WM_CTLCOLORDLG
            or WinUser.WM_CTLCOLOREDIT
            or WinUser.WM_CTLCOLORSTATIC
            or WinUser.WM_CTLCOLORLISTBOX
            or WinUser.WM_CTLCOLORBTN
                => WmCtlColor(wparam),
            WinUser.WM_DESTROY
                => WmDestroy(),
            _
                => IntPtr.Zero,
        };
    }

    private IntPtr WmInitDialog(IntPtr hWnd)
    {
        Handle = hWnd;
        Owner.ReActivate();
        CBTHookProc = CbtHookProc;
        fnMessageBox ??= MessageBoxW;
        Win32UI.RegisterUnmanagedWindow(hWnd);
        const int HMBF_REPMSGBOX = 1;
        Win32UI.PnHookMessageBox(CBTHookProc, fnMessageBox, HMBF_REPMSGBOX);

        if (Text != null)
        {
            Win32UI.SetWindowText(hWnd, Text);
        }

        themeHelper ??= new(this);
        Win32UI.GetWindowRect(hWnd, out var rect);
        Win32UI.MakeCenter(rect, Owner.Bounds, out var r);
        Win32UI.MoveWindow(hWnd, r.X, r.Y, r.Width, r.Height, false);

        if (IsColor)
        {
            const int COLOR_CURRENT = 709;
            NativeWindowHelper.Attach(Win32UI.GetDlgItem(hWnd, COLOR_CURRENT), ref cdsccnw);
            cdsccnw.m_hParent = hWnd;
        }

        return new(1);
    }

    private IntPtr WmCtlColor(IntPtr hDC)
    {
        if (UseDark)
        {
            Win32UI.SetBkMode(hDC, WinGdi.TRANSPARENT);
            Win32UI.SetBkColor(hDC, BackCrColor);
            Win32UI.SetTextColor(hDC, ForeCrColor);
            return hBrush;
        }

        return IntPtr.Zero;
    }

    private IntPtr WmDestroy()
    {
        Win32UI.PnUnhookMessageBox();
        Win32UI.DeleteObject(hBrush);
        Win32UI.UnregisterUnmanagedWindow(Handle);
        themeHelper.Destroy();
        return IntPtr.Zero;
    }

    private SystemStyle GetNativeStyle(IntPtr hWnd, out bool up)
    {
        using var cn = Win32UI.GetClassName(hWnd).AsStringUni();

        if (cn == "ComboBox")
        {
            up = false;
            return UseDark ? SystemStyle.CfdDark : SystemStyle.Cfd;
        }

        if (cn == "Edit")
        {
            up = true;
            return UseDark ? SystemStyle.CfdDark : SystemStyle.Explorer;
        }

        up = true;
        return UseDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer;
    }

    private IntPtr CbtHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        switch (nCode)
        {
            case WinUser.HCBT_CREATEWND:
                var lpcs = Marshal.ReadIntPtr(lParam);

                if (Win32UI.IsDialog(lpcs))
                {
                    Win32UI.GetWindowRect(Marshal.ReadIntPtr(lpcs, CREATESTRUCT.hwndParent), out var lprc);

                    Win32UI.MakeCenter
                    (
                        new
                        (
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.x),
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.y),
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.cx),
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.cy)
                        ), lprc, out var r
                    );

                    Marshal.WriteInt32(lpcs, CREATESTRUCT.x, r.X);
                    Marshal.WriteInt32(lpcs, CREATESTRUCT.y, r.Y);
                    Win32UI.RegisterUnmanagedWindow(MsgBoxHandle = wParam);
                }

                break;
            case WinUser.HCBT_DESTROYWND:

                if (wParam == MsgBoxHandle)
                {
                    Win32UI.UnregisterUnmanagedWindow(wParam);
                }

                break;
        }

        return IntPtr.Zero;
    }

    private static int MessageBoxW(IntPtr hWnd, string lpText, string lpCaption, int uType)
    {
        var result = new IAppWindowWrapper(hWnd).MessageX.Popup(uType, lpText);
        return result;
    }

    public sealed override void Reset()
    {
        return;
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        if (!ThemeManager.IsDarkModeSupported)
        {
            return;
        }

        UseDark = useDark;
        var hWnd = Handle;
        ThemeManager.EnableDarkModeForWindow(hWnd, useDark);

        Win32UI.EnumChildWindows(hWnd, (child, _) =>
        {
            ThemeManager.EnableDarkModeForControl(child, GetNativeStyle(child, out var up), up);
            return true;
        }, IntPtr.Zero);

        if (IsFont)
        {
            if (useDark)
            {
                NativeWindowHelper.Attach(hWnd, ref fdnw);
            }
            else
            {
                NativeWindowHelper.Detach(fdnw);
            }

            IntPtr hCtrl;

            if ((hCtrl = Win32UI.GetDlgItem(hWnd, Dlgs.grp2)) != IntPtr.Zero)
            {
                if (useDark)
                {
                    if (ThemeManager.NewThemeAvailable)
                    {
                        ThemeManager.EnableDarkModeForControl(hCtrl, SystemStyle.DarkTheme);
                    }
                    else
                    {
                        NativeWindowHelper.Attach(hCtrl, ref gpnw);
                    }
                }
                else
                {
                    if (ThemeManager.NewThemeAvailable)
                    {
                        ThemeManager.EnableDarkModeForControl(hCtrl, SystemStyle.Explorer);
                    }
                    else
                    {
                        NativeWindowHelper.Detach(gpnw);
                        Win32UI.RedrawWindow(hCtrl, IntPtr.Zero, IntPtr.Zero, RDW.Common);
                    }
                }
            }
        }

        if (IsColor)
        {
            if (useDark)
            {
                NativeWindowHelper.Attach(hWnd, ref cdnw);
            }
            else
            {
                NativeWindowHelper.Detach(cdnw);
            }
        }

        if (!init)
        {
            Win32UI.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, RDW.Common);
        }
    }
}
