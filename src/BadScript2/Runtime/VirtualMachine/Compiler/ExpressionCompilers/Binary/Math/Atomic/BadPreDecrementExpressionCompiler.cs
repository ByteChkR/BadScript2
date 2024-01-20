using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
/// Compiles the <see cref="BadPreDecrementExpression" />.
/// </summary>
public class BadPreDecrementExpressionCompiler : BadExpressionCompiler<BadPreDecrementExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPreDecrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PreDec, expression.Position);
    }
}