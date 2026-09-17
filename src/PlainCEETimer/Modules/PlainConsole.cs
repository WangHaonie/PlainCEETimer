using System;
using System.Drawing;
using System.IO;
using Microsoft.Win32.SafeHandles;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public class PlainConsole
{
    public static PlainConsole Instance { get; } = new();

    private ConsoleColorString concolor;
    private bool RecordCursor;
    private uint CursorPosA;
    private uint CursorPosB;
    private readonly object syncLock = new();
    private static readonly bool _SupportAnsiColor;

    private static readonly IntPtr hIn;
    private static readonly IntPtr hOut;
    private static readonly IntPtr hErr;

    static PlainConsole()
    {
        Win32.PnAllocConsole(SystemVersion.BeforeNT10, out var phStdIn, out var phStdOut, out var phStdErr);
        EnsureConsole(phStdIn, phStdOut, phStdErr);
        hIn = phStdIn; hOut = phStdOut; hErr = phStdErr;
        _SupportAnsiColor = Win32.PnSetConsoleMode(phStdOut, PSCMF.STD_OUT | PSCMF.FLAGS_ENABLE | PSCMF.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
    }

    public PlainConsole Title(string title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            Win32.SetConsoleTitle(title);
        }

        return this;
    }

    public PlainConsole Write(string s)
    {
        lock (syncLock)
        {
            Console.Write(s);
            if (RecordCursor) CursorPosB = Win32.PnConsoleGetCursor(hOut);
            return this;
        }
    }

    public PlainConsole Write(string s, ConsoleColor color)
    {
        var tmp = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Write(s);
        Console.ForegroundColor = tmp;
        return this;
    }

    public PlainConsole WriteLine()
    {
        return Write("\r\n");
    }

    public PlainConsole WriteLine(string s)
    {
        Write(s);
        return WriteLine();
    }

    public PlainConsole WriteLine(string s, ConsoleColor color)
    {
        Write(s, color);
        return WriteLine();
    }

    public PlainConsole Write<T>(T obj)
    {
        return Write(obj.ToString());
    }

    public PlainConsole Write<T>(T obj, ConsoleColor color)
    {
        return Write(obj.ToString(), color);
    }

    public PlainConsole WriteLine<T>(T obj, ConsoleColor color)
    {
        return WriteLine(obj.ToString(), color);
    }

    public PlainConsole Color(ConsoleColor color)
    {
        Console.ForegroundColor = color;
        return this;
    }

    public PlainConsole Color(ConsoleColor fore, ConsoleColor back)
    {
        Console.BackgroundColor = back;
        return Color(fore);
    }

    public PlainConsole Color(Color fore, Color back)
    {
        if (_SupportAnsiColor)
        {
            concolor ??= new();
            concolor.ChangeColor(fore, back);
            Write(concolor.Value);
        }
        else
        {
            if (Win32.PnGetConsoleColor(hOut, fore, out var color))
                Console.ForegroundColor = color;
            if (Win32.PnGetConsoleColor(hOut, back, out color))
                Console.BackgroundColor = color;
        }

        return this;
    }

    public PlainConsole ResetColor()
    {
        if (_SupportAnsiColor) Write(ConsoleColorString.Reset);
        Console.ResetColor();
        return this;
    }

    public PlainConsole Timeout(int seconds)
    {
        CStd.system($"timeout {seconds} >nul");
        return this;
    }

    public PlainConsole Anchor()
    {
        RecordCursor = true;
        CursorPosA = Win32.PnConsoleGetCursor(hOut);
        return this;
    }

    public PlainConsole AnchorEnd()
    {
        RecordCursor = false;
        return this;
    }

    public PlainConsole Clear()
    {
        if (RecordCursor)
        {
            Win32.PnConsoleClear(hOut, CursorPosA, CursorPosB);
        }

        return this;
    }

    private static void EnsureConsole(IntPtr hIn, IntPtr hOut, IntPtr hErr)
    {
        Console.SetIn(new StreamReader(new FileStream(new SafeFileHandle(hIn, false), FileAccess.Read), Console.InputEncoding));
        Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(hOut, false), FileAccess.Write), Console.OutputEncoding) { AutoFlush = true });
        Console.SetError(new StreamWriter(new FileStream(new SafeFileHandle(hErr, false), FileAccess.Write), Console.OutputEncoding) { AutoFlush = true });
    }

    ~PlainConsole()
    {
        concolor.Destroy();
    }
}