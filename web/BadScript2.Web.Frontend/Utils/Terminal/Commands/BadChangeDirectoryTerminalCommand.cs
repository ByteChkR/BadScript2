using BadScript2.ConsoleCore.Systems.Docs;
using BadScript2.ConsoleCore.Systems.Html;
using BadScript2.ConsoleCore.Systems.Settings;
namespace BadScript2.Web.Frontend.Utils;

public class BadClearConsoleTerminalCommand : BadTerminalCommand
{
    public BadClearConsoleTerminalCommand() : base("Clears the Console", "clear") { }
    public override Task Run(BadReplContext context, string[] args)
    {
        context.Console.Clear();
        return Task.CompletedTask;
    }
}
public class BadChangeDirectoryTerminalCommand : BadTerminalCommand
{
    public BadChangeDirectoryTerminalCommand() : base("Changes the Current Working Directory", "cd") { }
    public override Task Run(BadReplContext context, string[] args)
    {
        if (args.Length == 0)
        {
            context.Console.WriteLine(context.FileSystem.GetCurrentDirectory());
        }
        else
        {
            string path = args[0];
            context.FileSystem.SetCurrentDirectory(path);
        }
        return Task.CompletedTask;
    }
}