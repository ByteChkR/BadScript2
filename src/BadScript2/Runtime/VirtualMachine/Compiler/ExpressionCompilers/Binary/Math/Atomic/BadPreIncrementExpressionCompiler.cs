using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
///     Compiles the <see cref="BadPreIncrementExpression" />.
/// </summary>
public class BadPreIncrementExpressionCompiler : BadExpressionCompiler<BadPreIncrementExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPreIncrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PreInc, expression.Position);
    }
}