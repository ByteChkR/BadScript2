using BadScript2.Parser.Expressions.Variables;

/// <summary>
/// Contains Variable Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <summary>
///     Compiles the <see cref="BadFormattedStringExpression" />.
/// </summary>
public class BadFormattedStringExpressionCompiler : BadExpressionCompiler<BadFormattedStringExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadFormattedStringExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Expressions, false))
        {
            yield return instruction;
        }

        yield return new BadInstruction(
            BadOpCode.FormatString,
            expression.Position,
            expression.Value,
            expression.ExpressionCount
        );
    }
}