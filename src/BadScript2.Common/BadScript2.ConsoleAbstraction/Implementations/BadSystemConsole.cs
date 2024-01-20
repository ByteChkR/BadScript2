using System;
using System.Threading.Tasks;

/// <summary>
/// Contains Implementation of the Console Abstractions
/// </summary>
namespace BadScript2.ConsoleAbstraction.Implementations;

/// <summary>
///     Implements a wrapper for the default system console
/// </summary>
public class BadSystemConsole : IBadConsole
{
    /// <inheritdoc />
    public void Write(string str)
    {
        Console.Write(str);
    }

    /// <inheritdoc />
    public void WriteLine(string str)
    {
        Console.WriteLine(str);
    }

    /// <inheritdoc />
    public string ReadLine()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    /// <inheritdoc />
    public Task<string> ReadLineAsync()
    {
        return Task.Run(Console.ReadLine);
    }

    /// <inheritdoc />
    public void Clear()
    {
        Console.Clear();
    }

    /// <inheritdoc />
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    /// <inheritdoc />
    public ConsoleColor BackgroundColor
    {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }
}