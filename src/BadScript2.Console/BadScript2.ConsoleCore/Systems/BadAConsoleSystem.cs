using BadScript2.ConsoleAbstraction;

namespace BadScript2.ConsoleCore.Systems;

public abstract class BadAConsoleSystem
{
    public abstract string Name { get; }

    public abstract int Run(object? settings);

    public abstract object? Parse(string[] args);
}

public abstract class BadConsoleSystem<T> : BadAConsoleSystem
{
    public override int Run(object? settings)
    {
        if (settings is T t)
        {
            return Run(t);
        }

        if (settings is null)
        {
            BadConsole.WriteLine("No settings provided.");
        }
        else
        {
            BadConsole.WriteLine("Invalid settings type");
        }

        return -1;
    }

    public override object? Parse(string[] args)
    {
        T t = CommandLine.Parser.Default.ParseArguments<T>(args).Value;
        if (t is null)
        {
            return null;
        }

        return t;
    }

    protected abstract int Run(T settings);
}