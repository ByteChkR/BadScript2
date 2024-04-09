using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.NUnit;

using NUnitLite;

/// <summary>
/// Contains the 'test' command implementation
/// </summary>
namespace BadScript2.ConsoleCore.Systems.Test;

/// <summary>
///     Runs unit tests using NUnitLite
/// </summary>
public class BadTestSystem : BadConsoleSystem<BadTestSystemSettings>
{
    /// <summary>
    ///     Creates a new BadTestSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadTestSystem(BadRuntime runtime) : base(runtime) { }

    /// <inheritdoc />
    public override string Name => "test";

    /// <inheritdoc />
    protected override Task<int> Run(BadTestSystemSettings settings)
    {
        try
        {
            new AutoRun(typeof(BadUnitTests).Assembly).Execute(Array.Empty<string>());

            return Task.FromResult(0);
        }
        catch (Exception e)
        {
            BadConsole.WriteLine(e.ToString());

            return Task.FromResult(e.HResult);
        }
    }
}