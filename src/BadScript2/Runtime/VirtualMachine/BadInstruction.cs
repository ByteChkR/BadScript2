using BadScript2.Common;

namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Implements a single instruction for the BadVirtualMachine.
/// </summary>
public struct BadInstruction
{
    /// <summary>
    /// The OpCode of this Instruction.
    /// </summary>
    public readonly BadOpCode OpCode;
    /// <summary>
    /// The arguments of this Instruction.
    /// </summary>
    public readonly object[] Arguments;
    /// <summary>
    /// The position of this Instruction in the source code.
    /// </summary>
    public readonly BadSourcePosition Position;

    /// <summary>
    /// Creates a new <see cref="BadInstruction" /> instance.
    /// </summary>
    /// <param name="opCode">The OpCode of this Instruction.</param>
    /// <param name="position">The position of this Instruction in the source code.</param>
    /// <param name="arguments">The arguments of this Instruction.</param>
    public BadInstruction(BadOpCode opCode, BadSourcePosition position, params object[] arguments)
    {
        OpCode = opCode;
        Position = position;
        Arguments = arguments;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{OpCode} {string.Join(" ", Arguments.Select(x => x.ToString()))}";
    }
}