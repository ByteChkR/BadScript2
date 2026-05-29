using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Stores the current execution state of the Virtual Machine.
/// </summary>
public class BadRuntimeVirtualStackFrame
{
    /// <summary>
    ///     The current execution context.
    /// </summary>
    public readonly BadExecutionContext Context;

    /// <summary>
    ///     The compiled function that owns this frame, if any.
    /// </summary>
    public BadCompiledFunction? Function;

    /// <summary>
    ///     The local variable slots for this frame (Phase 5 optimization).
    ///     Used for compiled functions with slot-based local variables.
    /// </summary>
    public BadObject[]? LocalSlots;

    /// <summary>
    ///     The symbol table for this frame (Phase 5 optimization).
    ///     Maps variable names to slot indices.
    /// </summary>
    public BadSymbolTable? SymbolTable;

    /// <summary>
    ///     Cached slot-backed variable references for this frame.
    /// </summary>
    public Dictionary<string, BadObjectReference>? SlotReferences;

    /// <summary>
    ///     Property metadata for slot-backed locals and parameters.
    /// </summary>
    public BadPropertyInfo[]? SlotPropertyInfos;

    /// <summary>
    ///     Captured attributes for slot-backed locals and parameters.
    /// </summary>
    public BadObject[][]? SlotAttributes;

    /// <summary>
    ///     Lazy-resolved references for captured variables.
    /// </summary>
    public BadObjectReference?[]? CaptureReferences;

    /// <summary>
    ///     The current break instruction pointer.
    /// </summary>
    public int BreakPointer = -1;

    /// <summary>
    ///     The current continue instruction pointer.
    /// </summary>
    public int ContinuePointer = -1;

    /// <summary>
    ///     The current create instruction pointer.
    /// </summary>
    public int CreatePointer = 0;

    /// <summary>
    ///     The Return Pointer
    /// </summary>
    public int ReturnPointer = -1;

    /// <summary>
    ///     When non-null, this frame was pushed by the static inline fast-path.
    ///     Holds the caller's instruction array so it can be restored on return.
    /// </summary>
    public BadInstruction[]? SavedInstructions = null;

    /// <summary>
    ///     The current throw instruction pointer.
    /// </summary>
    public int ThrowPointer = -1;

    /// <summary>
    ///     Creates a new <see cref="BadRuntimeVirtualStackFrame" /> instance.
    /// </summary>
    /// <param name="context">The current execution context.</param>
    public BadRuntimeVirtualStackFrame(BadExecutionContext context)
    {
        Context = context;
    }
}
