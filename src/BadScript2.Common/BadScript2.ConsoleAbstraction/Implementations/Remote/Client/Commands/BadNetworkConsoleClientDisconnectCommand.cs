namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;

/// <summary>
///     Commands that disconnects the client from the server
/// </summary>
public class BadNetworkConsoleClientDisconnectCommand : BadNetworkConsoleClientCommand
{
	/// <summary>
	///     The Client that owns this Command
	/// </summary>
	private readonly BadNetworkConsoleClient m_Client;

	/// <summary>
	///     Constructs a new Command with the given name
	/// </summary>
	/// <param name="client">The Client that owns this command</param>
	public BadNetworkConsoleClientDisconnectCommand(BadNetworkConsoleClient client) : base("disconnect")
    {
        m_Client = client;
    }

    /// <inheritdoc />
    public override void Invoke(string[] args)
    {
        m_Client.Stop();
    }
}