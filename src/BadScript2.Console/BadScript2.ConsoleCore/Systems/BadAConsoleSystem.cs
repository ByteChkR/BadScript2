namespace BadScript2.ConsoleCore.Systems;

public abstract class BadAConsoleSystem
{
    public abstract string Name { get; }

    public abstract int Run(object? settings);

    public abstract object? Parse(string[] args);
}