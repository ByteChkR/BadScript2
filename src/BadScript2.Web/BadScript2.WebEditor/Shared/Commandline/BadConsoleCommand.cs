namespace BadScript2.WebEditor.Shared.Commandline;

public abstract class BadConsoleCommand
{
	public readonly string[] Aliases;
	public readonly string[] Arguments;
	public readonly string Description;
	public readonly string Name;

	protected BadConsoleCommand(string name, string description, string[] aliases, string[] arguments)
	{
		Name = name;
		Description = description;
		Aliases = aliases;
		Arguments = arguments;
	}

	public abstract string Execute(string args);
}
