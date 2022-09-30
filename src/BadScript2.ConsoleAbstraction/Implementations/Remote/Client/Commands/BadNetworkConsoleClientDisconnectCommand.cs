namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands
{
    public class BadNetworkConsoleClientDisconnectCommand : BadNetworkConsoleClientCommand
    {
        private readonly BadNetworkConsoleClient m_Client;

        public BadNetworkConsoleClientDisconnectCommand(BadNetworkConsoleClient client) : base("disconnect")
        {
            m_Client = client;
        }

        public override void Invoke(string args)
        {
            m_Client.Stop();
        }
    }
}