using BadScript2.IO;

namespace BadScript2.Interop.IO;

public static class BadIOApiExtensions
{
    public static BadRuntime UseFileSystemApi(this BadRuntime runtime, IFileSystem? fileSystem = null)
    {
        if (fileSystem != null)
        {
            runtime.ConfigureContextOptions(opts=>opts.AddOrReplaceApi(new BadIOApi(fileSystem)));
        }
        else
        {
            runtime.ConfigureContextOptions(opts=>opts.AddOrReplaceApi(new BadIOApi()));
        }

        return runtime;
    }
}