using BadScript2.Common;

namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Bit-flags that can be attached to a <see cref="BadInstruction"/> by the
///     compiler's post-processing passes.
/// </summary>
[Flags]
public enum BadInstructionFlags : byte
{
    None = 0,

    /// <summary>
    ///     Phase C2 – Escape Analysis: the numeric result produced by this
    ///     arithmetic instruction is immediately consumed by the next instruction
    ///     (another arithmetic / comparison op) and does NOT escape to a variable,
    ///     return value, or external function.  The VM may reuse a per-VM scratch
    ///     <see cref="BadScript2.Runtime.Objects.Native.BadNumber"/> instead of
    ///     allocating a new one.
    /// </summary>
    TransientResult = 1,
}

/// <summary>
///     Implements a single instruction for the BadVirtualMachine.
/// </summary>
public struct BadInstruction
{
    /// <summary>
    ///     The OpCode of this Instruction.
    /// </summary>
    public readonly BadOpCode OpCode;

    /// <summary>
    ///     The arguments of this Instruction.
    /// </summary>
    public readonly object[] Arguments;

    /// <summary>
    ///     The position of this Instruction in the source code.
    /// </summary>
    public readonly BadSourcePosition Position;

    /// <summary>
    ///     Compiler-set flags (escape analysis annotations, etc.).
    /// </summary>
    public BadInstructionFlags Flags;

    /// <summary>
    ///     Creates a new <see cref="BadInstruction" /> instance.
    /// </summary>
    /// <param name="opCode">The OpCode of this Instruction.</param>
    /// <param name="position">The position of this Instruction in the source code.</param>
    /// <param name="arguments">The arguments of this Instruction.</param>
    public BadInstruction(BadOpCode opCode, BadSourcePosition position, params object[] arguments)
    {
        OpCode = opCode;
        Position = position;
        Arguments = arguments;
        Flags = BadInstructionFlags.None;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{OpCode} {string.Join(" ", Arguments.Select(x => x.ToString()))}";
    }
}