namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client;

/// <summary>
///     Used to parse commands on the client
/// </summary>
public interface IBadNetworkConsoleClientCommandParser
{
    /// <summary>
    ///     Executes a command on the client
    /// </summary>
    /// <param name="command">The command to execute</param>
    public void ExecuteCommand(string command);
}