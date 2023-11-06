using BadScript2.ConsoleAbstraction.Implementations.Remote.Client;

namespace BadScript2.ConsoleCore.Systems.Run;

/// <summary>
///     Connects to a remote console
/// </summary>
public class BadRemoteConsoleSystem : BadConsoleSystem<BadRemoteConsoleSystemSettings>
{
    public override string Name => "remote";

    protected override int Run(BadRemoteConsoleSystemSettings settings)
    {
        BadNetworkConsoleClient client = new BadNetworkConsoleClient(settings.Host, settings.Port);
        client.Start();

        return 0;
    }

    public BadRemoteConsoleSystem(BadRuntime runtime) : base(runtime) { }
}