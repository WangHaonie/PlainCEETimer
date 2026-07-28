using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public class DeviceContext : IDeviceContext
{
    public bool IsValid => m_value != IntPtr.Zero;

    private readonly IntPtr m_hwnd;
    private readonly IntPtr m_value;

    private DeviceContext(IntPtr hWnd, IntPtr hDC)
    {
        m_hwnd = hWnd;
        m_value = hDC;
    }

    public IntPtr GetHdc()
    {
        return m_value;
    }

    public void ReleaseHdc()
    {
        Win32UI.ReleaseDC(m_hwnd, m_value);
    }

    public void Dispose()
    {
        ReleaseHdc();
        GC.SuppressFinalize(this);
    }

    public static DeviceContext CreateDC(IWin32Window window)
    {
        return CreateDC(window.Handle);
    }

    public static DeviceContext CreateDC(IntPtr hWnd)
    {
        return new(hWnd, Win32UI.GetDC(hWnd));
    }

    ~DeviceContext()
    {
        Dispose();
    }
}
