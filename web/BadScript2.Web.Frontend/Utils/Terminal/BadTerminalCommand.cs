namespace BadScript2.Web.Frontend.Utils;

public abstract class BadTerminalCommand
{
    protected BadTerminalCommand(string description, string name, params string[] aliases)
    {
        Name = name;
        Description = description;
        Names = aliases.Prepend(name);
    }

    public string Name { get; }

    public string Description { get; }

    public IEnumerable<string> Names { get; }

    public abstract Task Run(BadReplContext context, string[] args);
}