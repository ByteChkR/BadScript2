using BadScript2.IO;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Module.Handlers;

/// <summary>
///     Imports a module from a local path
/// </summary>
public class BadLocalPathImportHandler : BadImportHandler
{
    /// <summary>
    ///     The File System
    /// </summary>
    private readonly IFileSystem m_FileSystem;

    /// <summary>
    ///     The Runtime
    /// </summary>
    private readonly BadRuntime m_Runtime;

    /// <summary>
    ///     The Working Directory
    /// </summary>
    private readonly string m_WorkingDirectory;

    /// <summary>
    ///     Creates a new BadLocalPathImportHandler instance
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="workingDirectory"></param>
    public BadLocalPathImportHandler(BadRuntime runtime, string workingDirectory, IFileSystem mFileSystem)
    {
        m_Runtime = runtime;
        m_WorkingDirectory = workingDirectory;
        m_FileSystem = mFileSystem;
    }

    /// <summary>
    ///     Returns the full path for the specified path
    /// </summary>
    /// <param name="path">The Path</param>
    /// <returns>The Full Path</returns>
    private string GetPath(string path)
    {
        string p = m_FileSystem.GetFullPath(Path.Combine(m_WorkingDirectory, path));

        if (!p.EndsWith("." + BadRuntimeSettings.Instance.FileExtension))
        {
            p += "." + BadRuntimeSettings.Instance.FileExtension;
        }

        return p;
    }

    /// <inheritdoc />
    public override bool IsTransient()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool Has(string path)
    {
        string fullPath = GetPath(path);

        return m_FileSystem.IsFile(fullPath);
    }

    /// <inheritdoc />
    public override string GetHash(string path)
    {
        string fullPath = GetPath(path);

        return "file://" + fullPath;
    }

    /// <inheritdoc />
    public override IEnumerable<BadObject> Get(string path)
    {
        string fullPath = GetPath(path);

        IEnumerable<BadExpression> parsed = BadRuntime.ParseFile(fullPath, m_FileSystem);

        BadExecutionContext ctx = m_Runtime.CreateContext(Path.GetDirectoryName(fullPath) ?? "/");

        foreach (BadObject o in ctx.Execute(parsed))
        {
            yield return o;
        }

        yield return ctx.Scope.GetExports();
    }
}