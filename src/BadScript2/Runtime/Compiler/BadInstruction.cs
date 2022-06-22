using BadScript2.Common;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler;

public class BadInstruction
{
    public readonly BadObject[] Arguments;
    public readonly BadOpCode OpCode;
    public readonly BadSourcePosition Position;

    public BadInstruction(BadOpCode opCode, BadSourcePosition position, params BadObject[] arguments)
    {
        OpCode = opCode;
        Position = position;
        Arguments = arguments;
    }

    public override string ToString()
    {
        return $"{OpCode} {string.Join(" ", Arguments.Cast<object>())}";
    }
}