using System;

namespace PlainCEETimer.Modules;

public class ConsoleHelper
{
    public static readonly ConsoleHelper Instance = new();

    public ConsoleHelper Write(string s)
    {
        Console.Write(s);
        return this;
    }

    public ConsoleHelper Write<T>(T obj)
    {
        return Write(obj.ToString());
    }

    public ConsoleHelper Write(string s, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Write(s);
        Console.ResetColor();
        return this;
    }

    public ConsoleHelper Write<T>(T obj, ConsoleColor color)
    {
        return Write(obj.ToString(), color);
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

    public ConsoleHelper WriteLine<T>(T obj, ConsoleColor color)
    {
        return WriteLine(obj.ToString(), color);
    }
}