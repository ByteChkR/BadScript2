using BadScript2.ConsoleAbstraction;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Implements a wrapper for the default NUnit console
/// </summary>
public class BadNUnitTestConsole : IBadConsole
{
    /// <inheritdoc/>
    public void Write(string str)
    {
        TestContext.Write(str);
    }

    /// <inheritdoc/>
    public void WriteLine(string str)
    {
        TestContext.WriteLine(str);
    }

    /// <inheritdoc/>
    public string ReadLine()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task<string> ReadLineAsync()
    {
        return Task.Run(() => Console.ReadLine() ?? "");
    }

    /// <inheritdoc/>
    public void Clear()
    {
        //Do nothing
    }

    /// <inheritdoc/>
    public ConsoleColor ForegroundColor { get; set; }

    /// <inheritdoc/>
    public ConsoleColor BackgroundColor { get; set; }
}