namespace BadScript2.Interop.Net;

/// <summary>
/// Net Api Extensions
/// </summary>
public static class BadNetApiExtensions
{
    /// <summary>
    /// Configures the Runtime to use the Net API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseNetApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts => opts.AddExtension<BadNetInteropExtensions>());
        runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadNetApi()));

        return runtime;
    }
}