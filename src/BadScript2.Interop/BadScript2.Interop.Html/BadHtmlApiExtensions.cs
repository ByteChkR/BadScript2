namespace BadScript2.Interop.Html;

/// <summary>
///     Html APi Extensions
/// </summary>
public static class BadHtmlApiExtensions
{
    /// <summary>
    ///     Configures the Runtime to use the Html API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseHtmlApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadHtmlApi()));

        return runtime;
    }
}