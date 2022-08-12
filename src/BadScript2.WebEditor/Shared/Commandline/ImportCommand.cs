namespace BadScript2.WebEditor.Shared.Commandline;

public class ImportCommand : ConsoleCommand
{
    private Action<string> m_Import;
    private Action m_UploadFile;

    public ImportCommand(Action<string> importFunc, Action uploadFile) : base(
        "import",
        "Imports a Zip File into the Project. Prebuilt Images: (images/bootstrap/RootFS.zip, images/Minimal.zip, images/EmptyProject.zip, images/CoreProjects.zip)",
        Array.Empty<string>(),
        new []{"(optional) url; if not specified opens upload file dialog."}
    )
    {
        m_Import = importFunc;
        m_UploadFile = uploadFile;
    }
    public override string Execute(string args)
    {


        if (string.IsNullOrEmpty(args))
        {
            m_UploadFile();
        }
        else
        {
            m_Import(args);
        }
        

        return "Imported!";
    }
}