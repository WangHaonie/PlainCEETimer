using System;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules;

public class ConsoleHelper
{
    public static ConsoleHelper Instance { get; } = new();

    private readonly object syncLock = new();

    static ConsoleHelper()
    {
        Win32.AllocConsole();
    }

    public ConsoleHelper Write(string s)
    {
        lock (syncLock)
        {
            Console.Write(s);
            return this;
        }
    }

    public ConsoleHelper Write(string s, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Write(s);
        Console.ResetColor();
        return this;
    }

    public ConsoleHelper WriteLine()
    {
        return Write("\r\n");
    }

    public ConsoleHelper WriteLine(string s)
    {
        Write(s);
        return WriteLine();
    }

    public ConsoleHelper WriteLine(string s, ConsoleColor color)
    {
        Write(s, color);
        return WriteLine();
    }

    public ConsoleHelper Write<T>(T obj)
    {
        return Write(obj.ToString());
    }

    public ConsoleHelper Write<T>(T obj, ConsoleColor color)
    {
        return Write(obj.ToString(), color);
    }

    public ConsoleHelper WriteLine<T>(T obj, ConsoleColor color)
    {
        return WriteLine(obj.ToString(), color);
    }

    public ConsoleHelper Color(ConsoleColor color)
    {
        Console.ForegroundColor = color;
        return this;
    }

    public ConsoleHelper Color(ConsoleColor fore, ConsoleColor back)
    {
        Console.BackgroundColor = back;
        return Color(fore);
    }

    public ConsoleHelper ResetColor()
    {
        Console.ResetColor();
        return this;
    }

    public ConsoleHelper Timeout(int seconds)
    {
        CStd.system($"timeout {seconds} >nul");
        return this;
    }
}