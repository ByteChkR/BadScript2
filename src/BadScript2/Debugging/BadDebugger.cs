using BadScript2.Common.Logging;
using BadScript2.Runtime.Error;

namespace BadScript2.Debugging;

public static class BadDebugger
{
    private static IBadDebugger? s_Debugger = null;
    public static bool IsAttached { get; private set; }

    public static void Attach(IBadDebugger debugger)
    {
        if (IsAttached)
        {
            throw new BadRuntimeException("Already a Debugger Attached");
        }

        IsAttached = true;
        s_Debugger = debugger;
        BadLogger.Warn($"Debugger '{debugger}' is Attached");
    }

    public static void Detach()
    {
        BadLogger.Warn($"Debugger '{s_Debugger}' is Detached");
        IsAttached = false;
        s_Debugger = null;
    }

    internal static void Step(BadDebuggerStep step) => s_Debugger?.Step(step);

}