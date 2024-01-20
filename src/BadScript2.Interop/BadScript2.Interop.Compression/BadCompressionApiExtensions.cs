using BadScript2.IO;

namespace BadScript2.Interop.Compression;

/// <summary>
/// Compression API Extensions
/// </summary>
public static class BadCompressionApiExtensions
{
    /// <summary>
    /// Configures the Runtime to use the Compression API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="fileSystem">The File System to use</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseCompressionApi(this BadRuntime runtime, IFileSystem? fileSystem = null)
    {
        if (fileSystem != null)
        {
            runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadCompressionApi(fileSystem)));
        }
        else
        {
            runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadCompressionApi()));
        }

        return runtime;
    }
}