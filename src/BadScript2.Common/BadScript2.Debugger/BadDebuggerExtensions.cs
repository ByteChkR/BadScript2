using BadScript2.Debugger.Scriptable;

namespace BadScript2.Debugger;

public static class BadDebuggerExtensions
{
    public static BadRuntime UseConsoleDebugger(this BadRuntime runtime)
    {
        return runtime.UseDebugger(new BadConsoleDebugger());
    }

    public static BadRuntime UseScriptDebugger(this BadRuntime runtime, string? debuggerPath = null)
    {
        if (debuggerPath == null)
        {
            return runtime.UseDebugger(new BadScriptDebugger(runtime.Options));
        }

        return runtime.UseDebugger(new BadScriptDebugger(runtime.Options, debuggerPath));
    }
}