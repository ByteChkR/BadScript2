using BadScript2.IO;

namespace BadScript2.WebEditor.Shared.Commandline
{
    public class BadExportCommand : BadConsoleCommand
    {
        private readonly Action<string, byte[]> m_OnDownloadFile;

        public BadExportCommand(Action<string, byte[]> onDownloadFile) : base(
            "export",
            "Exports the Project",
            Array.Empty<string>(),
            Array.Empty<string>()
        )
        {
            m_OnDownloadFile = onDownloadFile;
        }

        public override string Execute(string args)
        {
            MemoryStream ms = new MemoryStream();
            BadFileSystem.Instance.ExportZip(ms);
            m_OnDownloadFile.Invoke("Export.zip", ms.ToArray());

            return "Exported!";
        }
    }
}