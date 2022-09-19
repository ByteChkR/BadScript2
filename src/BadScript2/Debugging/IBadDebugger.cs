namespace BadScript2.Debugging;

/// <summary>
/// Defines the Debugging Interface
/// </summary>
public interface IBadDebugger
{
    /// <summary>
    /// Gets called on every step when the Debugger is attached.
    /// </summary>
    /// <param name="stepInfo">The Current Step Information</param>
    void Step(BadDebuggerStep stepInfo);
}