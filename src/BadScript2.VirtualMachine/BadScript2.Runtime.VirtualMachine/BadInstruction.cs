﻿using System.Linq;

using BadScript2.Common;

namespace BadScript2.VirtualMachine;

public struct BadInstruction
{
    public readonly BadOpCode OpCode;
    public readonly object[] Arguments;
    public readonly BadSourcePosition Position;

    public BadInstruction(BadOpCode opCode, BadSourcePosition position, params object[] arguments)
    {
        OpCode = opCode;
        Position = position;
        Arguments = arguments;
    }

    public override string ToString()
    {
        return $"{OpCode} {string.Join(" ", Arguments.Select(x => x.ToString()))}";
    }
}