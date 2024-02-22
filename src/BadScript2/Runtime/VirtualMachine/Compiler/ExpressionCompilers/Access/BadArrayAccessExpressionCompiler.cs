using BadScript2.Parser.Expressions.Access;

/// <summary>
/// Contains Access Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadArrayAccessExpression" />.
/// </summary>
public class BadArrayAccessExpressionCompiler : BadExpressionCompiler<BadArrayAccessExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadArrayAccessExpression expression)
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
                BadOpCode.LoadArrayAccessNullChecked,
                expression.Position,
                expression.ArgumentCount
            );
        }
        else
        {
            yield return new BadInstruction(BadOpCode.LoadArrayAccess, expression.Position, expression.ArgumentCount);
        }
    }
}