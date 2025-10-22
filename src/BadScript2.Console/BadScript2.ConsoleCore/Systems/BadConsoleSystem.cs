using BadScript2.ConsoleAbstraction;

using CommandLine;

namespace BadScript2.ConsoleCore.Systems;

/// <summary>
///     Implements a Console System that uses a settings object of Type T
/// </summary>
/// <typeparam name="T">The Settings Type</typeparam>
public abstract class BadConsoleSystem<T> : BadAConsoleSystem
{
    /// <summary>
    ///     Creates a new BadConsoleSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    protected BadConsoleSystem(Func<BadRuntime> runtime) : base(runtime) { }

    /// <inheritdoc />
    public override Task<int> Run(object? settings)
    {
        if (settings is T t)
        {
            return Run(t);
        }

        BadConsole.WriteLine(settings is null ? "No settings provided." : "Invalid settings type");

        return Task.FromResult(-1);
    }

    /// <inheritdoc />
    public override object? Parse(string[] args)
    {
        CommandLine.Parser parser =
            new CommandLine.Parser(() => ParserSettings.CreateDefault(ParserSettings.DefaultMaximumLength));

        T t = parser.ParseArguments<T>(args)
                    .Value;

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
    protected abstract Task<int> Run(T settings);
}