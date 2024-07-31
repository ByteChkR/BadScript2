namespace BadScript2.Interop.NetHost;

/// <summary>
///     NetHost API Extensions
/// </summary>
public static class BadNetApiExtensions
{
    /// <summary>
    ///     Configures the Runtime to use the NetHost API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseNetHostApi(this BadRuntime runtime)
    {
        return runtime
               .UseExtension<BadNetHostExtensions>()
               .UseApi(new BadNetHostApi(), true);
    }
}