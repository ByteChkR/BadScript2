namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;

/// <summary>
///     Abstract Base Class for Remote Console Commands
/// </summary>
public abstract class BadNetworkConsoleClientCommand
{
	/// <summary>
	///     Constructs a new Command with the given name
	/// </summary>
	/// <param name="name">Command Name</param>
	protected BadNetworkConsoleClientCommand(string name)
	{
		Name = name;
	}

	/// <summary>
	///     The Name of the Command
	/// </summary>
	public string Name { get; }

	/// <summary>
	///     Executes the Command with the given arguments
	/// </summary>
	/// <param name="args">Arguments</param>
	public abstract void Invoke(string args);
}
