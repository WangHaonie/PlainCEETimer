using System;

namespace PlainCEETimer.Modules;

public class ConsoleHelper
{
    public ConsoleHelper Write(string s)
    {
        Console.Write(s);
        return this;
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
        Write("\r\n");
        return this;
    }

    public ConsoleHelper WriteLine(string s)
    {
        Write(s);
        WriteLine();
        return this;
    }

    public ConsoleHelper WriteLine(string s, ConsoleColor color)
    {
        Write(s, color);
        WriteLine();
        return this;
    }
}