using BadScript2.IO;

namespace BadScript2.WebEditor.Shared.Commandline;

public class BadChangeDirectoryCommand : BadConsoleCommand
{
    public BadChangeDirectoryCommand() : base(
        "cd",
        "Changes the Current Working Directory",
        Array.Empty<string>(),
        new[]
        {
            "(optional) directory",
        }
    ) { }

    public override string Execute(string args)
    {
        if (string.IsNullOrEmpty(args))
        {
            return BadFileSystem.Instance.GetCurrentDirectory();
        }

        BadFileSystem.Instance.SetCurrentDirectory(args);

        return "";
    }
}