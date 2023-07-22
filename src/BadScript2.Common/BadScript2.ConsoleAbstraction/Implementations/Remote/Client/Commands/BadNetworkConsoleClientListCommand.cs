namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;

/// <summary>
///     Command that lists all commands available to the user
/// </summary>
public class BadNetworkConsoleClientListCommand : BadNetworkConsoleClientCommand
{
	/// <summary>
	///     The Client that owns this Command
	/// </summary>
	private readonly BadNetworkConsoleClient m_Client;

	/// <summary>
	///     Constructs a new Command with the given name
	/// </summary>
	/// <param name="client">The Client that owns this command</param>
	public BadNetworkConsoleClientListCommand(BadNetworkConsoleClient client) : base("list")
    {
        m_Client = client;
    }

    public override void Invoke(string args)
    {
        BadConsole.WriteLine("Commands:");

        foreach (BadNetworkConsoleClientCommand command in m_Client.Commands)
        {
            BadConsole.WriteLine('\t' + command.Name);
        }
    }
}