using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.NUnit;
using BadScript2.Runtime.Settings;

using NUnitLite;

namespace BadScript2.ConsoleCore.Systems.Test;

public class BadTestSystem : BadConsoleSystem<BadTestSystemSettings>
{
    public override string Name => "test";

    protected override int Run(BadTestSystemSettings settings)
    {
        try
        {
            new AutoRun(typeof(BadUnitTests).Assembly).Execute(Array.Empty<string>());

            return -1;
        }
        catch (Exception e)
        {
            BadConsole.WriteLine(e.ToString());

            return e.HResult;
        }
    }
}