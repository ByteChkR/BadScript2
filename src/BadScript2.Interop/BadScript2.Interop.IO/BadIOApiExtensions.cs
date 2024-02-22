using BadScript2.IO;

namespace BadScript2.Interop.IO;

/// <summary>
///     IO Api Extensions
/// </summary>
public static class BadIOApiExtensions
{
    /// <summary>
    ///     Configures the Runtime to use the IO API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="fileSystem">The File System to use</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseFileSystemApi(this BadRuntime runtime, IFileSystem? fileSystem = null)
    {
        if (fileSystem != null)
        {
            runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadIOApi(fileSystem)));
        }
        else
        {
            runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadIOApi(BadFileSystem.Instance)));
        }

        return runtime;
    }
}