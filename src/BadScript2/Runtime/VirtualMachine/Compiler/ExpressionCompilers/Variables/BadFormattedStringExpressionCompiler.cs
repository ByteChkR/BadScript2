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
    public override void Compile(BadExpressionCompileContext context, BadFormattedStringExpression expression)
    {
        context.Compile(expression.Expressions, false);
        context.Emit(BadOpCode.FormatString, expression.Position, expression.Value, expression.ExpressionCount);
    }
}