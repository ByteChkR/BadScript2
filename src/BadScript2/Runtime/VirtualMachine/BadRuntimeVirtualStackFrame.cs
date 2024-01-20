namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Stores the current execution state of the Virtual Machine.
/// </summary>
public class BadRuntimeVirtualStackFrame
{
    /// <summary>
    /// The current execution context.
    /// </summary>
    public readonly BadExecutionContext Context;
    /// <summary>
    /// The current break instruction pointer.
    /// </summary>
    public int BreakPointer = -1;
    /// <summary>
    /// The current continue instruction pointer.
    /// </summary>
    public int ContinuePointer = -1;
    /// <summary>
    /// The current create instruction pointer.
    /// </summary>
    public int CreatePointer = 0;
    /// <summary>
    /// The Return Pointer
    /// </summary>
    public int ReturnPointer = -1;
    /// <summary>
    /// The current throw instruction pointer.
    /// </summary>
    public int ThrowPointer = -1;

    /// <summary>
    /// Creates a new <see cref="BadRuntimeVirtualStackFrame" /> instance.
    /// </summary>
    /// <param name="context">The current execution context.</param>
    public BadRuntimeVirtualStackFrame(BadExecutionContext context)
    {
        Context = context;
    }
}