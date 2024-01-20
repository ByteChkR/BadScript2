using BadScript2.Debugger.Scriptable;

namespace BadScript2.Debugger;

public static class BadDebuggerExtensions
{
    public static BadRuntime UseConsoleDebugger(this BadRuntime runtime)
    {
        return runtime
            .UseDebuggerExtensions()
            .UseDebugger(new BadConsoleDebugger());
    }

    public static BadRuntime UseDebuggerExtensions(this BadRuntime runtime)
    {
        return runtime.ConfigureContextOptions(opts => opts.AddExtension<BadScriptDebuggerExtension>());
    }

    public static BadRuntime UseScriptDebugger(this BadRuntime runtime, string? debuggerPath = null)
    {
        return runtime.UseDebuggerExtensions()
            .UseDebugger(debuggerPath == null ? new BadScriptDebugger(runtime) : new BadScriptDebugger(runtime, debuggerPath));
    }
}