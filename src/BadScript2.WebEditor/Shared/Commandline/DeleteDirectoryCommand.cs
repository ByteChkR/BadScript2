using BadScript2.IO;

namespace BadScript2.WebEditor.Shared.Commandline;

public class DeleteDirectoryCommand : ConsoleCommand
{
    public DeleteDirectoryCommand() : base(
        "deletedir",
        "Deletes a Directory at the specified path",
        new[] { "deldir", "rmdir" },
        new[] { "filepath" }
    ) { }

    public override string Execute(string args)
    {
        BadFileSystem.Instance.DeleteDirectory(args, true);

        return $"Directory '{BadFileSystem.Instance.GetFullPath(args)}' deleted";
    }
}