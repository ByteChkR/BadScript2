/// <summary>
/// Contains the Console Subsystems of the Console Application
/// </summary>
namespace BadScript2.ConsoleCore.Systems;

/// <summary>
///     Implements a the base features of a Console System
/// </summary>
public abstract class BadAConsoleSystem
{
    /// <summary>
    /// The Runtime to use
    /// </summary>
    protected readonly BadRuntime Runtime;

    /// <summary>
    /// Creates a new BadAConsoleSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
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