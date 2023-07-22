using BadScript2.IO;

namespace BadScript2.WebEditor.Shared.Commandline;

public class BadNewFileCommand : BadConsoleCommand
{
    public BadNewFileCommand() : base(
        "new",
        "Creates a new file at the specified path",
        Array.Empty<string>(),
        new[]
        {
            "filepath",
        }
    ) { }

    public override string Execute(string args)
    {
        BadFileSystem.WriteAllText(args, "");

        return $"File '{BadFileSystem.Instance.GetFullPath(args)}' created";
    }
}