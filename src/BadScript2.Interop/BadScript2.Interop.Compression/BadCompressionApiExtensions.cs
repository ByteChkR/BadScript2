using BadScript2.IO;

namespace BadScript2.Interop.Compression;

public static class BadCompressionApiExtensions
{
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