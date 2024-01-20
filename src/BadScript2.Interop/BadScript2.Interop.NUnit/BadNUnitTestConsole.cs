using BadScript2.ConsoleAbstraction;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Implements a wrapper for the default NUnit console
/// </summary>
public class BadNUnitTestConsole : IBadConsole
{
    public void Write(string str)
    {
        TestContext.Write(str);
    }

    public void WriteLine(string str)
    {
        TestContext.WriteLine(str);
    }

    public string ReadLine()
    {
        throw new NotSupportedException();
    }

    public Task<string> ReadLineAsync()
    {
        return Task.Run(() => Console.ReadLine() ?? "");
    }

    public void Clear()
    {
        //Do nothing
    }

    public ConsoleColor ForegroundColor { get; set; }

    public ConsoleColor BackgroundColor { get; set; }
}