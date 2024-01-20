using BadScript2.Debugger.Scriptable;

namespace BadScript2.Debugger;

/// <summary>
/// Debugger Extensions for the BadScript Runtime
/// </summary>
public static class BadDebuggerExtensions
{
    /// <summary>
    /// Configures the Runtime to use the Console Debugger
    /// </summary>
    /// <param name="runtime">The Runtime to configure</param>
    /// <returns>The configured Runtime</returns>
    public static BadRuntime UseConsoleDebugger(this BadRuntime runtime)
    {
        return runtime
            .UseDebuggerExtensions()
            .UseDebugger(new BadConsoleDebugger());
    }

    /// <summary>
    /// Configures the Runtime to use the Debugger Extensions
    /// </summary>
    /// <param name="runtime">The Runtime to configure</param>
    /// <returns>The configured Runtime</returns>
    public static BadRuntime UseDebuggerExtensions(this BadRuntime runtime)
    {
        return runtime.ConfigureContextOptions(opts => opts.AddExtension<BadScriptDebuggerExtension>());
    }

    /// <summary>
    /// Configures the Runtime to use the Script Debugger
    /// </summary>
    /// <param name="runtime">The Runtime to configure</param>
    /// <param name="debuggerPath">The File Path to the Debugger</param>
    /// <returns>The configured Runtime</returns>
    public static BadRuntime UseScriptDebugger(this BadRuntime runtime, string? debuggerPath = null)
    {
        return runtime.UseDebuggerExtensions()
            .UseDebugger(debuggerPath == null ? new BadScriptDebugger(runtime) : new BadScriptDebugger(runtime, debuggerPath));
    }
}