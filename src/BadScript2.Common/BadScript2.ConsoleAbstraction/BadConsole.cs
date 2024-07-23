using System;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations;
namespace BadScript2.ConsoleAbstraction;

/// <summary>
///     Wrapper class for the console abstraction
/// </summary>
public static class BadConsole
{
    public static readonly IBadConsole DummyConsole = new BadDummyConsole();

    /// <summary>
    ///     The Console Implementation that is used.
    /// </summary>
    private static IBadConsole s_Console;

    /// <summary>
    ///     Static Constructor
    /// </summary>
    static BadConsole()
    {
        s_Console = new BadSystemConsole();
    }


    /// <summary>
    ///     The Foreground Color
    /// </summary>
    public static ConsoleColor ForegroundColor
    {
        get => s_Console.ForegroundColor;
        set => s_Console.ForegroundColor = value;
    }

    /// <summary>
    ///     The Background Color
    /// </summary>
    public static ConsoleColor BackgroundColor
    {
        get => s_Console.BackgroundColor;
        set => s_Console.BackgroundColor = value;
    }

    /// <summary>
    ///     Returns the Current Console Implementation
    /// </summary>
    /// <returns>Console Implementation</returns>
    public static IBadConsole GetConsole()
    {
        return s_Console;
    }

    /// <summary>
    ///     Sets the Current Console Implementation
    /// </summary>
    /// <param name="console">The new Console Implementation that will be used.</param>
    public static void SetConsole(IBadConsole console)
    {
        s_Console = console;
    }

    /// <summary>
    ///     Writes a string to the console
    /// </summary>
    /// <param name="str">The String to be written</param>
    public static void Write(string str)
    {
        s_Console.Write(str);
    }

    /// <summary>
    ///     Writes a string to the console and appends a newline
    /// </summary>
    /// <param name="str">The String to be written</param>
    public static void WriteLine(string str)
    {
        s_Console.WriteLine(str);
    }

    /// <summary>
    ///     Reads a line from the console
    /// </summary>
    /// <returns>The line that was read</returns>
    public static string ReadLine()
    {
        return s_Console.ReadLine();
    }

    /// <summary>
    ///     Reads a line from the console asynchronously
    /// </summary>
    /// <returns>The line that was read</returns>
    public static Task<string> ReadLineAsync()
    {
        return s_Console.ReadLineAsync();
    }

    /// <summary>
    ///     Clears the console
    /// </summary>
    public static void Clear()
    {
        s_Console.Clear();
    }

    private class BadDummyConsole : IBadConsole
    {
        public ConsoleColor ForegroundColor { get; set; }

        public ConsoleColor BackgroundColor { get; set; }

        public void Write(string str) { }

        public void WriteLine(string str) { }

        public string ReadLine()
        {
            throw new NotSupportedException();
        }

        public Task<string> ReadLineAsync()
        {
            throw new NotSupportedException();
        }

        public void Clear() { }
    }
}