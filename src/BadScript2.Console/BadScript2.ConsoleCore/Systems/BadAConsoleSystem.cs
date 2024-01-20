namespace BadScript2.ConsoleCore.Systems;

/// <summary>
///     Implements a the base features of a Console System
/// </summary>
public abstract class BadAConsoleSystem
{
    protected readonly BadRuntime Runtime;

    protected BadAConsoleSystem(BadRuntime runtime)
    {
        Runtime = runtime;
    }

    /// <summary>
    ///     The Name of the Console System
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///     Runs the Console System with the given settings
    /// </summary>
    /// <param name="settings">The Settings Object</param>
    /// <returns>Return Code</returns>
    public abstract int Run(object? settings);

    /// <summary>
    ///     Parses the given arguments into a settings object
    /// </summary>
    /// <param name="args">Startup Arguments</param>
    /// <returns>Settings Object</returns>
    public abstract object? Parse(string[] args);
}