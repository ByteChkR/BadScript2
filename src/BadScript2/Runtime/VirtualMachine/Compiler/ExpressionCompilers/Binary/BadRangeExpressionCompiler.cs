using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadRangeExpression" />.
/// </summary>
public class BadRangeExpressionCompiler : BadExpressionCompiler<BadRangeExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadRangeExpression expression)
    {
        context.Compile(expression.Right);
        context.Compile(expression.Left);
        context.Emit(BadOpCode.Range, expression.Position);
    }
}