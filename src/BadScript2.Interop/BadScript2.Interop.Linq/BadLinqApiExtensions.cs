namespace BadScript2.Interop.Linq;

/// <summary>
/// Linq API Extensions
/// </summary>
public static class BadLinqApiExtensions
{
    /// <summary>
    /// Configures the Runtime to use the Linq API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseLinqApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts => opts.AddExtension<BadLinqExtensions>());

        return runtime;
    }
}