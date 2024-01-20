using BadScript2.Parser.Expressions.Constant;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadArrayExpressionCompiler : BadExpressionCompiler<BadArrayExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadArrayExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.InitExpressions, false))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.ArrayInit, expression.Position, expression.Length);
    }
}