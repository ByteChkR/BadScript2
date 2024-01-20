using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
/// Compiles the <see cref="BadArrayAccessReverseExpression" />.
/// </summary>
public class BadArrayAccessReverseExpressionCompiler : BadExpressionCompiler<BadArrayAccessReverseExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(
        BadCompiler compiler,
        BadArrayAccessReverseExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Arguments, false))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        if (expression.NullChecked)
        {
            yield return new BadInstruction(
                BadOpCode.LoadArrayAccessReverseNullChecked,
                expression.Position,
                expression.ArgumentCount
            );
        }
        else
        {
            yield return new BadInstruction(
                BadOpCode.LoadArrayAccessReverse,
                expression.Position,
                expression.ArgumentCount
            );
        }
    }
}