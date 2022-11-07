namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;

public class BadNetworkConsoleClientListCommand : BadNetworkConsoleClientCommand
{
    private readonly BadNetworkConsoleClient m_Client;

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