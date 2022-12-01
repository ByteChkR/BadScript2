using System.Collections.Generic;

using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Block;

public class BadLockExpressionCompiler : BadExpressionCompiler<BadLockExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadLockExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.LockExpression))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Dup, expression.Position);
        yield return new BadInstruction(BadOpCode.AquireLock, expression.Position);

        foreach (BadInstruction instruction in compiler.Compile(expression.Block))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.ReleaseLock, expression.Position);
    }
}