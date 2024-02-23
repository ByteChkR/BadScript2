using System.Diagnostics;

using BadHtml;

using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

/// <summary>
/// Contains the 'html' console command implementation
/// </summary>
namespace BadScript2.ConsoleCore.Systems.Docs;

/// <summary>
///     Runs the Html Template Engine
/// </summary>
public class BadDocsSystem : BadConsoleSystem<BadDocsSystemSettings>
{
    /// <summary>
    ///     Creates a new BadHtmlSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadDocsSystem(BadRuntime runtime) : base(runtime) { }


    /// <inheritdoc />
    public override string Name => "docs";

    private static string DocsPath
    {
        get
        {
            string? s = BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Docs.RootDirectory");

            if (s == null)
            {
                throw new BadRuntimeException("Subsystems.Docs.RootDirectory not set");
            }

            BadFileSystem.Instance.CreateDirectory(s);

            return s;
        }
    }
    private static string TemplatePath => Path.Combine(DocsPath, "docs.bhtml");

    /// <inheritdoc />
    protected override int Run(BadDocsSystemSettings settings)
    {
        BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
        BadRuntimeSettings.Instance.WriteStackTraceInRuntimeErrors = true;
        string outFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".html");
        string htmlString = BadHtmlTemplate.Create(TemplatePath).Run(null, new BadHtmlTemplateOptions { Runtime = Runtime, SkipEmptyTextNodes = true });
        File.WriteAllText(outFile, htmlString);
        Process p = new Process();
        p.StartInfo = new ProcessStartInfo(outFile)
        {
            UseShellExecute = true,
        };
        p.Start();

        return 0;
    }
}