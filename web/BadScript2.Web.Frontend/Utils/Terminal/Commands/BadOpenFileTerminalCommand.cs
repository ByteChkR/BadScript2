namespace BadScript2.Web.Frontend.Utils;

public class BadOpenFileTerminalCommand : BadTerminalCommand
{
    public BadOpenFileTerminalCommand() : base("Opens a file", "open") { }
    public override Task Run(BadReplContext context, string[] args)
    {
        if (args.Length == 0)
        {
            context.Console.WriteLine("No file specified.");
        }
        else
        {
            string path = args[0];
            context.OpenFile(path);
        }
        return Task.CompletedTask;
    }
}