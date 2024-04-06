namespace BadScript2.Web.Frontend.Utils;

public class BadListDirectoryTerminalCommand : BadTerminalCommand
{
    public BadListDirectoryTerminalCommand() : base("Lists the contents of the Current Working Directory", "ls", "dir") { }
    public override Task Run(BadReplContext context, string[] args)
    {
        string dir = args.Length == 0 ? context.FileSystem.GetCurrentDirectory() : args[0];
        foreach (string file in context.FileSystem.GetDirectories(dir, false))
        {
            context.Console.WriteLine($"{file,-32} <DIR> <DIR>");
        }
        foreach (string file in context.FileSystem.GetFiles(dir, "", false))
        {
            using var fs = context.FileSystem.OpenRead(file);
            context.Console.WriteLine($"{file,-32} <FILE> {fs.Length,8} byte(s)");
        }
        return Task.CompletedTask;
    }
}