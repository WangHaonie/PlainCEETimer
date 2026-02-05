using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public class SystemMenu
{
    private sealed class ParentNativeWindow(IntPtr hWnd) : NativeWindow
    {
        private bool _assigned;
        private readonly List<NativeMenuItem> items = [];

        internal void Add(NativeMenuItem item)
        {
            if (!_assigned)
            {
                AssignHandle(hWnd);
                _assigned = true;
            }

            items.Add(item);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;

            if (m.Msg == WM_SYSCOMMAND)
            {
                var count = items.Count;

                for (int i = 0; i < count; i++)
                {
                    if (items[i].WmSysCommand(ref m))
                    {
                        m.Result = IntPtr.Zero;
                        return;
                    }
                }
            }

            base.WndProc(ref m);
        }
    }

    private class NativeMenuItem(IntPtr id, EventHandler onClick)
    {
        internal bool WmSysCommand(ref Message m)
        {
            if (m.WParam == id)
            {
                onClick(this, EventArgs.Empty);
                return true;
            }

            return false;
        }
    }

    private ParentNativeWindow pnw;
    private readonly IntPtr m_hmenu;
    private readonly IntPtr m_owner;
    private readonly RandomUID uids;

    public SystemMenu(IntPtr hMenu, IntPtr hOwner)
    {
        if (Win32UI.IsMenu(hMenu))
        {
            m_hmenu = hMenu;
            m_owner = hOwner;
            uids = new(1000, 0xF000);
            return;
        }

        throw new InvalidOperationException();
    }

    public SystemMenu InsertItem(int index, string text, EventHandler onClick)
    {
        int id;

        if ((id = InsertMenu(index, MenuFlag.String, text)) > 0 && onClick != null)
        {
            pnw.Add(new(new(id), onClick));
        }

        return this;
    }

    public SystemMenu InsertSeparator(int index)
    {
        InsertMenu(index, MenuFlag.Separator, null);
        return this;
    }

    public static SystemMenu From(Form wnd)
    {
        var hwnd = wnd.Handle;
        var ncm = new SystemMenu(Win32UI.GetSystemMenu(hwnd, false), hwnd);
        return ncm;
    }

    private int InsertMenu(int index, MenuFlag flags, string text)
    {
        pnw ??= new(m_owner);

        if (index < -1)
        {
            index += Win32UI.GetMenuItemCount(m_hmenu) + 1;
        }

        var id = uids.Next();

        if (Win32UI.InsertMenu(m_hmenu, index, MenuFlag.ByPosition | flags, id, text))
        {
            return id;
        }

        uids.Remove(id);
        return 0;
    }
}