using BadScript2.Parser.Expressions.Binary.Math;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
///     Compiles the <see cref="BadNegationExpression" />.
/// </summary>
public class BadNegateExpressionCompiler : BadExpressionCompiler<BadNegationExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadNegationExpression expression)
    {
        context.Compile(expression.Expression);
        context.Emit(BadOpCode.Neg, expression.Position);
    }
}