using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadNegateExpressionCompiler : BadExpressionCompiler<BadNegationExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNegationExpression expression)
    {
        foreach (BadInstruction instr in compiler.Compile(expression.Expression))
        {
            yield return instr;
        }

        yield return new BadInstruction(BadOpCode.Neg, expression.Position);
    }
}