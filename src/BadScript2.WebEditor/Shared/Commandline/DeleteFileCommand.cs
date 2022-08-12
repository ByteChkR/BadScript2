using BadScript2.IO;

namespace BadScript2.WebEditor.Shared.Commandline;

public class DeleteFileCommand : ConsoleCommand
{
    public DeleteFileCommand() : base(
        "delete",
        "Deletes a file at the specified path",
        new []{"del", "rm"},
        new[] { "filepath" }
    ) { }

    public override string Execute(string args)
    {
        BadFileSystem.Instance.DeleteFile(args);
        return $"File '{BadFileSystem.Instance.GetFullPath(args)}' deleted";
    }
}