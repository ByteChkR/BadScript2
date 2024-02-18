using BadScript2.IO;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

namespace BadScript2.Runtime.Module.Handlers;

/// <summary>
///     Imports a module from a library path
/// </summary>
public class BadLibraryPathImportHandler : BadImportHandler
{
    /// <summary>
    ///     The Runtime
    /// </summary>
    private readonly BadRuntime m_Runtime;

    /// <summary>
    ///     Creates a new BadLibraryPathImportHandler instance
    /// </summary>
    /// <param name="runtime"></param>
    public BadLibraryPathImportHandler(BadRuntime runtime)
    {
        m_Runtime = runtime;
    }

    /// <summary>
    ///     The Library Directory
    /// </summary>
    /// <exception cref="BadRuntimeException">If the Library Directory is not found</exception>
    private static string LibraryDirectory =>
        BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Run.LibraryDirectory") ??
        throw new BadRuntimeException("Test directory not found");

    /// <summary>
    ///     Returns the path without the < and>
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>Path without the < and></returns>
    private string GetPath(string path)
    {
        string p = path.Substring(1, path.Length - 2);
        if (!p.EndsWith("." + BadRuntimeSettings.Instance.FileExtension))
        {
            p += "." + BadRuntimeSettings.Instance.FileExtension;
        }

        return p;
    }

    /// <inheritdoc />
    public override bool Has(string path)
    {
        return path.StartsWith("<") && path.EndsWith(">");
    }

    /// <inheritdoc />
    public override string GetHash(string path)
    {
        return "lib://" + GetPath(path);
    }

    /// <inheritdoc />
    public override IEnumerable<BadObject> Get(string path)
    {
        string p = Path.Combine(LibraryDirectory, GetPath(path));
        string d = Path.GetDirectoryName(p) ?? throw new BadRuntimeException("Invalid Library Path");
        string fullPath = BadFileSystem.Instance.GetFullPath(Path.Combine(d, path));

        IEnumerable<BadExpression> parsed = BadRuntime.ParseFile(fullPath);

        BadExecutionContext ctx = m_Runtime.CreateContext(Path.GetDirectoryName(fullPath) ?? "/");


        foreach (BadObject o in ctx.Execute(parsed))
        {
            yield return o;
        }

        yield return ctx.Scope.GetExports();
    }
}