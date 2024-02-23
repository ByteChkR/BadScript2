using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
///     Compiles the <see cref="BadPostIncrementExpression" />.
/// </summary>
public class BadPostIncrementExpressionCompiler : BadExpressionCompiler<BadPostIncrementExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPostIncrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PostInc, expression.Position);
    }
}