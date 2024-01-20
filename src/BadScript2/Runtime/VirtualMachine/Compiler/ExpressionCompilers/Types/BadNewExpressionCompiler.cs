using BadScript2.Parser.Expressions.Types;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;

/// <summary>
/// Compiles the <see cref="BadNewExpression" />.
/// </summary>
public class BadNewExpressionCompiler : BadExpressionCompiler<BadNewExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNewExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right.Arguments, false))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Right.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.New, expression.Position, expression.Right.ArgumentCount);
    }
}