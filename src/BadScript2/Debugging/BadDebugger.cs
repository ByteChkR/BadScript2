using BadScript2.Common.Logging;
using BadScript2.Runtime.Error;

namespace BadScript2.Debugging;

/// <summary>
/// Public Debugger Interface
/// </summary>
public static class BadDebugger
{
    /// <summary>
    /// The currently attached debugger.
    /// </summary>
    private static IBadDebugger? s_Debugger;
    /// <summary>
    /// True if a debugger is attached.
    /// </summary>
    public static bool IsAttached { get; private set; }

    /// <summary>
    /// Attaches a debugger to the system.
    /// </summary>
    /// <param name="debugger">The debugger to attach.</param>
    /// <exception cref="BadRuntimeException">Gets raised if there is already a debugger attached</exception>
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

    /// <summary>
    /// Detaches the debugger from the system.
    /// </summary>
    public static void Detach()
    {
        BadLogger.Warn($"Debugger '{s_Debugger}' is Detached");
        IsAttached = false;
        s_Debugger = null;
    }

    /// <summary>
    /// Sends a step event to the debugger.
    /// </summary>
    /// <param name="stepInfo">The Step info</param>
    internal static void Step(BadDebuggerStep stepInfo)
    {
        s_Debugger?.Step(stepInfo);
    }
}