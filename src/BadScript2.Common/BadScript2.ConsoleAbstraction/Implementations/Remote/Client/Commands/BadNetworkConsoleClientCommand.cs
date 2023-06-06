namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;

public abstract class BadNetworkConsoleClientCommand
{
	protected BadNetworkConsoleClientCommand(string name)
	{
		Name = name;
	}

	public string Name { get; }

	public abstract void Invoke(string args);
}
