namespace BadScript2.ConsoleCore.Systems;

public abstract class BadConsoleSystem
{
    public abstract string Name { get; }

    public abstract int Run(object? settings);

    public abstract object? Parse(string[] args);
}

public abstract class BadConsoleSystem<T> : BadConsoleSystem
{
    public override int Run(object? settings)
    {
        if (settings is T t)
        {
            return Run(t);
        }

        if (settings is null)
        {
            Console.WriteLine("No settings provided.");
        }
        else
        {
            Console.WriteLine("Invalid settings type");
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