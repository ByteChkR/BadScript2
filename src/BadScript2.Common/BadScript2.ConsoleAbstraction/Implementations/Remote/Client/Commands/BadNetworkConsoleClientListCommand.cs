namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;

/// <summary>
///     Command that lists all commands available to the user
/// </summary>
public class BadNetworkConsoleClientListCommand : BadNetworkConsoleClientCommand
{
	/// <summary>
	///     The Command Parser
	/// </summary>
	private readonly BadDefaultNetworkClientCommandParser m_Parser;

	/// <summary>
	///     Constructs a new Command with the given name
	/// </summary>
	/// <param name="client">The Client</param>
	/// <param name="parser">The Command Parser</param>
	public BadNetworkConsoleClientListCommand(BadDefaultNetworkClientCommandParser parser) : base("list")
    {
        m_Parser = parser;
    }

    /// <inheritdoc />
    public override void Invoke(string[] args)
    {
        BadConsole.WriteLine("Commands:");

        foreach (BadNetworkConsoleClientCommand command in m_Parser.Commands)
        {
            BadConsole.WriteLine('\t' + command.Name);
        }
    }
}