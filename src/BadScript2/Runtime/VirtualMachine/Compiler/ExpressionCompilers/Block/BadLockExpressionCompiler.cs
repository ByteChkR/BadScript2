using BadScript2.Parser.Expressions.Block.Lock;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
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