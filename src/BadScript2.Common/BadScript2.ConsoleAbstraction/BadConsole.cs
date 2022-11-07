using System;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations;

namespace BadScript2.ConsoleAbstraction;

public static class BadConsole
{
    private static IBadConsole s_Console;

    static BadConsole()
    {
        s_Console = new SystemConsole();
    }


    public static ConsoleColor ForegroundColor
    {
        get => s_Console.ForegroundColor;
        set => s_Console.ForegroundColor = value;
    }

    public static ConsoleColor BackgroundColor
    {
        get => s_Console.BackgroundColor;
        set => s_Console.BackgroundColor = value;
    }

    public static IBadConsole GetConsole()
    {
        return s_Console;
    }

    public static void SetConsole(IBadConsole console)
    {
        s_Console = console;
    }

    public static void Write(string str)
    {
        s_Console.Write(str);
    }

    public static void WriteLine(string str)
    {
        s_Console.WriteLine(str);
    }

    public static string ReadLine()
    {
        return s_Console.ReadLine();
    }

    public static Task<string> ReadLineAsync()
    {
        return s_Console.ReadLineAsync();
    }

    public static void Clear()
    {
        s_Console.Clear();
    }
}