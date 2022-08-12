using Microsoft.JSInterop;

namespace BadScript2.WebEditor.Shared.Commandline;

public abstract class ConsoleCommand
{
    public readonly string[] Aliases;
    public readonly string[] Arguments;
    public readonly string Description;
    public readonly string Name;

    protected ConsoleCommand(string name, string description, string[] aliases, string[] arguments)
    {
        Name = name;
        Description = description;
        Aliases = aliases;
        Arguments = arguments;
    }

    public abstract string Execute(string args);
}