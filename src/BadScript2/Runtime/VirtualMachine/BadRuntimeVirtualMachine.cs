using System.Runtime.ExceptionServices;
using BadScript2.Debugging;
using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Module;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine.Compiler;

namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Implements a Virtual Machine for the BadScript Language.
/// </summary>
public class BadRuntimeVirtualMachine
{
    /// <summary>
    ///     Gets raised when the VM executes an <see cref="BadOpCode.Eval" /> instruction.
    /// </summary>
    public static event Action<BadExpression>? OnEvalInstruction;

    /// <summary>
    ///     Count of executed <see cref="BadOpCode.Eval" /> instructions.
    /// </summary>
    public static long EvalInstructionCount;

    /// <summary>
    ///     Count of static inline fast-path invocations
    /// </summary>
    public static long StaticInlineFastPathCount;

    /// <summary>
    ///     Count of InvokeMember calls using method-slot fast-path.
    /// </summary>
    public static long InvokeMemberMethodSlotFastPathCount;

    /// <summary>
    ///     Count of InvokeMember calls using call-site fallback path.
    /// </summary>
    public static long InvokeMemberCallSitePathCount;

    /// <summary>
    ///     Count of LoadMember operations using property reference cache fast-path.
    /// </summary>
    public static long LoadMemberPropertyCacheFastPathCount;

    /// <summary>
    ///     Count of LoadMember operations using slow property lookup path.
    /// </summary>
    public static long LoadMemberPropertySlowPathCount;

    /// <summary>
    ///     Count of LoadVar operations resolved via slot-backed local fast-path.
    /// </summary>
    public static long LoadVarSlotPathCount;

    /// <summary>
    ///     Count of LoadVar operations resolved via capture reference path.
    /// </summary>
    public static long LoadVarCapturePathCount;

    /// <summary>
    ///     Count of LoadVar operations resolved via scope fallback path.
    /// </summary>
    public static long LoadVarScopePathCount;

    /// <summary>
    ///     Count of MoveNext fast-path hits for native enumerators.
    /// </summary>
    public static long LoopMoveNextNativeFastPathCount;

    /// <summary>
    ///     Count of MoveNext fast-path hits for method-slot based enumerators.
    /// </summary>
    public static long LoopMoveNextMethodSlotFastPathCount;

    /// <summary>
    ///     Count of MoveNext fallback path hits.
    /// </summary>
    public static long LoopMoveNextSlowPathCount;

    /// <summary>
    ///     Count of GetCurrent fast-path hits for native enumerators.
    /// </summary>
    public static long LoopGetCurrentNativeFastPathCount;

    /// <summary>
    ///     Count of GetCurrent fast-path hits for method-slot based enumerators.
    /// </summary>
    public static long LoopGetCurrentMethodSlotFastPathCount;

    /// <summary>
    ///     Count of GetCurrent fallback path hits.
    /// </summary>
    public static long LoopGetCurrentSlowPathCount;

    /// <summary>
    ///     Count of generic Invoke calls that use static inline fast-path.
    /// </summary>
    public static long InvokeStaticInlineFromInvokeFastPathCount;

    /// <summary>
    ///     Count of InvokeCompiled calls that use static inline fast-path.
    /// </summary>
    public static long InvokeStaticInlineFromInvokeCompiledFastPathCount;

    /// <summary>
    ///     Count of InvokeCompiled calls that fall back to spawning a nested VM.
    /// </summary>
    public static long InvokeCompiledFallbackVmPathCount;

    /// <summary>
    ///     Count of generic Invoke calls that fall back to regular invocation.
    /// </summary>
    public static long InvokeFallbackPathCount;

    /// <summary>
    ///     Count of generic Invoke self-recursion path hits.
    /// </summary>
    public static long InvokeSelfRecursionPathCount;

    /// <summary>
    ///     Count of InvokeCompiled self-recursion path hits.
    /// </summary>
    public static long InvokeCompiledSelfRecursionPathCount;

    /// <summary>
    ///     Count of null-checked InvokeMember short-circuit hits.
    /// </summary>
    public static long InvokeMemberNullCheckedShortCircuitCount;

    /// <summary>
    ///     Count of LoadMember generic path hits.
    /// </summary>
    public static long LoadMemberGenericPathCount;

    /// <summary>
    ///     Count of GetEnumerator direct path hits (object already has MoveNext/GetCurrent).
    /// </summary>
    public static long GetEnumeratorDirectPathCount;

    /// <summary>
    ///     Count of GetEnumerator method path hits (calling GetEnumerator function).
    /// </summary>
    public static long GetEnumeratorMethodPathCount;

    /// <summary>
    ///     Count of GetEnumerator enumerable adapter path hits (IBadEnumerable).
    /// </summary>
    public static long GetEnumeratorEnumerablePathCount;

    /// <summary>
    ///     Count of GetEnumerator fallback path hits (push target as-is).
    /// </summary>
    public static long GetEnumeratorFallbackPathCount;

    /// <summary>
    ///     Count of LoadLocal opcode executions.
    /// </summary>
    public static long LoadLocalOpcodeCount;

    /// <summary>
    ///     Count of StoreLocal opcode executions.
    /// </summary>
    public static long StoreLocalOpcodeCount;

    /// <summary>
    ///     Count of LoadCaptured opcode executions.
    /// </summary>
    public static long LoadCapturedOpcodeCount;

    /// <summary>
    ///     Count of StoreCaptured opcode executions.
    /// </summary>
    public static long StoreCapturedOpcodeCount;

    /// <summary>
    ///     Count of InitLocals opcode executions.
    /// </summary>
    public static long InitLocalsOpcodeCount;

    /// <summary>
    ///     Count of binary operator specialization fast-path hits.
    /// </summary>
    public static long BinaryOperatorSpecializationHitCount;

    /// <summary>
    ///     Count of binary operator specialization fallback path hits.
    /// </summary>
    public static long BinaryOperatorSpecializationMissCount;

    /// <summary>
    ///     Count of unary operator specialization fast-path hits.
    /// </summary>
    public static long UnaryOperatorSpecializationHitCount;

    /// <summary>
    ///     Count of unary operator specialization fallback path hits.
    /// </summary>
    public static long UnaryOperatorSpecializationMissCount;

    /// <summary>
    ///     Count of comparison specialization fast-path hits.
    /// </summary>
    public static long ComparisonSpecializationHitCount;

    /// <summary>
    ///     Count of comparison specialization fallback path hits.
    /// </summary>
    public static long ComparisonSpecializationMissCount;

    /// <summary>
    ///     Count of loop condition specialization hits.
    /// </summary>
    public static long LoopConditionSpecializationCount;

    /// <summary>
    ///     Count of InvokeMember inline cache hits.
    /// </summary>
    public static long InvokeMemberInlineCacheHitCount;

    /// <summary>
    ///     Count of InvokeMember inline cache misses.
    /// </summary>
    public static long InvokeMemberInlineCacheMissCount;

    /// <summary>
    ///     Count of LoadMember inline cache hits.
    /// </summary>
    public static long LoadMemberInlineCacheHitCount;

    /// <summary>
    ///     Count of LoadMember inline cache misses.
    /// </summary>
    public static long LoadMemberInlineCacheMissCount;

    /// <summary>
    ///     Count of LoadMemberNullChecked inline cache hits.
    /// </summary>
    public static long NullCheckInlineCacheHitCount;

    /// <summary>
    ///     Count of LoadMemberNullChecked inline cache misses.
    /// </summary>
    public static long NullCheckInlineCacheMissCount;

    /// <summary>
    ///     The Argument Stack
    /// </summary>
    private readonly Stack<BadObject> m_ArgumentStack = new Stack<BadObject>();

    /// <summary>
    ///     The Context Stack
    /// </summary>
    private readonly Stack<BadRuntimeVirtualStackFrame> m_ContextStack = new Stack<BadRuntimeVirtualStackFrame>();

    // Hot-path cache for TryResolveFrameVariable. The entry is only valid while
    // stack shape (count + top frame) stays unchanged.
    private int m_VariableResolutionCacheStackCount = -1;
    private BadRuntimeVirtualStackFrame? m_VariableResolutionCacheTopFrame;
    private string? m_VariableResolutionCacheName;
    private bool m_VariableResolutionCacheResult;
    private BadRuntimeVirtualStackFrame? m_VariableResolutionCacheFrame;
    private BadSlotInfo? m_VariableResolutionCacheSlotInfo;
    private bool m_VariableResolutionCacheIsCapture;

    /// <summary>
    ///     The Function that is executed by this Virtual Machine
    /// </summary>
    private readonly BadCompiledFunction m_Function;

    /// <summary>
    ///     The Instructions
    /// </summary>
    private BadInstruction[] m_CurrentInstructions;

    /// <summary>
    ///     Indicates if the Virtual Machine should use Operator Overrides.
    /// </summary>
    private readonly bool m_UseOverrides;

    /// <summary>
    ///     The Current Instruction Pointer
    /// </summary>
    private int m_InstructionPointer;

    /// <summary>
    ///     Per-VM polymorphic cache for InvokeMember fast-path call-sites.
    /// </summary>
    private readonly Dictionary<(Type TargetType, string MemberName), BadMethodCallSite> m_InvokeMemberCallSiteCache =
        new Dictionary<(Type TargetType, string MemberName), BadMethodCallSite>();

    /// <summary>
    ///     FIFO list for bounded call-site cache eviction.
    /// </summary>
    private readonly Queue<(Type TargetType, string MemberName)> m_InvokeMemberCallSiteOrder =
        new Queue<(Type TargetType, string MemberName)>();

    /// <summary>
    ///     Maximum number of cached InvokeMember call-sites per VM instance.
    /// </summary>
    private const int InvokeMemberCallSiteCacheLimit = 64;

    /// <summary>
    ///     Per-VM inline cache for LoadMember and LoadMemberNullChecked call-sites.
    /// </summary>
    private readonly Dictionary<(int InstructionIndex, string MemberName), (BadObject Target, BadObjectReference Reference)>
        m_LoadMemberInlineCache =
            new Dictionary<(int InstructionIndex, string MemberName), (BadObject Target, BadObjectReference Reference)>();

    /// <summary>
    ///     FIFO list for bounded member inline cache eviction.
    /// </summary>
    private readonly Queue<(int InstructionIndex, string MemberName)> m_LoadMemberInlineCacheOrder =
        new Queue<(int InstructionIndex, string MemberName)>();

    /// <summary>
    ///     Maximum number of LoadMember inline cache entries per VM instance.
    /// </summary>
    private const int LoadMemberInlineCacheLimit = 128;

    /// <summary>
    ///     Per-VM inline cache for null-checked InvokeMember property existence checks.
    /// </summary>
    private readonly Dictionary<(int InstructionIndex, string MemberName), (BadObject Target, bool HasMember)>
        m_InvokeMemberNullCheckCache =
            new Dictionary<(int InstructionIndex, string MemberName), (BadObject Target, bool HasMember)>();

    /// <summary>
    ///     FIFO list for bounded null-check cache eviction.
    /// </summary>
    private readonly Queue<(int InstructionIndex, string MemberName)> m_InvokeMemberNullCheckCacheOrder =
        new Queue<(int InstructionIndex, string MemberName)>();

    /// <summary>
    ///     Maximum number of null-check cache entries per VM instance.
    /// </summary>
    private const int InvokeMemberNullCheckCacheLimit = 128;

    // -------------------------------------------------------------------------
    // Phase C2 – Escape Analysis: per-VM mutable scratch number for transient
    //            arithmetic results that don't escape to variables or callers.
    // -------------------------------------------------------------------------

    /// <summary>
    ///     Mutable <see cref="BadNumber"/> reused for arithmetic results whose
    ///     <see cref="BadInstructionFlags.TransientResult"/> flag is set.
    ///     Avoids a heap allocation for intermediate compound-expression values
    ///     like <c>(a + b) * c</c> where <c>a+b</c> is immediately consumed.
    /// </summary>
    private readonly BadScratchNumber m_ScratchNumber = new BadScratchNumber(0m);

    /// <summary>
    ///     Counter of scratch-number reuses (diagnostic, not performance-critical).
    /// </summary>
    public static long EscapeAnalysisScratchHitCount;

    /// <summary>
    ///     Counter of normal (non-transient) arithmetic allocations avoided by
    ///     the integer cache (Phase C1).
    /// </summary>
    public static long EscapeAnalysisIntCacheHitCount;

    private void InitializeFunctionFrame(BadRuntimeVirtualStackFrame frame, BadCompiledFunction function, BadObject[]? args = null)
    {
        if (function.SymbolTable == null || !BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath)
        {
            return;
        }

        frame.SymbolTable = function.SymbolTable;
        frame.LocalSlots = new BadObject[function.SymbolTable.TotalSlotCount];
        frame.SlotPropertyInfos = new BadPropertyInfo[function.SymbolTable.TotalSlotCount];
        frame.SlotAttributes = new BadObject[function.SymbolTable.TotalSlotCount][];
        frame.CaptureReferences = new BadObjectReference[function.SymbolTable.TotalSlotCount];

        string functionName = function.Name?.Text ?? "<compiled>";
        BadObject[] boundParameters =
            BadFunction.BindParameterValues(functionName, function.Parameters, frame.Context, args ?? [], null);

        for (int i = 0; i < function.Parameters.Length; i++)
        {
            BadFunctionParameter parameter = function.Parameters[i];

            if (!function.SymbolTable.TryGetSymbol(parameter.Name, out BadSlotInfo? slotInfo) ||
                slotInfo == null ||
                slotInfo.IsCapture)
            {
                continue;
            }

            frame.LocalSlots[slotInfo.SlotIndex] = boundParameters[i];
            frame.SlotPropertyInfos[slotInfo.SlotIndex] =
                new BadPropertyInfo(BadFunction.GetParameterType(parameter), false);
            frame.SlotAttributes[slotInfo.SlotIndex] = Array.Empty<BadObject>();
        }

        foreach (BadSlotInfo local in function.SymbolTable.Locals)
        {
            frame.LocalSlots[local.SlotIndex] = BadObject.Null;
            frame.SlotPropertyInfos[local.SlotIndex] ??= new BadPropertyInfo(BadAnyPrototype.Instance, false);
            frame.SlotAttributes[local.SlotIndex] ??= Array.Empty<BadObject>();
        }
    }

    private BadRuntimeVirtualStackFrame CreateFunctionFrame(BadCompiledFunction function,
                                                            BadExecutionContext context,
                                                            int returnPointer = -1,
                                                            BadInstruction[]? savedInstructions = null,
                                                            BadObject[]? args = null)
    {
        BadRuntimeVirtualStackFrame frame = new BadRuntimeVirtualStackFrame(context)
                                            {
                                                Function = function,
                                                ReturnPointer = returnPointer,
                                                SavedInstructions = savedInstructions,
                                            };
        InitializeFunctionFrame(frame, function, args);
        return frame;
    }

    private bool TryGetCurrentFrameSlot(string name, out BadRuntimeVirtualStackFrame frame, out BadSlotInfo slotInfo)
    {
        frame = m_ContextStack.Peek();
        slotInfo = null!;

        if (frame.SymbolTable == null || frame.LocalSlots == null)
        {
            return false;
        }

        if (!frame.SymbolTable.TryGetSymbol(name, out BadSlotInfo? info) || info == null || info.IsCapture)
        {
            return false;
        }

        slotInfo = info;
        return true;
    }

    private bool TryGetCurrentFrameCapture(string name, out BadRuntimeVirtualStackFrame frame, out BadSlotInfo slotInfo)
    {
        frame = m_ContextStack.Peek();
        slotInfo = null!;

        if (frame.SymbolTable == null || frame.CaptureReferences == null)
        {
            return false;
        }

        if (!frame.SymbolTable.TryGetSymbol(name, out BadSlotInfo? info) || info == null || !info.IsCapture)
        {
            return false;
        }

        slotInfo = info;
        return true;
    }

    /// <summary>
    ///     Tries to resolve a variable through slot/capture metadata across all active frames.
    ///     This is needed for nested scopes (for/while/body) that do not carry a symbol table.
    /// </summary>
    private bool TryResolveFrameVariable(string name,
                                         out BadRuntimeVirtualStackFrame frame,
                                         out BadSlotInfo slotInfo,
                                         out bool isCapture)
    {
        if (IsVariableResolutionCacheHit(name,
                                         out BadRuntimeVirtualStackFrame? cachedFrame,
                                         out BadSlotInfo? cachedSlotInfo,
                                         out bool cachedIsCapture,
                                         out bool cachedResult))
        {
            if (cachedResult)
            {
                frame = cachedFrame!;
                slotInfo = cachedSlotInfo!;
                isCapture = cachedIsCapture;
                return true;
            }

            frame = null!;
            slotInfo = null!;
            isCapture = false;
            return false;
        }

        frame = null!;
        slotInfo = null!;
        isCapture = false;

        foreach (BadRuntimeVirtualStackFrame candidate in m_ContextStack)
        {
            if (candidate.SymbolTable == null)
            {
                continue;
            }

            if (!candidate.SymbolTable.TryGetSymbol(name, out BadSlotInfo? info) || info == null)
            {
                continue;
            }

            if (info.IsCapture)
            {
                if (candidate.CaptureReferences == null)
                {
                    continue;
                }

                frame = candidate;
                slotInfo = info;
                isCapture = true;
                UpdateVariableResolutionCache(name, frame, slotInfo, isCapture, true);
                return true;
            }

            if (candidate.LocalSlots == null)
            {
                continue;
            }

            frame = candidate;
            slotInfo = info;
            isCapture = false;
            UpdateVariableResolutionCache(name, frame, slotInfo, isCapture, true);
            return true;
        }

        UpdateVariableResolutionCache(name, null, null, false, false);
        return false;
    }

    private bool IsVariableResolutionCacheHit(string name,
                                              out BadRuntimeVirtualStackFrame? frame,
                                              out BadSlotInfo? slotInfo,
                                              out bool isCapture,
                                              out bool result)
    {
        frame = null;
        slotInfo = null;
        isCapture = false;
        result = false;

        if (m_VariableResolutionCacheName == null ||
            !string.Equals(m_VariableResolutionCacheName, name, StringComparison.Ordinal))
        {
            return false;
        }

        if (m_ContextStack.Count != m_VariableResolutionCacheStackCount || m_ContextStack.Count == 0)
        {
            return false;
        }

        if (!ReferenceEquals(m_ContextStack.Peek(), m_VariableResolutionCacheTopFrame))
        {
            return false;
        }

        frame = m_VariableResolutionCacheFrame;
        slotInfo = m_VariableResolutionCacheSlotInfo;
        isCapture = m_VariableResolutionCacheIsCapture;
        result = m_VariableResolutionCacheResult;
        return true;
    }

    private void UpdateVariableResolutionCache(string name,
                                               BadRuntimeVirtualStackFrame? frame,
                                               BadSlotInfo? slotInfo,
                                               bool isCapture,
                                               bool result)
    {
        m_VariableResolutionCacheName = name;
        m_VariableResolutionCacheResult = result;
        m_VariableResolutionCacheFrame = frame;
        m_VariableResolutionCacheSlotInfo = slotInfo;
        m_VariableResolutionCacheIsCapture = isCapture;
        m_VariableResolutionCacheStackCount = m_ContextStack.Count;
        m_VariableResolutionCacheTopFrame = m_ContextStack.Count == 0 ? null : m_ContextStack.Peek();
    }

    private BadObjectReference GetSlotReference(BadRuntimeVirtualStackFrame frame, string name, BadSlotInfo slotInfo)
    {
        frame.SlotReferences ??= new Dictionary<string, BadObjectReference>(StringComparer.Ordinal);

        if (frame.SlotReferences.TryGetValue(name, out BadObjectReference? existing))
        {
            return existing;
        }

        BadObjectReference reference = BadObjectReference.Make($"slot:{name}",
                                                               _ => frame.LocalSlots![slotInfo.SlotIndex],
                                                               (o, p, info, noChangeEvent) =>
                                                               {
                                                                   if (frame.Context.Scope.GetTable().InnerTable.ContainsKey(name))
                                                                   {
                                                                       frame.Context.Scope.GetVariable(name, frame.Context.Scope)
                                                                            .Set(o, p, info, noChangeEvent);
                                                                       frame.LocalSlots![slotInfo.SlotIndex] =
                                                                           frame.Context.Scope.GetVariable(name, frame.Context.Scope)
                                                                                .Dereference(p);
                                                                   }
                                                                   else
                                                                   {
                                                                       BadPropertyInfo propertyInfo =
                                                                           frame.SlotPropertyInfos?[slotInfo.SlotIndex] ??
                                                                           info ??
                                                                           new BadPropertyInfo(BadAnyPrototype.Instance);
                                                                       BadObject currentValue =
                                                                           frame.LocalSlots![slotInfo.SlotIndex];

                                                                       if (currentValue != BadObject.Null &&
                                                                           propertyInfo.IsReadOnly)
                                                                       {
                                                                           throw BadRuntimeException.Create(frame.Context.Scope,
                                                                               $"{name} is read-only",
                                                                               p);
                                                                       }

                                                                       if (propertyInfo.Type != null &&
                                                                           !propertyInfo.Type.IsAssignableFrom(o))
                                                                       {
                                                                           throw BadRuntimeException.Create(frame.Context.Scope,
                                                                               $"Cannot assign object {o.GetType().Name} to property '{name}' of type '{propertyInfo.Type.Name}'",
                                                                               p);
                                                                       }

                                                                       frame.LocalSlots![slotInfo.SlotIndex] = o;
                                                                       frame.SlotPropertyInfos![slotInfo.SlotIndex] = propertyInfo;
                                                                    }
                                                               });

        frame.SlotReferences[name] = reference;
        return reference;
    }

    private BadObjectReference GetCaptureReference(BadRuntimeVirtualStackFrame frame, string name, BadSlotInfo slotInfo)
    {
        if (frame.CaptureReferences == null)
        {
            throw new InvalidOperationException("Capture references are not initialized for the current frame.");
        }

        if (frame.CaptureReferences[slotInfo.SlotIndex] != null)
        {
            return frame.CaptureReferences[slotInfo.SlotIndex]!;
        }

        BadScope? parentScope = frame.Context.Scope.Parent;

        if (parentScope == null)
        {
            return frame.Context.Scope.GetVariable(name, frame.Context.Scope);
        }

        BadObjectReference reference;

        try
        {
            reference = parentScope.GetVariable(name, frame.Context.Scope);
        }
        catch (BadRuntimeException)
        {
            // Fallback: some symbols can be marked as capture although they currently only exist in this scope.
            reference = frame.Context.Scope.GetVariable(name, frame.Context.Scope);
        }

        frame.CaptureReferences[slotInfo.SlotIndex] = reference;
        return reference;
    }

    private bool TryGetCaptureSlotByIndex(BadRuntimeVirtualStackFrame frame, int slotIndex, out BadSlotInfo slotInfo)
    {
        slotInfo = null!;

        if (frame.SymbolTable == null)
        {
            return false;
        }

        // OPTIMIZED: Use direct index lookup instead of iterating Captures
        if (frame.SymbolTable.TryGetSymbolByIndex(slotIndex, out BadSlotInfo? symbol) && 
            symbol != null && 
            symbol.IsCapture)
        {
            slotInfo = symbol;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Creates a new <see cref="BadRuntimeVirtualMachine" /> instance.
    /// </summary>
    /// <param name="instructions">The Instructions to execute.</param>
    /// <param name="useOverrides">Indicates if the Virtual Machine should use Operator Overrides.</param>
    public BadRuntimeVirtualMachine(BadCompiledFunction function,
                                    BadInstruction[] instructions,
                                    bool useOverrides = true)
    {
        m_Function = function;
        m_CurrentInstructions = instructions;
        m_UseOverrides = useOverrides;
    }

    /// <summary>
    /// Resets the global eval instruction counter.
    /// </summary>
    public static void ResetEvalInstructionCounter()
    {
        Interlocked.Exchange(ref EvalInstructionCount, 0);
    }

    /// <summary>
    /// Resets the static inline fast-path counter.
    /// </summary>
    public static void ResetStaticInlineCounter()
    {
        Interlocked.Exchange(ref StaticInlineFastPathCount, 0);
    }

    /// <summary>
    ///     Resets all VM optimization counters.
    /// </summary>
    public static void ResetOptimizationCounters()
    {
        ResetEvalInstructionCounter();
        ResetStaticInlineCounter();

        Interlocked.Exchange(ref InvokeMemberMethodSlotFastPathCount, 0);
        Interlocked.Exchange(ref InvokeMemberCallSitePathCount, 0);
        Interlocked.Exchange(ref LoadMemberPropertyCacheFastPathCount, 0);
        Interlocked.Exchange(ref LoadMemberPropertySlowPathCount, 0);
        Interlocked.Exchange(ref LoadVarSlotPathCount, 0);
        Interlocked.Exchange(ref LoadVarCapturePathCount, 0);
        Interlocked.Exchange(ref LoadVarScopePathCount, 0);
        Interlocked.Exchange(ref LoopMoveNextNativeFastPathCount, 0);
        Interlocked.Exchange(ref LoopMoveNextMethodSlotFastPathCount, 0);
        Interlocked.Exchange(ref LoopMoveNextSlowPathCount, 0);
        Interlocked.Exchange(ref LoopGetCurrentNativeFastPathCount, 0);
        Interlocked.Exchange(ref LoopGetCurrentMethodSlotFastPathCount, 0);
        Interlocked.Exchange(ref LoopGetCurrentSlowPathCount, 0);
        Interlocked.Exchange(ref InvokeStaticInlineFromInvokeFastPathCount, 0);
        Interlocked.Exchange(ref InvokeStaticInlineFromInvokeCompiledFastPathCount, 0);
        Interlocked.Exchange(ref InvokeCompiledFallbackVmPathCount, 0);
        Interlocked.Exchange(ref InvokeFallbackPathCount, 0);
        Interlocked.Exchange(ref InvokeSelfRecursionPathCount, 0);
        Interlocked.Exchange(ref InvokeCompiledSelfRecursionPathCount, 0);
        Interlocked.Exchange(ref InvokeMemberNullCheckedShortCircuitCount, 0);
        Interlocked.Exchange(ref LoadMemberGenericPathCount, 0);
        Interlocked.Exchange(ref GetEnumeratorDirectPathCount, 0);
        Interlocked.Exchange(ref GetEnumeratorMethodPathCount, 0);
        Interlocked.Exchange(ref GetEnumeratorEnumerablePathCount, 0);
        Interlocked.Exchange(ref GetEnumeratorFallbackPathCount, 0);
        Interlocked.Exchange(ref LoadLocalOpcodeCount, 0);
        Interlocked.Exchange(ref StoreLocalOpcodeCount, 0);
        Interlocked.Exchange(ref LoadCapturedOpcodeCount, 0);
        Interlocked.Exchange(ref StoreCapturedOpcodeCount, 0);
        Interlocked.Exchange(ref InitLocalsOpcodeCount, 0);
        Interlocked.Exchange(ref BinaryOperatorSpecializationHitCount, 0);
        Interlocked.Exchange(ref BinaryOperatorSpecializationMissCount, 0);
        Interlocked.Exchange(ref UnaryOperatorSpecializationHitCount, 0);
        Interlocked.Exchange(ref UnaryOperatorSpecializationMissCount, 0);
        Interlocked.Exchange(ref ComparisonSpecializationHitCount, 0);
        Interlocked.Exchange(ref ComparisonSpecializationMissCount, 0);
        Interlocked.Exchange(ref LoopConditionSpecializationCount, 0);
        Interlocked.Exchange(ref InvokeMemberInlineCacheHitCount, 0);
        Interlocked.Exchange(ref InvokeMemberInlineCacheMissCount, 0);
        Interlocked.Exchange(ref LoadMemberInlineCacheHitCount, 0);
        Interlocked.Exchange(ref LoadMemberInlineCacheMissCount, 0);
        Interlocked.Exchange(ref NullCheckInlineCacheHitCount, 0);
        Interlocked.Exchange(ref NullCheckInlineCacheMissCount, 0);
        Interlocked.Exchange(ref EscapeAnalysisScratchHitCount, 0);
        Interlocked.Exchange(ref EscapeAnalysisIntCacheHitCount, 0);
    }

    /// <summary>
    ///     Returns a snapshot of VM optimization counters.
    /// </summary>
    public static Dictionary<string, long> GetOptimizationCounterSnapshot()
    {
        return new Dictionary<string, long>(StringComparer.Ordinal)
               {
                   ["EvalFallback"] = Interlocked.Read(ref EvalInstructionCount),
                   ["StaticInlineFastPath"] = Interlocked.Read(ref StaticInlineFastPathCount),
                   ["InvokeMemberMethodSlotFastPath"] = Interlocked.Read(ref InvokeMemberMethodSlotFastPathCount),
                   ["InvokeMemberCallSitePath"] = Interlocked.Read(ref InvokeMemberCallSitePathCount),
                   ["LoadMemberPropertyCacheFastPath"] = Interlocked.Read(ref LoadMemberPropertyCacheFastPathCount),
                   ["LoadMemberPropertySlowPath"] = Interlocked.Read(ref LoadMemberPropertySlowPathCount),
                   ["LoadVarSlotPath"] = Interlocked.Read(ref LoadVarSlotPathCount),
                   ["LoadVarCapturePath"] = Interlocked.Read(ref LoadVarCapturePathCount),
                   ["LoadVarScopePath"] = Interlocked.Read(ref LoadVarScopePathCount),
                   ["LoopMoveNextNativeFastPath"] = Interlocked.Read(ref LoopMoveNextNativeFastPathCount),
                   ["LoopMoveNextMethodSlotFastPath"] = Interlocked.Read(ref LoopMoveNextMethodSlotFastPathCount),
                   ["LoopMoveNextSlowPath"] = Interlocked.Read(ref LoopMoveNextSlowPathCount),
                   ["LoopGetCurrentNativeFastPath"] = Interlocked.Read(ref LoopGetCurrentNativeFastPathCount),
                   ["LoopGetCurrentMethodSlotFastPath"] = Interlocked.Read(ref LoopGetCurrentMethodSlotFastPathCount),
                   ["LoopGetCurrentSlowPath"] = Interlocked.Read(ref LoopGetCurrentSlowPathCount),
                   ["InvokeStaticInlineFromInvokeFastPath"] = Interlocked.Read(ref InvokeStaticInlineFromInvokeFastPathCount),
                   ["InvokeStaticInlineFromInvokeCompiledFastPath"] = Interlocked.Read(ref InvokeStaticInlineFromInvokeCompiledFastPathCount),
                   ["InvokeCompiledFallbackVmPath"] = Interlocked.Read(ref InvokeCompiledFallbackVmPathCount),
                   ["InvokeFallbackPath"] = Interlocked.Read(ref InvokeFallbackPathCount),
                   ["InvokeSelfRecursionPath"] = Interlocked.Read(ref InvokeSelfRecursionPathCount),
                   ["InvokeCompiledSelfRecursionPath"] = Interlocked.Read(ref InvokeCompiledSelfRecursionPathCount),
                   ["InvokeMemberNullCheckedShortCircuit"] = Interlocked.Read(ref InvokeMemberNullCheckedShortCircuitCount),
                   ["LoadMemberGenericPath"] = Interlocked.Read(ref LoadMemberGenericPathCount),
                   ["GetEnumeratorDirectPath"] = Interlocked.Read(ref GetEnumeratorDirectPathCount),
                   ["GetEnumeratorMethodPath"] = Interlocked.Read(ref GetEnumeratorMethodPathCount),
                   ["GetEnumeratorEnumerablePath"] = Interlocked.Read(ref GetEnumeratorEnumerablePathCount),
                   ["GetEnumeratorFallbackPath"] = Interlocked.Read(ref GetEnumeratorFallbackPathCount),
                   ["LoadLocalOpcode"] = Interlocked.Read(ref LoadLocalOpcodeCount),
                   ["StoreLocalOpcode"] = Interlocked.Read(ref StoreLocalOpcodeCount),
                   ["LoadCapturedOpcode"] = Interlocked.Read(ref LoadCapturedOpcodeCount),
                   ["StoreCapturedOpcode"] = Interlocked.Read(ref StoreCapturedOpcodeCount),
                   ["InitLocalsOpcode"] = Interlocked.Read(ref InitLocalsOpcodeCount),
                   ["BinaryOperatorSpecializationHit"] = Interlocked.Read(ref BinaryOperatorSpecializationHitCount),
                   ["BinaryOperatorSpecializationMiss"] = Interlocked.Read(ref BinaryOperatorSpecializationMissCount),
                   ["UnaryOperatorSpecializationHit"] = Interlocked.Read(ref UnaryOperatorSpecializationHitCount),
                   ["UnaryOperatorSpecializationMiss"] = Interlocked.Read(ref UnaryOperatorSpecializationMissCount),
                   ["ComparisonSpecializationHit"] = Interlocked.Read(ref ComparisonSpecializationHitCount),
                   ["ComparisonSpecializationMiss"] = Interlocked.Read(ref ComparisonSpecializationMissCount),
                   ["LoopConditionSpecialization"] = Interlocked.Read(ref LoopConditionSpecializationCount),
                   ["InvokeMemberInlineCacheHit"] = Interlocked.Read(ref InvokeMemberInlineCacheHitCount),
                   ["InvokeMemberInlineCacheMiss"] = Interlocked.Read(ref InvokeMemberInlineCacheMissCount),
                   ["LoadMemberInlineCacheHit"] = Interlocked.Read(ref LoadMemberInlineCacheHitCount),
                   ["LoadMemberInlineCacheMiss"] = Interlocked.Read(ref LoadMemberInlineCacheMissCount),
                   ["NullCheckInlineCacheHit"] = Interlocked.Read(ref NullCheckInlineCacheHitCount),
                   ["NullCheckInlineCacheMiss"] = Interlocked.Read(ref NullCheckInlineCacheMissCount),
                   ["EscapeAnalysisScratchHit"] = Interlocked.Read(ref EscapeAnalysisScratchHitCount),
                   ["EscapeAnalysisIntCacheHit"] = Interlocked.Read(ref EscapeAnalysisIntCacheHitCount),
               };
    }

    /// <summary>
    /// Pops the current context frame and restores the caller instruction stream when the frame
    /// was created by the static inline fast-path.
    /// </summary>
    private BadRuntimeVirtualStackFrame PopContextFrame()
    {
        BadRuntimeVirtualStackFrame frame = m_ContextStack.Pop();

        if (frame.SavedInstructions != null)
        {
            m_CurrentInstructions = frame.SavedInstructions;
        }

        return frame;
    }

    /// <summary>
    /// Conservative heuristic for the static inline fast-path.
    /// Only straight-line static compiled functions without explicit this/base loads are inlined.
    /// </summary>
    private static bool CanUseStaticInlineFastPath(BadCompiledFunction func)
    {
        if (func.Instructions.Length == 0)
        {
            return false;
        }

        foreach (BadInstruction instr in func.Instructions)
        {
            if (instr.OpCode == BadOpCode.LoadVar &&
                instr.Arguments.Length > 0 &&
                instr.Arguments[0] is string varName &&
                (varName == "this" || varName == "base"))
            {
                return false;
            }

            switch (instr.OpCode)
            {
                case BadOpCode.JumpRelative:
                case BadOpCode.JumpRelativeIfFalse:
                case BadOpCode.JumpRelativeIfTrue:
                case BadOpCode.JumpRelativeIfNotNull:
                case BadOpCode.JumpRelativeIfNull:
                case BadOpCode.SetBreakPointer:
                case BadOpCode.SetContinuePointer:
                case BadOpCode.SetThrowPointer:
                case BadOpCode.Break:
                case BadOpCode.Continue:
                case BadOpCode.Throw:
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns a cached method call-site for the target/member pair or creates one.
    /// </summary>
    private BadMethodCallSite GetOrCreateInvokeMemberCallSite(BadObject target, string memberName, out bool cacheHit)
    {
        (Type TargetType, string MemberName) key = (target.GetType(), memberName);

        if (m_InvokeMemberCallSiteCache.TryGetValue(key, out BadMethodCallSite? callSite))
        {
            cacheHit = true;
            return callSite;
        }

        cacheHit = false;

        callSite = new BadMethodCallSite(memberName);
        m_InvokeMemberCallSiteCache[key] = callSite;
        m_InvokeMemberCallSiteOrder.Enqueue(key);

        while (m_InvokeMemberCallSiteOrder.Count > InvokeMemberCallSiteCacheLimit)
        {
            (Type TargetType, string MemberName) oldest = m_InvokeMemberCallSiteOrder.Dequeue();
            m_InvokeMemberCallSiteCache.Remove(oldest);
        }

        return callSite;
    }

    private bool TryGetLoadMemberInlineCache(int instructionIndex,
                                             string memberName,
                                             BadObject target,
                                             out BadObjectReference reference)
    {
        reference = null!;

        (int InstructionIndex, string MemberName) key = (instructionIndex, memberName);

        if (!m_LoadMemberInlineCache.TryGetValue(key, out (BadObject Target, BadObjectReference Reference) entry))
        {
            return false;
        }

        if (!ReferenceEquals(entry.Target, target))
        {
            return false;
        }

        reference = entry.Reference;
        return true;
    }

    private void SetLoadMemberInlineCache(int instructionIndex, string memberName, BadObject target, BadObjectReference reference)
    {
        (int InstructionIndex, string MemberName) key = (instructionIndex, memberName);

        m_LoadMemberInlineCache[key] = (target, reference);
        m_LoadMemberInlineCacheOrder.Enqueue(key);

        while (m_LoadMemberInlineCacheOrder.Count > LoadMemberInlineCacheLimit)
        {
            (int InstructionIndex, string MemberName) oldest = m_LoadMemberInlineCacheOrder.Dequeue();
            m_LoadMemberInlineCache.Remove(oldest);
        }
    }

    private bool TryGetInvokeMemberNullCheckCache(int instructionIndex,
                                                  string memberName,
                                                  BadObject target,
                                                  out bool hasMember)
    {
        hasMember = false;

        (int InstructionIndex, string MemberName) key = (instructionIndex, memberName);

        if (!m_InvokeMemberNullCheckCache.TryGetValue(key, out (BadObject Target, bool HasMember) entry))
        {
            return false;
        }

        if (!ReferenceEquals(entry.Target, target))
        {
            return false;
        }

        hasMember = entry.HasMember;
        return true;
    }

    private void SetInvokeMemberNullCheckCache(int instructionIndex, string memberName, BadObject target, bool hasMember)
    {
        (int InstructionIndex, string MemberName) key = (instructionIndex, memberName);

        m_InvokeMemberNullCheckCache[key] = (target, hasMember);
        m_InvokeMemberNullCheckCacheOrder.Enqueue(key);

        while (m_InvokeMemberNullCheckCacheOrder.Count > InvokeMemberNullCheckCacheLimit)
        {
            (int InstructionIndex, string MemberName) oldest = m_InvokeMemberNullCheckCacheOrder.Dequeue();
            m_InvokeMemberNullCheckCache.Remove(oldest);
        }
    }

    private bool TrySpecializeArithmetic(BadOpCode opCode,
                                              BadObject left,
                                              BadObject right,
                                              bool isTransient,
                                              out BadObject result)
    {
        result = BadObject.Null;

        if (left is IBadNumber leftNumber && right is IBadNumber rightNumber)
        {
            // Phase C1: use integer cache instead of allocating a new BadNumber
            decimal numericResult = opCode switch
                                    {
                                        BadOpCode.Add => leftNumber.Value + rightNumber.Value,
                                        BadOpCode.Sub => leftNumber.Value - rightNumber.Value,
                                        BadOpCode.Mul => leftNumber.Value * rightNumber.Value,
                                        BadOpCode.Div => leftNumber.Value / rightNumber.Value,
                                        BadOpCode.Mod => leftNumber.Value % rightNumber.Value,
                                        _ => 0m
                                    };

            if (opCode is BadOpCode.Add or BadOpCode.Sub or BadOpCode.Mul or BadOpCode.Div or BadOpCode.Mod)
            {
                if (isTransient && BadNativeOptimizationSettings.Instance.UseEscapeAnalysis)
                {
                    // Phase C2: reuse the per-VM scratch number; no allocation
                    Interlocked.Increment(ref EscapeAnalysisScratchHitCount);
                    m_ScratchNumber.ScratchValue = numericResult;
                    result = m_ScratchNumber;
                }
                else
                {
                    result = BadNumber.Get(numericResult);
                }

                return true;
            }

            result = BadObject.Null;
            return false;
        }

        if (opCode == BadOpCode.Add && left is IBadString leftString && right is IBadString rightString)
        {
            result = leftString.Value + rightString.Value;
            return true;
        }

        return false;
    }

    private static bool TrySpecializeComparison(BadOpCode opCode,
                                                BadObject left,
                                                BadObject right,
                                                out BadObject result)
    {
        result = BadObject.Null;

        if (left is IBadNumber leftNumber && right is IBadNumber rightNumber)
        {
            bool numericResult = opCode switch
                                 {
                                     BadOpCode.Equals => leftNumber.Value == rightNumber.Value,
                                     BadOpCode.NotEquals => leftNumber.Value != rightNumber.Value,
                                     BadOpCode.Greater => leftNumber.Value > rightNumber.Value,
                                     BadOpCode.GreaterEquals => leftNumber.Value >= rightNumber.Value,
                                     BadOpCode.Less => leftNumber.Value < rightNumber.Value,
                                     BadOpCode.LessEquals => leftNumber.Value <= rightNumber.Value,
                                     _ => false
                                 };

            if (opCode is BadOpCode.Equals or BadOpCode.NotEquals or BadOpCode.Greater or BadOpCode.GreaterEquals or BadOpCode.Less or BadOpCode.LessEquals)
            {
                result = numericResult;
                return true;
            }
        }

        if (opCode is BadOpCode.Equals or BadOpCode.NotEquals)
        {
            if (left is IBadString leftString && right is IBadString rightString)
            {
                bool equals = string.Equals(leftString.Value, rightString.Value, StringComparison.Ordinal);
                result = opCode == BadOpCode.Equals ? equals : !equals;
                return true;
            }

            if (left is IBadBoolean leftBool && right is IBadBoolean rightBool)
            {
                bool equals = leftBool.Value == rightBool.Value;
                result = opCode == BadOpCode.Equals ? equals : !equals;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Executes a step in the Virtual Machine.
    /// </summary>
    /// <param name="ctx">The Scope to execute in.</param>
    /// <returns>Execution Enumeration</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if an error occurs during execution.</exception>
    /// <exception cref="BadRuntimeErrorException">>Gets thrown if an error occurs during execution.</exception>
    /// <exception cref="ArgumentOutOfRangeException">>Gets thrown if the instruction pointer is out of range.</exception>
    private IEnumerable<BadObject> ExecuteStep(BadExecutionContext ctx)
    {
        BadInstruction instr = m_CurrentInstructions[m_InstructionPointer];

        if (BadDebugger.IsAttached)
        {
            BadDebugger.Step(new BadDebuggerStep(ctx, instr.Position, instr));
        }

        m_InstructionPointer++;

        switch (instr.OpCode)
        {
            case BadOpCode.Nop:

                break;
            case BadOpCode.Dup:
                m_ArgumentStack.Push(m_ArgumentStack.Peek());

                break;
            case BadOpCode.Pop:
                m_ArgumentStack.Pop();

                break;
            case BadOpCode.AquireLock:
            {
                BadObject lockObj = m_ArgumentStack.Pop()
                                                   .Dereference(instr.Position);

                if (lockObj is not BadArray && lockObj is not BadTable && lockObj is BadClass)
                {
                    throw new BadRuntimeException("Lock object must be of type Array, Object or Class",
                                                  instr.Position
                                                 );
                }

                while (!BadLockList.Instance.TryAquire(lockObj))
                {
                    yield return BadObject.Null;
                }

                break;
            }
            case BadOpCode.ArrayInit:
            {
                int length = (int)instr.Arguments[0];
                List<BadObject> arr = new List<BadObject>(length);

                // Pre-size and write by index to avoid O(n^2) Insert(0, ...)
                for (int i = 0; i < length; i++)
                {
                    arr.Add(BadObject.Null);
                }

                for (int i = length - 1; i >= 0; i--)
                {
                    arr[i] = m_ArgumentStack.Pop().Dereference(instr.Position);
                }

                m_ArgumentStack.Push(new BadArray(arr));

                break;
            }
            case BadOpCode.TableInit:
            {
                int length = (int)instr.Arguments[0];
                Dictionary<string, BadObject> arr = new Dictionary<string, BadObject>();

                for (int i = 0; i < length; i++)
                {
                    BadObject val = m_ArgumentStack.Pop()
                                                   .Dereference(instr.Position);

                    BadObject key = m_ArgumentStack.Pop()
                                                   .Dereference(instr.Position);

                    if (key is not IBadString s)
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "Invalid Property Key", instr.Position);
                    }

                    arr.Add(s.Value, val);
                }

                m_ArgumentStack.Push(new BadTable(arr));

                break;
            }
            case BadOpCode.HasProperty:
            {
                BadObject key = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);

                BadObject obj = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);
                BadObject? result = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadInExpression.InWithOverride(ctx, key, obj, instr.Position))
                    {
                        result = o;
                    }
                }
                else
                {
                    result = BadInExpression.In(ctx, key, obj);
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.Invoke:
            {
                BadObject func = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                int argCount = (int)instr.Arguments[0];
                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop()
                                             .Dereference(instr.Position);
                }

                BadObject r = BadObject.Null;

                if (m_Function == func) //Invoke Self
                {
                    Interlocked.Increment(ref InvokeSelfRecursionPathCount);
                    m_ContextStack.Push(CreateFunctionFrame(m_Function,
                                                            m_Function.CreateExecutionContext(ctx, args),
                                                            m_InstructionPointer,
                                                            null,
                                                            args));
                    m_InstructionPointer = 0;

                    break;
                }

                // Phase 4C: Static Inline Fast-Path
                // For static BadCompiledFunctions, inline the call directly into this VM
                // instead of spawning a new BadRuntimeVirtualMachine.
                if (BadNativeOptimizationSettings.Instance.UseStaticMethodSpecialization &&
                    func is BadCompiledFunction staticCompiledFunc &&
                    staticCompiledFunc.IsStatic &&
                    CanUseStaticInlineFastPath(staticCompiledFunc))
                {
                    Interlocked.Increment(ref StaticInlineFastPathCount);
                    Interlocked.Increment(ref InvokeStaticInlineFromInvokeFastPathCount);

                    BadExecutionContext inlineCtx = staticCompiledFunc.CreateInlineExecutionContext(ctx, args);
                    m_ContextStack.Push(CreateFunctionFrame(staticCompiledFunc,
                                                            inlineCtx,
                                                            m_InstructionPointer,
                                                            m_CurrentInstructions,
                                                            args));
                    m_CurrentInstructions = staticCompiledFunc.Instructions;
                    m_InstructionPointer = 0;

                    break;
                }

                Interlocked.Increment(ref InvokeFallbackPathCount);
                foreach (BadObject o in BadInvocationExpression.Invoke(func, args, instr.Position, ctx))
                {
                    r = o;

                    yield return o;
                }

                m_ArgumentStack.Push(r);

                break;
            }
            case BadOpCode.InvokeCompiled:
            {
                // Phase 5.2: Compiled-Call-Fastpath
                // Direct invocation of a known compiled function
                int argCount = (int)instr.Arguments[0];
                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop()
                                             .Dereference(instr.Position);
                }

                BadObject compiledFuncObj = m_ArgumentStack.Pop()
                                                           .Dereference(instr.Position);

                if (compiledFuncObj is not BadCompiledFunction compiledFunc)
                {
                    throw BadRuntimeException.Create(ctx.Scope, 
                        "InvokeCompiled target must be a BadCompiledFunction", 
                        instr.Position);
                }

                BadObject r = BadObject.Null;

                // Self-recursion check
                if (m_Function == compiledFuncObj)
                {
                    Interlocked.Increment(ref InvokeCompiledSelfRecursionPathCount);
                    m_ContextStack.Push(CreateFunctionFrame(m_Function,
                                                            m_Function.CreateExecutionContext(ctx, args),
                                                            m_InstructionPointer,
                                                            null,
                                                            args));
                    m_InstructionPointer = 0;

                    break;
                }

                // Static inline fast-path
                if (BadNativeOptimizationSettings.Instance.UseStaticMethodSpecialization &&
                    compiledFunc.IsStatic &&
                    CanUseStaticInlineFastPath(compiledFunc))
                {
                    Interlocked.Increment(ref StaticInlineFastPathCount);
                    Interlocked.Increment(ref InvokeStaticInlineFromInvokeCompiledFastPathCount);

                    BadExecutionContext inlineCtx = compiledFunc.CreateInlineExecutionContext(ctx, args);
                    m_ContextStack.Push(CreateFunctionFrame(compiledFunc,
                                                            inlineCtx,
                                                            m_InstructionPointer,
                                                            m_CurrentInstructions,
                                                            args));
                    m_CurrentInstructions = compiledFunc.Instructions;
                    m_InstructionPointer = 0;

                    break;
                }

                // Fallback: create new VM for the function
                Interlocked.Increment(ref InvokeCompiledFallbackVmPathCount);
                var vm = new BadRuntimeVirtualMachine(compiledFunc, compiledFunc.Instructions);
                foreach (BadObject o in vm.Execute(compiledFunc.CreateExecutionContext(ctx, args), args))
                {
                    r = o;
                    yield return o;
                }

                m_ArgumentStack.Push(r);

                break;
            }
            case BadOpCode.InvokeMember:
            {
                // Phase 5.3: Method-call fast-path
                // Stack layout before execution: [.., arg1, arg2, ..., targetObject]
                int argCount = (int)instr.Arguments[0];
                string memberName = (string)instr.Arguments[1];
                bool nullChecked = (bool)instr.Arguments[2];

                BadObject target = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);
                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop()
                                             .Dereference(instr.Position);
                }

                if (nullChecked)
                {
                    bool hasProperty;

                    if (BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache &&
                        BadNativeOptimizationSettings.Instance.UseInlineCaching &&
                        TryGetInvokeMemberNullCheckCache(m_InstructionPointer - 1, memberName, target, out bool cachedHasProperty))
                    {
                        Interlocked.Increment(ref NullCheckInlineCacheHitCount);
                        hasProperty = cachedHasProperty;
                    }
                    else
                    {
                        if (BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache)
                        {
                            Interlocked.Increment(ref NullCheckInlineCacheMissCount);
                        }

                        hasProperty = target.HasProperty(memberName, ctx.Scope);

                        if (BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache &&
                            BadNativeOptimizationSettings.Instance.UseInlineCaching)
                        {
                            SetInvokeMemberNullCheckCache(m_InstructionPointer - 1, memberName, target, hasProperty);
                        }
                    }

                    if (!hasProperty)
                    {
                        Interlocked.Increment(ref InvokeMemberNullCheckedShortCircuitCount);
                        m_ArgumentStack.Push(BadObject.Null);
                        break;
                    }
                }

                BadObject result = BadObject.Null;

                // Fast path: BadClass with compiled method slot (VTable-style access, AP3)
                if (BadNativeOptimizationSettings.Instance.UseMethodSlotFastPath &&
                    target is BadClass cls &&
                    cls.TryGetMethodSlot(memberName, out BadObject? slotMethod) &&
                    slotMethod is BadFunction slotFn)
                {
                    Interlocked.Increment(ref InvokeMemberMethodSlotFastPathCount);
                    foreach (BadObject o in slotFn.Invoke(args, ctx))
                    {
                        result = o;
                        yield return o;
                    }
                }
                else
                {
                    Interlocked.Increment(ref InvokeMemberCallSitePathCount);
                    if (BadNativeOptimizationSettings.Instance.UseInlineCaching)
                    {
                        BadMethodCallSite callSite = GetOrCreateInvokeMemberCallSite(target, memberName, out bool cacheHit);

                        if (cacheHit)
                        {
                            Interlocked.Increment(ref InvokeMemberInlineCacheHitCount);
                        }
                        else
                        {
                            Interlocked.Increment(ref InvokeMemberInlineCacheMissCount);
                        }

                        foreach (BadObject o in callSite.Invoke(target, args, instr.Position, ctx))
                        {
                            result = o;
                            yield return o;
                        }
                    }
                    else
                    {
                        BadObject method = target.GetProperty(memberName, ctx.Scope).Dereference(instr.Position);

                        foreach (BadObject o in BadInvocationExpression.Invoke(method, args, instr.Position, ctx))
                        {
                            result = o;
                            yield return o;
                        }
                    }
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.New:
            {
                BadObject func = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);

                if (func is not BadClassPrototype ptype)
                {
                    throw new BadRuntimeException("Cannot create object from non-class type", instr.Position);
                }

                int argCount = (int)instr.Arguments[0];
                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop()
                                             .Dereference(instr.Position);
                }

                BadObject r = BadObject.Null;

                foreach (BadObject o in BadNewExpression.CreateObject(ptype, ctx, args, instr.Position))
                {
                    r = o;

                    yield return o;
                }

                m_ArgumentStack.Push(r);

                break;
            }
            case BadOpCode.Range:
            {
                BadObject start = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject end = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);

                if (start is not IBadNumber sn || end is not IBadNumber en)
                {
                    throw new BadRuntimeException("Range start and end must be numbers", instr.Position);
                }

                m_ArgumentStack.Push(new BadInteropEnumerator(BadRangeExpression.Range(sn.Value, en.Value)
                                                                  .GetEnumerator()
                                                             )
                                    );

                break;
            }
            case BadOpCode.ReleaseLock:
            {
                BadObject lockObj = m_ArgumentStack.Pop()
                                                   .Dereference(instr.Position);

                if (lockObj is not BadArray && lockObj is not BadTable && lockObj is BadClass)
                {
                    throw new BadRuntimeException("Lock object must be of type Array, Object or Class",
                                                  instr.Position
                                                 );
                }

                BadLockList.Instance.Release(lockObj);

                break;
            }
            case BadOpCode.DefVar:
            {
                string name = (string)instr.Arguments[0];
                bool isReadOnly = (bool)instr.Arguments[1];
                BadExpression[] attributes = instr.Arguments.Length > 2
                                                 ? (BadExpression[])instr.Arguments[2]
                                                 : Array.Empty<BadExpression>();
                List<BadObject> computedAttributes = new List<BadObject>();
                BadVariableDefinitionExpression variableExpression =
                    new BadVariableDefinitionExpression(name, instr.Position, null, isReadOnly);
                variableExpression.SetAttributes(attributes);

                foreach (BadObject o in variableExpression.EvaluateAttributes(ctx, computedAttributes))
                {
                    yield return o;
                }

                if (TryResolveFrameVariable(name,
                                            out BadRuntimeVirtualStackFrame resolvedFrame,
                                            out BadSlotInfo resolvedSlotInfo,
                                            out bool resolvedIsCapture) &&
                    !resolvedIsCapture)
                {
                    bool materializeClosureScope =
                        resolvedFrame.Function?.RequiresClosureScopeMaterialization == true;

                    if (materializeClosureScope &&
                        !ctx.Scope.HasLocal(name, ctx.Scope, false))
                    {
                        ctx.Scope.DefineVariable(name,
                                                 BadObject.Null,
                                                 ctx.Scope,
                                                 new BadPropertyInfo(BadAnyPrototype.Instance, isReadOnly),
                                                 computedAttributes.ToArray()
                                                );
                    }

                    resolvedFrame.LocalSlots![resolvedSlotInfo.SlotIndex] = BadObject.Null;
                    resolvedFrame.SlotPropertyInfos![resolvedSlotInfo.SlotIndex] =
                        new BadPropertyInfo(BadAnyPrototype.Instance, isReadOnly);
                    resolvedFrame.SlotAttributes![resolvedSlotInfo.SlotIndex] = computedAttributes.ToArray();
                    m_ArgumentStack.Push(GetSlotReference(resolvedFrame, name, resolvedSlotInfo));
                }
                else
                {
                    ctx.Scope.DefineVariable(name,
                                             BadObject.Null,
                                             ctx.Scope,
                                             new BadPropertyInfo(BadAnyPrototype.Instance, isReadOnly),
                                             computedAttributes.ToArray()
                                            );
                    m_ArgumentStack.Push(ctx.Scope.GetVariable(name));
                }

                break;
            }
            case BadOpCode.DefVarTyped:

            {
                string name = (string)instr.Arguments[0];
                bool isReadOnly = (bool)instr.Arguments[1];
                BadExpression[] attributes = instr.Arguments.Length > 2
                                                 ? (BadExpression[])instr.Arguments[2]
                                                 : Array.Empty<BadExpression>();
                BadClassPrototype type = (BadClassPrototype)m_ArgumentStack.Pop()
                                                                           .Dereference(instr.Position);

                if (type == BadVoidPrototype.Instance)
                {
                    throw BadRuntimeException.Create(ctx.Scope,
                                                     "Cannot declare a variable of type 'void'",
                                                     instr.Position
                                                    );
                }

                List<BadObject> computedAttributes = new List<BadObject>();
                BadVariableDefinitionExpression variableExpression =
                    new BadVariableDefinitionExpression(name, instr.Position, new BadConstantExpression(instr.Position, type), isReadOnly);
                variableExpression.SetAttributes(attributes);

                foreach (BadObject o in variableExpression.EvaluateAttributes(ctx, computedAttributes))
                {
                    yield return o;
                }

                if (TryResolveFrameVariable(name,
                                            out BadRuntimeVirtualStackFrame resolvedFrame,
                                            out BadSlotInfo resolvedSlotInfo,
                                            out bool resolvedIsCapture) &&
                    !resolvedIsCapture)
                {
                    bool materializeClosureScope =
                        resolvedFrame.Function?.RequiresClosureScopeMaterialization == true;

                    if (materializeClosureScope &&
                        !ctx.Scope.HasLocal(name, ctx.Scope, false))
                    {
                        ctx.Scope.DefineVariable(name,
                                                 BadObject.Null,
                                                 ctx.Scope,
                                                 new BadPropertyInfo(type, isReadOnly),
                                                 computedAttributes.ToArray()
                                                );
                    }

                    resolvedFrame.LocalSlots![resolvedSlotInfo.SlotIndex] = BadObject.Null;
                    resolvedFrame.SlotPropertyInfos![resolvedSlotInfo.SlotIndex] = new BadPropertyInfo(type, isReadOnly);
                    resolvedFrame.SlotAttributes![resolvedSlotInfo.SlotIndex] = computedAttributes.ToArray();
                    m_ArgumentStack.Push(GetSlotReference(resolvedFrame, name, resolvedSlotInfo));
                }
                else
                {
                    ctx.Scope.DefineVariable(name,
                                             BadObject.Null,
                                             ctx.Scope,
                                             new BadPropertyInfo(type, isReadOnly),
                                             computedAttributes.ToArray()
                                            );
                    m_ArgumentStack.Push(ctx.Scope.GetVariable(name));
                }

                break;
            }
            case BadOpCode.LoadVar:
            {
                string variableName = (string)instr.Arguments[0];

                if (instr.Arguments.Length > 1 && instr.Arguments[1] is int genericArgCount && genericArgCount != 0)
                {
                    BadObject item;

                    if (TryResolveFrameVariable(variableName,
                                                out BadRuntimeVirtualStackFrame resolvedFrame,
                                                out BadSlotInfo resolvedSlotInfo,
                                                out bool isCapture))
                    {
                        if (isCapture && ctx.Scope.HasLocal(variableName, ctx.Scope, false))
                        {
                            item = ctx.Scope.GetVariable(variableName).Dereference(instr.Position);
                        }
                        else
                        {
                            item = isCapture
                                       ? GetCaptureReference(resolvedFrame, variableName, resolvedSlotInfo).Dereference(instr.Position)
                                       : resolvedFrame.LocalSlots![resolvedSlotInfo.SlotIndex];
                        }
                    }
                    else
                    {
                        item = ctx.Scope.GetVariable(variableName).Dereference(instr.Position);
                    }

                    if (item is not IBadGenericObject genItem)
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "Variable is not a generic object", instr.Position);
                    }

                    BadObject[] genericArgs = new BadObject[genericArgCount];

                    for (int i = genericArgCount - 1; i >= 0; i--)
                    {
                        genericArgs[i] = m_ArgumentStack.Pop()
                                                        .Dereference(instr.Position);
                    }

                    m_ArgumentStack.Push(genItem.CreateGeneric(genericArgs));
                }
                else
                {
                    if (TryResolveFrameVariable(variableName,
                                                out BadRuntimeVirtualStackFrame resolvedFrame,
                                                out BadSlotInfo resolvedSlotInfo,
                                                out bool isCapture))
                    {
                        if (isCapture)
                        {
                            if (ctx.Scope.HasLocal(variableName, ctx.Scope, false))
                            {
                                Interlocked.Increment(ref LoadVarScopePathCount);
                                m_ArgumentStack.Push(ctx.Scope.GetVariable(variableName));
                            }
                            else
                            {
                                Interlocked.Increment(ref LoadVarCapturePathCount);
                                m_ArgumentStack.Push(GetCaptureReference(resolvedFrame, variableName, resolvedSlotInfo));
                            }
                        }
                        else
                        {
                            Interlocked.Increment(ref LoadVarSlotPathCount);
                            m_ArgumentStack.Push(GetSlotReference(resolvedFrame, variableName, resolvedSlotInfo));
                        }
                    }
                    else
                    {
                        Interlocked.Increment(ref LoadVarScopePathCount);
                        m_ArgumentStack.Push(ctx.Scope.GetVariable(variableName));
                    }
                }

                break;
            }
            case BadOpCode.LoadMember:
            {
                if (instr.Arguments.Length > 1 && instr.Arguments[1] is int genericArgCount && genericArgCount != 0)
                {
                    Interlocked.Increment(ref LoadMemberGenericPathCount);
                    BadObject left =
                        m_ArgumentStack.Pop()
                                       .Dereference(instr.Position)
                                       .GetProperty((string)instr.Arguments[0], ctx.Scope)
                                       .Dereference(instr.Position);

                    if (left is not IBadGenericObject genItem)
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "Variable is not a generic object", instr.Position);
                    }

                    BadObject[] genericArgs = new BadObject[genericArgCount];

                    for (int i = genericArgCount - 1; i >= 0; i--)
                    {
                        genericArgs[i] = m_ArgumentStack.Pop()
                                                        .Dereference(instr.Position);
                    }

                    m_ArgumentStack.Push(genItem.CreateGeneric(genericArgs));
                }
                else
                {
                    int instructionIndex = m_InstructionPointer - 1;
                    BadObject loadTarget = m_ArgumentStack.Pop().Dereference(instr.Position);
                    string loadMemberName = (string)instr.Arguments[0];

                    if (BadNativeOptimizationSettings.Instance.UseInlineCaching &&
                        TryGetLoadMemberInlineCache(instructionIndex, loadMemberName, loadTarget, out BadObjectReference cachedReference))
                    {
                        Interlocked.Increment(ref LoadMemberInlineCacheHitCount);
                        m_ArgumentStack.Push(cachedReference);
                        break;
                    }

                    if (BadNativeOptimizationSettings.Instance.UseInlineCaching)
                    {
                        Interlocked.Increment(ref LoadMemberInlineCacheMissCount);
                    }

                    BadObjectReference resolvedReference;

                    // Fast path: BadClass with cached property reference (AP5)
                    if (BadNativeOptimizationSettings.Instance.UsePropertyReferenceCache &&
                        loadTarget is BadClass loadCls)
                    {
                        Interlocked.Increment(ref LoadMemberPropertyCacheFastPathCount);
                        resolvedReference = loadCls.GetCachedProperty(loadMemberName, ctx.Scope);
                    }
                    else
                    {
                        Interlocked.Increment(ref LoadMemberPropertySlowPathCount);
                        resolvedReference = loadTarget.GetProperty(loadMemberName, ctx.Scope);
                    }

                    if (BadNativeOptimizationSettings.Instance.UseInlineCaching)
                    {
                        SetLoadMemberInlineCache(instructionIndex, loadMemberName, loadTarget, resolvedReference);
                    }

                    m_ArgumentStack.Push(resolvedReference);
                }

                break;
            }
            case BadOpCode.LoadMemberNullChecked:
            {
                BadObject obj = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);
                string name = (string)instr.Arguments[0];

                int instructionIndex = m_InstructionPointer - 1;

                if (BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache &&
                    BadNativeOptimizationSettings.Instance.UseInlineCaching &&
                    TryGetLoadMemberInlineCache(instructionIndex, name, obj, out BadObjectReference nullCheckCachedReference))
                {
                    Interlocked.Increment(ref NullCheckInlineCacheHitCount);

                    if (instr.Arguments.Length > 1 && instr.Arguments[1] is int cachedGenericArgCount && cachedGenericArgCount != 0)
                    {
                        BadObject cachedLeft = nullCheckCachedReference.Dereference(instr.Position);

                        if (cachedLeft is not IBadGenericObject cachedGeneric)
                        {
                            throw BadRuntimeException.Create(ctx.Scope,
                                                             "Variable is not a generic object",
                                                             instr.Position
                                                            );
                        }

                        BadObject[] cachedGenericArgs = new BadObject[cachedGenericArgCount];

                        for (int i = cachedGenericArgCount - 1; i >= 0; i--)
                        {
                            cachedGenericArgs[i] = m_ArgumentStack.Pop().Dereference(instr.Position);
                        }

                        m_ArgumentStack.Push(cachedGeneric.CreateGeneric(cachedGenericArgs));
                    }
                    else
                    {
                        m_ArgumentStack.Push(nullCheckCachedReference);
                    }

                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache)
                {
                    Interlocked.Increment(ref NullCheckInlineCacheMissCount);
                }

                // null-propagation: if obj is null or does not have the property, push null
                if (obj == BadObject.Null || !obj.HasProperty(name, ctx.Scope))
                {
                    m_ArgumentStack.Push(BadObject.Null);
                }
                else
                {
                    // Use GetProperty (same as LoadMember) — TryGetProperty is NOT overridden by BadClass
                    BadObjectReference property = obj.GetProperty(name, ctx.Scope);

                    if (BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache &&
                        BadNativeOptimizationSettings.Instance.UseInlineCaching)
                    {
                        SetLoadMemberInlineCache(instructionIndex, name, obj, property);
                    }

                    if (instr.Arguments.Length > 1 && instr.Arguments[1] is int genericArgCount && genericArgCount != 0)
                    {
                        BadObject left = property.Dereference(instr.Position);

                        if (left is not IBadGenericObject genItem)
                        {
                            throw BadRuntimeException.Create(ctx.Scope,
                                                             "Variable is not a generic object",
                                                             instr.Position
                                                            );
                        }

                        BadObject[] genericArgs = new BadObject[genericArgCount];

                        for (int i = genericArgCount - 1; i >= 0; i--)
                        {
                            genericArgs[i] = m_ArgumentStack.Pop()
                                                            .Dereference(instr.Position);
                        }

                        m_ArgumentStack.Push(genItem.CreateGeneric(genericArgs));
                    }
                    else
                    {
                        m_ArgumentStack.Push(property);
                    }
                }

                break;
            }
            case BadOpCode.LoadArrayAccess:
            {
                BadObject obj = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);
                int argCount = (int)instr.Arguments[0];

                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop();
                }

                BadObject r = BadObject.Null;

                foreach (BadObject o in BadArrayAccessExpression.Access(ctx, obj, args, instr.Position))
                {
                    r = o;

                    yield return o;
                }

                m_ArgumentStack.Push(r);

                break;
            }
            case BadOpCode.LoadArrayAccessNullChecked:
            {
                BadObject obj = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);
                int argCount = (int)instr.Arguments[0];

                if (obj == BadObject.Null)
                {
                    for (int i = 0; i < argCount; i++)
                    {
                        m_ArgumentStack.Pop();
                    }

                    m_ArgumentStack.Push(BadObject.Null);

                    break;
                }

                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop();
                }

                BadObject r = BadObject.Null;

                foreach (BadObject o in BadArrayAccessExpression.Access(ctx, obj, args, instr.Position))
                {
                    r = o;

                    yield return o;
                }

                m_ArgumentStack.Push(r);

                break;
            }
            case BadOpCode.LoadArrayAccessReverse:
            {
                BadObject obj = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);
                int argCount = (int)instr.Arguments[0];

                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop();
                }

                BadObject r = BadObject.Null;

                foreach (BadObject o in BadArrayAccessReverseExpression.Access(ctx, obj, args, instr.Position))
                {
                    r = o;

                    yield return o;
                }

                m_ArgumentStack.Push(r);

                break;
            }
            case BadOpCode.LoadArrayAccessReverseNullChecked:
            {
                BadObject obj = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);

                if (obj == BadObject.Null)
                {
                    m_ArgumentStack.Push(BadObject.Null);

                    break;
                }

                int argCount = (int)instr.Arguments[0];

                BadObject[] args = new BadObject[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop();
                }

                BadObject r = BadObject.Null;

                foreach (BadObject o in BadArrayAccessReverseExpression.Access(ctx, obj, args, instr.Position))
                {
                    r = o;

                    yield return o;
                }

                m_ArgumentStack.Push(r);

                break;
            }
            case BadOpCode.Swap:
            {
                BadObject a = m_ArgumentStack.Pop();
                BadObject b = m_ArgumentStack.Pop();

                m_ArgumentStack.Push(a);
                m_ArgumentStack.Push(b);

                break;
            }
            case BadOpCode.Assign:
            {
                BadObject val = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                left.Set(val, instr.Position);
                m_ArgumentStack.Push(left);

                break;
            }
            case BadOpCode.Push:
                m_ArgumentStack.Push((BadObject)instr.Arguments[0]);

                break;
            case BadOpCode.FormatString:
            {
                string format = (string)instr.Arguments[0];
                int argCount = (int)instr.Arguments[1];
                object?[] args = new object?[argCount];

                for (int i = argCount - 1; i >= 0; i--)
                {
                    args[i] = m_ArgumentStack.Pop()
                                             .Dereference(instr.Position);
                }

                m_ArgumentStack.Push(string.Format(format, args));

                break;
            }
            case BadOpCode.And:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                m_ArgumentStack.Push(BadLogicAndExpression.And(left, right, instr.Position));

                break;
            }
            case BadOpCode.Not:
            {
                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseUnaryOperatorSpecialization && left is IBadBoolean boolValue)
                {
                    Interlocked.Increment(ref UnaryOperatorSpecializationHitCount);
                    m_ArgumentStack.Push(!boolValue.Value);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseUnaryOperatorSpecialization)
                {
                    Interlocked.Increment(ref UnaryOperatorSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject? o in BadLogicNotExpression.NotWithOverride(ctx, left, instr.Position))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadLogicNotExpression.Not(left, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.XOr:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                m_ArgumentStack.Push(BadLogicXOrExpression.XOr(left, right, instr.Position));

                break;
            }
            case BadOpCode.AndAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                left.Set(BadLogicAndExpression.And(left.Dereference(instr.Position), right, instr.Position), instr.Position);

                break;
            }
            case BadOpCode.XOrAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                left.Set(BadLogicXOrExpression.XOr(left.Dereference(instr.Position), right, instr.Position), instr.Position);

                break;
            }
            case BadOpCode.Add:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization &&
                    TrySpecializeArithmetic(BadOpCode.Add, left, right, instr.Flags.HasFlag(BadInstructionFlags.TransientResult), out BadObject specializedAddResult))
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationHitCount);
                    m_ArgumentStack.Push(specializedAddResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization)
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadAddExpression.AddWithOverride(ctx, left, right, instr.Position))
                    {
                        obj = o;
                    }
                }
                else
                {
                    obj = BadAddExpression.Add(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.Sub:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization &&
                    TrySpecializeArithmetic(BadOpCode.Sub, left, right, instr.Flags.HasFlag(BadInstructionFlags.TransientResult), out BadObject specializedSubResult))
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationHitCount);
                    m_ArgumentStack.Push(specializedSubResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization)
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadSubtractExpression.SubWithOverride(ctx, left, right, instr.Position))
                    {
                        obj = o;
                    }
                }
                else
                {
                    obj = BadSubtractExpression.Sub(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.Mul:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization &&
                    TrySpecializeArithmetic(BadOpCode.Mul, left, right, instr.Flags.HasFlag(BadInstructionFlags.TransientResult), out BadObject specializedMulResult))
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationHitCount);
                    m_ArgumentStack.Push(specializedMulResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization)
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadMultiplyExpression.MulWithOverride(ctx, left, right, instr.Position))
                    {
                        obj = o;
                    }
                }
                else
                {
                    obj = BadMultiplyExpression.Mul(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.Exp:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadExponentiationExpression.ExpWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position
                                 ))
                    {
                        obj = o;
                    }
                }
                else
                {
                    obj = BadExponentiationExpression.Exp(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.Div:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization &&
                    TrySpecializeArithmetic(BadOpCode.Div, left, right, instr.Flags.HasFlag(BadInstructionFlags.TransientResult), out BadObject specializedDivResult))
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationHitCount);
                    m_ArgumentStack.Push(specializedDivResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization)
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadDivideExpression.DivWithOverride(ctx, left, right, instr.Position))
                    {
                        obj = o;
                    }
                }
                else
                {
                    obj = BadDivideExpression.Div(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.Mod:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization &&
                    TrySpecializeArithmetic(BadOpCode.Mod, left, right, instr.Flags.HasFlag(BadInstructionFlags.TransientResult), out BadObject specializedModResult))
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationHitCount);
                    m_ArgumentStack.Push(specializedModResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization)
                {
                    Interlocked.Increment(ref BinaryOperatorSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadModulusExpression.ModWithOverride(ctx, left, right, instr.Position))
                    {
                        obj = o;
                    }
                }
                else
                {
                    obj = BadModulusExpression.Mod(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.Neg:
            {
                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);

                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseUnaryOperatorSpecialization && left is IBadNumber numberValue)
                {
                    Interlocked.Increment(ref UnaryOperatorSpecializationHitCount);
                    m_ArgumentStack.Push(BadNumber.Get(-numberValue.Value)); // Phase C1
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseUnaryOperatorSpecialization)
                {
                    Interlocked.Increment(ref UnaryOperatorSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadNegationExpression.NegateWithOverride(ctx, left, instr.Position))
                    {
                        yield return o;

                        obj = o;
                    }
                }
                else
                {
                    obj = BadNegationExpression.Negate(left, instr.Position);
                }

                m_ArgumentStack.Push(obj);

                break;
            }
            case BadOpCode.JumpRelative:
                m_InstructionPointer += (int)instr.Arguments[0];

                break;
            case BadOpCode.JumpRelativeIfFalse:
            {
                IBadBoolean val = (IBadBoolean)m_ArgumentStack.Pop()
                                                              .Dereference(instr.Position);

                if (!val.Value)
                {
                    m_InstructionPointer += (int)instr.Arguments[0];
                }

                break;
            }
            case BadOpCode.JumpRelativeIfNull:
            {
                BadObject val = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);

                if (val == BadObject.Null)
                {
                    m_InstructionPointer += (int)instr.Arguments[0];
                }

                break;
            }
            case BadOpCode.JumpRelativeIfNotNull:
            {
                BadObject val = m_ArgumentStack.Pop()
                                               .Dereference(instr.Position);

                if (val != BadObject.Null)
                {
                    m_InstructionPointer += (int)instr.Arguments[0];
                }

                break;
            }
            case BadOpCode.JumpRelativeIfTrue:
            {
                IBadBoolean val = (IBadBoolean)m_ArgumentStack.Pop()
                                                              .Dereference(instr.Position);

                if (val.Value)
                {
                    m_InstructionPointer += (int)instr.Arguments[0];
                }

                break;
            }
            case BadOpCode.CreateScope:
            {
                //0: name
                //1: useVisibility
                //3: flags
                //4: relative jump to break
                //5: relative jump to continue
                //6: relative jump to return
                //7: relative jump to throw
                string name = (string)instr.Arguments[0];
                bool? useVisibility = null;

                if (instr.Arguments[1] != BadObject.Null)
                {
                    useVisibility = (bool)instr.Arguments[1];
                }

                BadScopeFlags flags = BadScopeFlags.AllowThrow;

                if (instr.Arguments.Length > 2)
                {
                    flags = (BadScopeFlags)instr.Arguments[2];
                }

                BadRuntimeVirtualStackFrame sf =
                    new BadRuntimeVirtualStackFrame(new BadExecutionContext(ctx.Scope.CreateChild(name,
                                                                                 ctx.Scope,
                                                                                 useVisibility,
                                                                                 flags
                                                                                )
                                                                           )
                                                   ) { CreatePointer = m_InstructionPointer };
                m_ContextStack.Push(sf);

                break;
            }
            case BadOpCode.DestroyScope:
            {
                BadRuntimeVirtualStackFrame? frame = m_ContextStack.Pop();
                frame.Context.Dispose();

                break;
            }
            case BadOpCode.AddDisposeFinalizer:
            {
                string variableName = (string)instr.Arguments[0];
                BadObjectReference disposableRef;

                if (TryResolveFrameVariable(variableName,
                                            out BadRuntimeVirtualStackFrame resolvedFrame,
                                            out BadSlotInfo resolvedSlotInfo,
                                            out bool resolvedIsCapture))
                {
                    disposableRef = resolvedIsCapture
                                        ? GetCaptureReference(resolvedFrame, variableName, resolvedSlotInfo)
                                        : GetSlotReference(resolvedFrame, variableName, resolvedSlotInfo);
                }
                else
                {
                    disposableRef = ctx.Scope.GetVariable(variableName);
                }

                ctx.Scope.AddFinalizer(() =>
                                      {
                                          BadObject obj = disposableRef.Dereference(instr.Position);

                                          if (!obj.HasProperty("Dispose"))
                                          {
                                              throw BadRuntimeException.Create(ctx.Scope,
                                                                               "Object does not implement IDisposable",
                                                                               instr.Position
                                                                              );
                                          }

                                          BadObject disposeFunc = obj.GetProperty("Dispose", ctx.Scope)
                                                                     .Dereference(instr.Position);

                                          foreach (BadObject _ in BadInvocationExpression.Invoke(disposeFunc,
                                                       Array.Empty<BadObject>(),
                                                       instr.Position,
                                                       ctx
                                                      ))
                                          {
                                              // Intentionally ignored - only completion/errors matter.
                                          }
                                      });

                break;
            }
            case BadOpCode.ClearStack:
                m_ArgumentStack.Clear();

                break;
            case BadOpCode.TypeOf:
            {
                m_ArgumentStack.Push(m_ArgumentStack.Pop()
                                                    .Dereference(instr.Position)
                                                    .GetPrototype()
                                    );

                break;
            }
            case BadOpCode.InstanceOf:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);

                if (right is not BadClassPrototype type)
                {
                    throw BadRuntimeException.Create(ctx.Scope,
                                                     "Cannot check if an object is an instance of a non-class object.",
                                                     instr.Position
                                                    );
                }

                m_ArgumentStack.Push(type.IsSuperClassOf(left.GetPrototype()));

                break;
            }
            case BadOpCode.Export:
            {
                if (instr.Arguments.Length == 0)
                {
                    BadObject obj = m_ArgumentStack.Pop()
                                                    .Dereference(instr.Position);
                    ctx.Scope.SetExports(ctx, obj);
                }
                else
                {
                    string name = (string)instr.Arguments[0];

                    BadObject obj = ctx.Scope.GetVariable(name)
                                       .Dereference(instr.Position);
                    ctx.Scope.AddExport(name, obj);
                }

                break;
            }
            case BadOpCode.Import:
            {
                string name = (string)instr.Arguments[0];
                string path = (string)instr.Arguments[1];
                foreach (var o in BadImportExpression.Import(ctx, name, path, instr.Position))
                {
                }

                break;
            }
            case BadOpCode.DefineProperty:
            {
                string name = (string)instr.Arguments[0];
                BadExpression getExpression = (BadExpression)instr.Arguments[1];
                BadExpression? setExpression = instr.Arguments[2] as BadExpression;
                BadExpression[] attributes = (BadExpression[])instr.Arguments[3];
                bool hasTypeExpression = (bool)instr.Arguments[4];
                BadClassPrototype type = BadAnyPrototype.Instance;

                if (hasTypeExpression)
                {
                    BadObject obj = m_ArgumentStack.Pop()
                                                   .Dereference(instr.Position);

                    if (obj is not BadClassPrototype proto)
                    {
                        throw BadRuntimeException.Create(ctx.Scope,
                                                         "Type expression must be a class prototype",
                                                         instr.Position
                                                        );
                    }

                    type = proto;
                }

                if (type == BadVoidPrototype.Instance)
                {
                    throw BadRuntimeException.Create(ctx.Scope,
                                                     "Cannot declare a property of type 'void'",
                                                     instr.Position
                                                    );
                }

                List<BadObject> computedAttributes = new List<BadObject>();
                BadPropertyDefinitionExpression propertyExpression =
                    new BadPropertyDefinitionExpression(name,
                                                        instr.Position,
                                                        getExpression,
                                                        hasTypeExpression ? new BadConstantExpression(instr.Position, type) : null,
                                                        setExpression
                                                       );
                propertyExpression.SetAttributes(attributes);

                foreach (BadObject o in propertyExpression.EvaluateAttributes(ctx, computedAttributes))
                {
                    yield return o;
                }

                ctx.Scope.DefineProperty(name,
                                         type,
                                         getExpression,
                                         setExpression,
                                         ctx,
                                         computedAttributes.ToArray()
                                        );

                break;
            }
            case BadOpCode.CreateFunction:
            {
                BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)instr.Arguments[0];
                BadObject result = BadObject.Null;

                foreach (BadObject o in template.Instantiate(ctx))
                {
                    result = o;
                    yield return o;
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.CreateClass:
            {
                BadCompiledClassTemplate template = (BadCompiledClassTemplate)instr.Arguments[0];
                BadObject result = BadObject.Null;

                foreach (BadObject o in template.Instantiate(ctx))
                {
                    result = o;
                    yield return o;
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.CreateInterface:
            {
                BadCompiledInterfaceTemplate template = (BadCompiledInterfaceTemplate)instr.Arguments[0];
                BadObject result = BadObject.Null;

                foreach (BadObject o in template.Instantiate(ctx))
                {
                    result = o;
                    yield return o;
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.Delete:
            {
                BadObject? obj = m_ArgumentStack.Pop();

                if (obj is not BadObjectReference r)
                {
                    throw BadRuntimeException.Create(ctx.Scope,
                                                     "Cannot delete a non-reference object.",
                                                     instr.Position
                                                    );
                }

                r.Delete(instr.Position);

                break;
            }
            case BadOpCode.Equals:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization &&
                    TrySpecializeComparison(BadOpCode.Equals, left, right, out BadObject specializedEqualsResult))
                {
                    Interlocked.Increment(ref ComparisonSpecializationHitCount);
                    m_ArgumentStack.Push(specializedEqualsResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization)
                {
                    Interlocked.Increment(ref ComparisonSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadEqualityExpression.EqualWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadEqualityExpression.Equal(left, right);
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.NotEquals:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization &&
                    TrySpecializeComparison(BadOpCode.NotEquals, left, right, out BadObject specializedNotEqualsResult))
                {
                    Interlocked.Increment(ref ComparisonSpecializationHitCount);
                    m_ArgumentStack.Push(specializedNotEqualsResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization)
                {
                    Interlocked.Increment(ref ComparisonSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadInequalityExpression.NotEqualWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadInequalityExpression.NotEqual(left, right);
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.Greater:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization &&
                    TrySpecializeComparison(BadOpCode.Greater, left, right, out BadObject specializedGreaterResult))
                {
                    Interlocked.Increment(ref ComparisonSpecializationHitCount);
                    if (BadNativeOptimizationSettings.Instance.UseLoopConditionSpecialization)
                    {
                        Interlocked.Increment(ref LoopConditionSpecializationCount);
                    }
                    m_ArgumentStack.Push(specializedGreaterResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization)
                {
                    Interlocked.Increment(ref ComparisonSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadGreaterThanExpression.GreaterThanWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadGreaterThanExpression.GreaterThan(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.GreaterEquals:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization &&
                    TrySpecializeComparison(BadOpCode.GreaterEquals, left, right, out BadObject specializedGreaterEqualsResult))
                {
                    Interlocked.Increment(ref ComparisonSpecializationHitCount);
                    if (BadNativeOptimizationSettings.Instance.UseLoopConditionSpecialization)
                    {
                        Interlocked.Increment(ref LoopConditionSpecializationCount);
                    }
                    m_ArgumentStack.Push(specializedGreaterEqualsResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization)
                {
                    Interlocked.Increment(ref ComparisonSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadGreaterOrEqualExpression.GreaterOrEqualWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadGreaterOrEqualExpression.GreaterOrEqual(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.Less:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization &&
                    TrySpecializeComparison(BadOpCode.Less, left, right, out BadObject specializedLessResult))
                {
                    Interlocked.Increment(ref ComparisonSpecializationHitCount);
                    if (BadNativeOptimizationSettings.Instance.UseLoopConditionSpecialization)
                    {
                        Interlocked.Increment(ref LoopConditionSpecializationCount);
                    }
                    m_ArgumentStack.Push(specializedLessResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization)
                {
                    Interlocked.Increment(ref ComparisonSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadLessThanExpression.LessThanWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadLessThanExpression.LessThan(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.LessEquals:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                BadObject obj = BadObject.Null;

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization &&
                    TrySpecializeComparison(BadOpCode.LessEquals, left, right, out BadObject specializedLessEqualsResult))
                {
                    Interlocked.Increment(ref ComparisonSpecializationHitCount);
                    if (BadNativeOptimizationSettings.Instance.UseLoopConditionSpecialization)
                    {
                        Interlocked.Increment(ref LoopConditionSpecializationCount);
                    }
                    m_ArgumentStack.Push(specializedLessEqualsResult);
                    break;
                }

                if (BadNativeOptimizationSettings.Instance.UseComparisonSpecialization)
                {
                    Interlocked.Increment(ref ComparisonSpecializationMissCount);
                }

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadLessOrEqualExpression.LessOrEqualWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadLessOrEqualExpression.LessOrEqual(left, right, instr.Position);
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.AddAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject obj = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadAddAssignExpression.AddWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position,
                                  "+="
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadAddAssignExpression.Add(left, left.Dereference(instr.Position), right, instr.Position, "+=");
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.SubAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject obj = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadSubtractAssignExpression.SubtractWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position,
                                  "-="
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadSubtractAssignExpression.Subtract(left,
                                                               left.Dereference(instr.Position),
                                                               right,
                                                               instr.Position,
                                                               "-="
                                                              );
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.MulAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject obj = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadMultiplyAssignExpression.MultiplyWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position,
                                  "*="
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadMultiplyAssignExpression.Multiply(left,
                                                               left.Dereference(instr.Position),
                                                               right,
                                                               instr.Position,
                                                               "*="
                                                              );
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.DivAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject obj = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadDivideAssignExpression.DivideWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position,
                                  "/="
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadDivideAssignExpression.Divide(left, left.Dereference(instr.Position), right, instr.Position, "/=");
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.ModAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject obj = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadModulusAssignExpression.ModulusWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position,
                                  "%="
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadModulusAssignExpression.Modulus(left, left.Dereference(instr.Position), right, instr.Position, "%=");
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.ExpAssign:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject obj = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadExponentiationAssignExpression.ExpWithOverride(ctx,
                                  left,
                                  right,
                                  instr.Position,
                                  "**="
                                 ))
                    {
                        obj = o;

                        yield return o;
                    }
                }
                else
                {
                    obj = BadExponentiationAssignExpression.Exp(left,
                                                                left.Dereference(instr.Position),
                                                                right,
                                                                instr.Position,
                                                                "**="
                                                               );
                }

                m_ArgumentStack.Push(obj.Dereference(instr.Position));

                break;
            }
            case BadOpCode.PostInc:
            {
                BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject? result = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadPostIncrementExpression.IncrementWithOverride(ctx,
                                  obj,
                                  instr.Position
                                 ))
                    {
                        result = o;
                    }
                }
                else
                {
                    result = BadPostIncrementExpression.Increment(obj, instr.Position);
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.PostDec:
            {
                BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject? result = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadPostDecrementExpression.DecrementWithOverride(ctx,
                                  obj,
                                  instr.Position
                                 ))
                    {
                        result = o;
                    }
                }
                else
                {
                    result = BadPostDecrementExpression.Decrement(obj, instr.Position);
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.PreInc:
            {
                BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject? result = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadPreIncrementExpression.IncrementWithOverride(ctx,
                                  obj,
                                  instr.Position
                                 ))
                    {
                        result = o;
                    }
                }
                else
                {
                    result = BadPreIncrementExpression.Increment(obj, instr.Position);
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.PreDec:
            {
                BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
                BadObject? result = BadObject.Null;

                if (m_UseOverrides)
                {
                    foreach (BadObject o in BadPreDecrementExpression.DecrementWithOverride(ctx,
                                  obj,
                                  instr.Position
                                 ))
                    {
                        result = o;
                    }
                }
                else
                {
                    result = BadPreDecrementExpression.Decrement(obj, instr.Position);
                }

                m_ArgumentStack.Push(result);

                break;
            }
            case BadOpCode.Return:
            {
                BadObject ret = BadObject.Null;

                if (instr.Arguments.Length != 0)
                {
                    ret = m_ArgumentStack.Pop();
                    bool isRefReturn = (bool)instr.Arguments[0];

                    if (!isRefReturn)
                    {
                        ret = ret.Dereference(instr.Position);
                    }
                }

                if (ctx.Scope.FunctionObject != null &&
                    ctx.Scope.FunctionObject.ReturnType == BadVoidPrototype.Instance)
                {
                    if (!ctx.Scope.FunctionObject.IsSingleLine)
                    {
                        throw BadRuntimeException.Create(ctx.Scope,
                                                         "Cannot return a value from a void function",
                                                         instr.Position
                                                        );
                    }

                    ctx.Scope.SetReturnValue(BadVoidPrototype.Object);
                }
                else
                {
                    ctx.Scope.SetReturnValue(ret);
                }

                break;
            }
            case BadOpCode.Break:
                ctx.Scope.SetBreak();

                break;
            case BadOpCode.Continue:
                ctx.Scope.SetContinue();

                break;
            case BadOpCode.Throw:
                throw new BadRuntimeErrorException(new BadRuntimeError(null,
                                                                       m_ArgumentStack.Pop(),
                                                                       ctx.Scope.GetStackTrace()
                                                                      )
                                                  );

            case BadOpCode.SetBreakPointer:
                m_ContextStack.Peek()
                              .BreakPointer = (int)instr.Arguments[0];

                break;
            case BadOpCode.SetContinuePointer:
                m_ContextStack.Peek()
                              .ContinuePointer = (int)instr.Arguments[0];

                break;
            case BadOpCode.SetThrowPointer:
                m_ContextStack.Peek()
                              .ThrowPointer = (int)instr.Arguments[0];

                break;
            case BadOpCode.BinaryUnpack:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);

                BadObject left = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);
                m_ArgumentStack.Push(BadBinaryUnpackExpression.Unpack(left, right, instr.Position));

                break;
            }
            case BadOpCode.UnaryUnpack:
            {
                BadObject right = m_ArgumentStack.Pop()
                                                 .Dereference(instr.Position);
                BadTable table = ctx.Scope.GetTable();
                BadUnaryUnpackExpression.Unpack(table, right, instr.Position);
                m_ArgumentStack.Push(table);

                break;
            }
            case BadOpCode.Eval:
            {
                BadExpression expr = (BadExpression)instr.Arguments[0];
                Interlocked.Increment(ref EvalInstructionCount);
                OnEvalInstruction?.Invoke(expr);
                BadLogger.Warn($"VM Eval fallback for expression '{expr.GetType().Name}'",
                               BadLogMask.GetMask("Compiler", "EVAL"),
                               expr.Position
                              );
                BadObject ret = BadObject.Null;

                foreach (BadObject o in ctx.Execute(expr))
                {
                    ret = o;

                    yield return o;
                }

                m_ArgumentStack.Push(ret);

                break;
            }
            case BadOpCode.LoadLocal:
            {
                // Load a local variable from slot (Phase 5 optimization)
                Interlocked.Increment(ref LoadLocalOpcodeCount);
                int slotIndex = (int)instr.Arguments[0];
                BadRuntimeVirtualStackFrame frame = m_ContextStack.Peek();

                if (frame.LocalSlots == null || slotIndex >= frame.LocalSlots.Length)
                {
                    throw BadRuntimeException.Create(ctx.Scope, $"Invalid local slot index: {slotIndex}", instr.Position);
                }

                m_ArgumentStack.Push(frame.LocalSlots[slotIndex]);
                break;
            }
            case BadOpCode.StoreLocal:
            {
                // Store a value into a local variable slot (Phase 5 optimization)
                Interlocked.Increment(ref StoreLocalOpcodeCount);
                int slotIndex = (int)instr.Arguments[0];
                BadObject value = m_ArgumentStack.Pop();
                BadRuntimeVirtualStackFrame frame = m_ContextStack.Peek();

                if (frame.LocalSlots == null || slotIndex >= frame.LocalSlots.Length)
                {
                    throw BadRuntimeException.Create(ctx.Scope, $"Invalid local slot index: {slotIndex}", instr.Position);
                }

                frame.LocalSlots[slotIndex] = value;
                m_ArgumentStack.Push(value);
                break;
            }
            case BadOpCode.LoadCaptured:
            {
                Interlocked.Increment(ref LoadCapturedOpcodeCount);
                int slotIndex = (int)instr.Arguments[0];
                BadRuntimeVirtualStackFrame frame = m_ContextStack.Peek();

                if (!TryGetCaptureSlotByIndex(frame, slotIndex, out BadSlotInfo slotInfo))
                {
                    throw BadRuntimeException.Create(ctx.Scope, $"Invalid captured slot index: {slotIndex}", instr.Position);
                }

                m_ArgumentStack.Push(GetCaptureReference(frame, slotInfo.Name, slotInfo));
                break;
            }
            case BadOpCode.StoreCaptured:
            {
                Interlocked.Increment(ref StoreCapturedOpcodeCount);
                int slotIndex = (int)instr.Arguments[0];
                BadObject value = m_ArgumentStack.Pop();
                BadRuntimeVirtualStackFrame frame = m_ContextStack.Peek();

                if (!TryGetCaptureSlotByIndex(frame, slotIndex, out BadSlotInfo slotInfo))
                {
                    throw BadRuntimeException.Create(ctx.Scope, $"Invalid captured slot index: {slotIndex}", instr.Position);
                }

                GetCaptureReference(frame, slotInfo.Name, slotInfo).Set(value.Dereference(instr.Position), instr.Position);
                m_ArgumentStack.Push(value);
                break;
            }
            case BadOpCode.InitLocals:
            {
                // Initialize local slots for a function frame (Phase 5 optimization)
                Interlocked.Increment(ref InitLocalsOpcodeCount);
                int slotCount = (int)instr.Arguments[0];
                BadRuntimeVirtualStackFrame frame = m_ContextStack.Peek();
                frame.LocalSlots = new BadObject[slotCount];
                break;
            }
            case BadOpCode.GetEnumerator:
            {
                BadObject target = m_ArgumentStack.Pop()
                                                .Dereference(instr.Position);

                bool hasDirectEnumerator = target.HasProperty("MoveNext", ctx.Scope) &&
                                           target.HasProperty("GetCurrent", ctx.Scope);

                if (hasDirectEnumerator)
                {
                    Interlocked.Increment(ref GetEnumeratorDirectPathCount);
                    m_ArgumentStack.Push(target);
                }
                else if (target.HasProperty("GetEnumerator", ctx.Scope))
                {
                    Interlocked.Increment(ref GetEnumeratorMethodPathCount);
                    if (target.GetProperty("GetEnumerator", ctx.Scope)
                              .Dereference(instr.Position) is not BadFunction getEnumerator)
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "Invalid enumerator", instr.Position);
                    }

                    BadObject enumerator = BadObject.Null;

                    foreach (BadObject o in getEnumerator.Invoke(Array.Empty<BadObject>(), ctx))
                    {
                        enumerator = o;
                        yield return o;
                    }

                    if (enumerator == BadObject.Null)
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "Invalid enumerator", instr.Position);
                    }

                    m_ArgumentStack.Push(enumerator.Dereference(instr.Position));
                }
                else if (target is IBadEnumerable enumerable)
                {
                    Interlocked.Increment(ref GetEnumeratorEnumerablePathCount);
                    m_ArgumentStack.Push(new BadInteropEnumerator(enumerable.GetEnumerator()));
                }
                else
                {
                    Interlocked.Increment(ref GetEnumeratorFallbackPathCount);
                    m_ArgumentStack.Push(target);
                }

                break;
            }
            case BadOpCode.MoveNext:
            {
                BadObject enumeratorTarget = m_ArgumentStack.Pop().Dereference(instr.Position);

                // Fast path 1: wrapped C# enumerator (IBadEnumerator = IEnumerator<BadObject>)
                if (BadNativeOptimizationSettings.Instance.UseLoopFastPath &&
                    enumeratorTarget is IBadEnumerator nativeEnum)
                {
                    Interlocked.Increment(ref LoopMoveNextNativeFastPathCount);
                    m_ArgumentStack.Push(nativeEnum.MoveNext() ? BadObject.True : BadObject.False);
                    break;
                }

                // Fast path 2: compiled BadClass with method slot (AP3 VTable)
                if (BadNativeOptimizationSettings.Instance.UseLoopFastPath &&
                    enumeratorTarget is BadClass moveNextCls &&
                    moveNextCls.TryGetMethodSlot("MoveNext", out BadObject? moveNextSlot) &&
                    moveNextSlot is BadFunction moveNextSlotFn)
                {
                    Interlocked.Increment(ref LoopMoveNextMethodSlotFastPathCount);
                    BadObject cond = BadObject.Null;
                    foreach (BadObject o in moveNextSlotFn.Invoke(Array.Empty<BadObject>(), ctx))
                    {
                        cond = o;
                        yield return o;
                    }
                    m_ArgumentStack.Push(cond.Dereference(instr.Position));
                    break;
                }

                // Slow path: general GetProperty lookup
                Interlocked.Increment(ref LoopMoveNextSlowPathCount);
                if (enumeratorTarget.GetProperty("MoveNext", ctx.Scope).Dereference(instr.Position)
                    is not BadFunction moveNext)
                {
                    throw new BadRuntimeException("Invalid enumerator", instr.Position);
                }
                BadObject condSlow = BadObject.Null;
                foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), ctx))
                {
                    condSlow = o;
                    yield return o;
                }
                m_ArgumentStack.Push(condSlow.Dereference(instr.Position));
                break;
            }
            case BadOpCode.GetCurrent:
            {
                BadObject enumeratorTarget = m_ArgumentStack.Pop().Dereference(instr.Position);

                // Fast path 1: wrapped C# enumerator
                if (BadNativeOptimizationSettings.Instance.UseLoopFastPath &&
                    enumeratorTarget is IBadEnumerator nativeEnumCurrent)
                {
                    Interlocked.Increment(ref LoopGetCurrentNativeFastPathCount);
                    m_ArgumentStack.Push(nativeEnumCurrent.Current);
                    break;
                }

                // Fast path 2: compiled BadClass with method slot
                if (BadNativeOptimizationSettings.Instance.UseLoopFastPath &&
                    enumeratorTarget is BadClass getCurrentCls &&
                    getCurrentCls.TryGetMethodSlot("GetCurrent", out BadObject? getCurrentSlot) &&
                    getCurrentSlot is BadFunction getCurrentSlotFn)
                {
                    Interlocked.Increment(ref LoopGetCurrentMethodSlotFastPathCount);
                    BadObject current = BadObject.Null;
                    foreach (BadObject o in getCurrentSlotFn.Invoke(Array.Empty<BadObject>(), ctx))
                    {
                        current = o;
                        yield return o;
                    }
                    m_ArgumentStack.Push(current.Dereference(instr.Position));
                    break;
                }

                // Slow path: general GetProperty lookup
                Interlocked.Increment(ref LoopGetCurrentSlowPathCount);
                if (enumeratorTarget.GetProperty("GetCurrent", ctx.Scope).Dereference(instr.Position)
                    is not BadFunction getCurrent)
                {
                    throw new BadRuntimeException("Invalid enumerator", instr.Position);
                }
                BadObject currentSlow = BadObject.Null;
                foreach (BadObject o in getCurrent.Invoke(Array.Empty<BadObject>(), ctx))
                {
                    currentSlow = o;
                    yield return o;
                }
                m_ArgumentStack.Push(currentSlow.Dereference(instr.Position));
                break;
            }
            case BadOpCode.BeginLoop:
            {
                // Begin loop block marker (Phase 5 loop optimization - reserved for future implementation)
                break;
            }
            case BadOpCode.EndLoop:
            {
                // End loop block marker (Phase 5 loop optimization - reserved for future implementation)
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    ///     Executes the Virtual Machine.
    /// </summary>
    /// <returns>The result of the execution.</returns>
    /// <exception cref="BadRuntimeException">Gets thrown when the Virtual Machine encounters an error.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Gets thrown when the Virtual Machine encounters an invalid Instruction.</exception>
    private IEnumerable<BadObject> Execute()
    {
        while (m_InstructionPointer <= m_CurrentInstructions.Length)
        {
            BadExecutionContext ctx = m_ContextStack.Peek()
                                                    .Context;

            if (ctx.Scope.ReturnValue != null)
            {
                //Pop scopes until we find a scope that captures return
                while ((ctx.Scope.Flags & BadScopeFlags.CaptureReturn) == 0)
                {
                    m_ContextStack.Pop();

                    if (m_ContextStack.Count == 0)
                    {
                        yield break; //We exited the virtual machine. Quit the Execute method.
                    }

                    BadRuntimeVirtualStackFrame sf = m_ContextStack.Peek();
                    ctx = sf.Context;

                    //Set Return Pointer to the next instruction
                    m_InstructionPointer = sf.ReturnPointer;
                }

                BadRuntimeVirtualStackFrame? retSf = m_ContextStack.Pop();

                if (m_ContextStack.Count == 0)
                {
                    yield break; //We exited the virtual machine. Quit the Execute method.
                }

                //We found a scope that captures return, we push the return value to the stack and continue execution
                m_ArgumentStack.Push(ctx.Scope.ReturnValue!);
                m_InstructionPointer = retSf.ReturnPointer;

                continue;
            }

            if (ctx.Scope.IsBreak)
            {
                //Pop scopes until we find a scope that captures break
                while ((ctx.Scope.Flags & BadScopeFlags.CaptureBreak) == 0)
                {
                    m_ContextStack.Pop();

                    if (m_ContextStack.Count == 0)
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "VIRTUAL MACHINE BREAK ERROR");
                    }

                    ctx = m_ContextStack.Peek()
                                        .Context;
                }

                BadRuntimeVirtualStackFrame? sf = m_ContextStack.Pop();

                //Set Return Pointer to the next instruction
                m_InstructionPointer = sf.CreatePointer + sf.BreakPointer;

                continue;
            }

            if (ctx.Scope.IsContinue)
            {
                //Pop scopes until we find a scope that captures continue
                while ((ctx.Scope.Flags & BadScopeFlags.CaptureContinue) == 0)
                {
                    m_ContextStack.Pop();

                    if (m_ContextStack.Count == 0)
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "VIRTUAL MACHINE CONTINUE ERROR");
                    }

                    BadRuntimeVirtualStackFrame sf = m_ContextStack.Peek();
                    ctx = sf.Context;

                    //Set Return Pointer to the next instruction
                    m_InstructionPointer = sf.CreatePointer + sf.ContinuePointer;
                }

                m_ContextStack.Pop();

                continue;
            }

            if (m_InstructionPointer >= m_CurrentInstructions.Length)
            {
                break;
            }

            int burstSize = BadNativeOptimizationSettings.Instance.VmBurstSize;
            bool burstExitOuter = false;
            bool burstYieldBreak = false;
            bool yieldedInBurst = false;

            for (int burst = 0; burst < burstSize; burst++)
            {
                if (burst > 0)
                {
                    if (m_ContextStack.Count == 0)
                    {
                        burstYieldBreak = true;
                        break;
                    }
                    ctx = m_ContextStack.Peek().Context;
                    if (ctx.Scope.ReturnValue != null || ctx.Scope.IsBreak || ctx.Scope.IsContinue)
                    {
                        break;
                    }
                    if (m_InstructionPointer >= m_CurrentInstructions.Length)
                    {
                        burstExitOuter = true;
                        break;
                    }
                }

                using IEnumerator<BadObject> enumerator = ExecuteStep(ctx).GetEnumerator();
                bool stepExceptionHandled = false;

                while (true)
                {
                    try
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        // Pop frames until we find an active throw target.
                        // CaptureThrow alone is not enough; the frame also needs a valid ThrowPointer.
                        var originalCtx = ctx;
                        BadRuntimeVirtualStackFrame? throwFrame = null;

                        while (m_ContextStack.Count != 0)
                        {
                            BadRuntimeVirtualStackFrame candidate = m_ContextStack.Peek();
                            ctx = candidate.Context;

                            if ((ctx.Scope.Flags & BadScopeFlags.CaptureThrow) != 0 && candidate.ThrowPointer >= 0)
                            {
                                throwFrame = candidate;
                                break;
                            }

                            m_ContextStack.Pop();
                        }

                        if (throwFrame == null)
                        {
                            // No frame installed a throw handler -> propagate as an uncaught exception.
                            ExceptionDispatchInfo.Capture(e).Throw();
                        }

                        BadRuntimeError error;

                        if (e is BadRuntimeErrorException err)
                        {
                            error = err.Error;
                        }
                        else
                        {
                            error = BadRuntimeError.FromException(e, originalCtx.Scope.GetStackTrace());
                        }

                        m_ArgumentStack.Push(error);

                        m_InstructionPointer = throwFrame.CreatePointer + throwFrame.ThrowPointer;
                        m_ContextStack.Pop();

                        if (m_ContextStack.Count == 0)
                        {
                            burstYieldBreak = true;
                        }

                        stepExceptionHandled = true;
                        break;
                    }

                    yield return enumerator.Current ?? BadObject.Null;
                    yieldedInBurst = true;
                }

                if (burstYieldBreak || stepExceptionHandled)
                {
                    break;
                }
            }

            if (burstYieldBreak)
            {
                yield break;
            }

            if (burstExitOuter)
            {
                break;
            }

            if (!yieldedInBurst)
            {
                yield return BadObject.Null;
            }
        }
    }

    /// <summary>
    ///     Executes the virtual machine with the given context.
    /// </summary>
    /// <param name="ctx">The context to execute the virtual machine with.</param>
    /// <returns>The return value of the virtual machine.</returns>
    public IEnumerable<BadObject> Execute(BadExecutionContext ctx, BadObject[]? args = null)
    {
        m_ContextStack.Clear();
        m_ArgumentStack.Clear();
        m_InvokeMemberCallSiteCache.Clear();
        m_InvokeMemberCallSiteOrder.Clear();
        m_LoadMemberInlineCache.Clear();
        m_LoadMemberInlineCacheOrder.Clear();
        m_InvokeMemberNullCheckCache.Clear();
        m_InvokeMemberNullCheckCacheOrder.Clear();
        m_InstructionPointer = 0;
        m_ContextStack.Push(CreateFunctionFrame(m_Function, ctx, args: args));

        return Execute();
    }
}
