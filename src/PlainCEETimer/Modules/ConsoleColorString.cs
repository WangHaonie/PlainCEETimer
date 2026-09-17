using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

[NoConstants]
public unsafe class ConsoleColorString : IDisposable
{
    public string Value
    {
        get
        {
            int* data = stackalloc int[3];
            data[1] = m_fore;
            data[2] = m_back;

            if (Win32.PnBuildAnsiColorString(m_pstr, BufferLength, data))
            {
                return new(m_pstr, 0, *data);
            }

            return string.Empty;
        }
    }

    private char* m_pstr;
    private int m_fore;
    private int m_back;
    private readonly char[] m_value;
    private readonly GCHandle m_gchstr;
    private readonly ArrayCache<char> m_cache;

    public const string Reset = "\x1b[0m";

    private const int BufferLength = 37;

    public ConsoleColorString()
    {
        m_cache = new(BufferLength, out m_value);
        m_gchstr = GCHandle.Alloc(m_value, GCHandleType.Pinned);
        m_pstr = (char*)m_gchstr.AddrOfPinnedObject();
    }

    public void ChangeColor(Color fore, Color back)
    {
        m_fore = (COLORREF)fore;
        m_back = (COLORREF)back;
    }

    public void Dispose()
    {
        if (m_pstr != null)
        {
            m_gchstr.Free();
            m_cache.Destroy();
            GC.SuppressFinalize(this);
            m_pstr = null;
        }
    }

    ~ConsoleColorString()
    {
        Dispose();
    }
}
