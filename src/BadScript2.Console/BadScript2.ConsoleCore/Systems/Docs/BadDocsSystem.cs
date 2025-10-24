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
    public BadDocsSystem(Func<BadRuntime> runtime) : base(runtime) { }


    /// <inheritdoc />
    public override string Name => "docs";

    /// <summary>
    /// The Path to the Docs Directory
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets thrown if the directory is not set</exception>
    private static string GetDocsPath(IFileSystem fs)
    {
        string? s = BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Docs.RootDirectory");

        if (s == null)
        {
            throw new BadRuntimeException("Subsystems.Docs.RootDirectory not set");
        }

        fs.CreateDirectory(s);

        return s;
    }

    /// <summary>
    /// The Template Path
    /// </summary>
    private static string GetTemplatePath(IFileSystem fs) => Path.Combine(GetDocsPath(fs), "docs.bhtml");

    /// <inheritdoc />
    protected override Task<int> Run(BadDocsSystemSettings settings)
    {
        BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
        BadRuntimeSettings.Instance.WriteStackTraceInRuntimeErrors = true;
        var fs = new BadSystemFileSystem();
        string outFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".html");

        using var runtime = RuntimeFactory();
        var templatePath = GetTemplatePath(fs);
        string htmlString = BadHtmlTemplate.Create(templatePath, fs.ReadAllText(templatePath))
                                           .Run(null,
                                                new BadHtmlTemplateOptions
                                                {
                                                    Runtime = runtime, SkipEmptyTextNodes = true,
                                                }
                                               );
        File.WriteAllText(outFile, htmlString);
        Process p = new Process();
        p.StartInfo = new ProcessStartInfo(outFile) { UseShellExecute = true };
        p.Start();

        return Task.FromResult(0);
    }
}