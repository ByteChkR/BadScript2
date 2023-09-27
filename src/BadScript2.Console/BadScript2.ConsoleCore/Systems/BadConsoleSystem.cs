using BadScript2.ConsoleAbstraction;

namespace BadScript2.ConsoleCore.Systems;

/// <summary>
///     Implements a Console System that uses a settings object of Type T
/// </summary>
/// <typeparam name="T">The Settings Type</typeparam>
public abstract class BadConsoleSystem<T> : BadAConsoleSystem
{
    public override int Run(object? settings)
    {
        if (settings is T t)
        {
            return Run(t);
        }

        if (settings is null)
        {
            BadConsole.WriteLine("No settings provided.");
        }
        else
        {
            BadConsole.WriteLine("Invalid settings type");
        }

        return -1;
    }

    public override object? Parse(string[] args)
    {
        T t = CommandLine.Parser.Default.ParseArguments<T>(args).Value;

        if (t is null)
        {
            return null;
        }

        return t;
    }

    /// <summary>
    ///     Runs the Console System with the given settings
    /// </summary>
    /// <param name="settings">The Settings Object</param>
    /// <returns>Return Code</returns>
    protected abstract int Run(T settings);
}