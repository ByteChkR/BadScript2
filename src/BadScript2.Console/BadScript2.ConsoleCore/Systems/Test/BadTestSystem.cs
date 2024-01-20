using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.NUnit;

using NUnitLite;

namespace BadScript2.ConsoleCore.Systems.Test;

/// <summary>
///     Runs unit tests using NUnitLite
/// </summary>
public class BadTestSystem : BadConsoleSystem<BadTestSystemSettings>
{
    public BadTestSystem(BadRuntime runtime) : base(runtime) { }

    public override string Name => "test";

    protected override int Run(BadTestSystemSettings settings)
    {
        try
        {
            new AutoRun(typeof(BadUnitTests).Assembly).Execute(Array.Empty<string>());

            return 0;
        }
        catch (Exception e)
        {
            BadConsole.WriteLine(e.ToString());

            return e.HResult;
        }
    }
}