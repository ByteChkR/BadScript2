namespace BadScript2.Interop.NUnit;

/// <summary>
/// Nunit API Extensions
/// </summary>
public static class BadNUnitApiExtensions
{
    /// <summary>
    /// Configures the Runtime to use the NUnit API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseNUnitApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadNUnitApi()));

        return runtime;
    }

    /// <summary>
    /// Configures the Runtime to use the NUnit Console API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="console">The Console Builder</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseNUnitConsole(this BadRuntime runtime, BadUnitTestContextBuilder console)
    {
        return runtime
            .ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadNUnitConsoleApi(console)))
            .UseConsole(new BadNUnitTestConsole())
            .UseConsoleLogWriter();
    }
}