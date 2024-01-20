using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
/// Compiles the <see cref="BadTypeOfExpression" />.
/// </summary>
public class BadTypeOfExpressionCompiler : BadExpressionCompiler<BadTypeOfExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadTypeOfExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Expression))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.TypeOf, expression.Position);
    }
}