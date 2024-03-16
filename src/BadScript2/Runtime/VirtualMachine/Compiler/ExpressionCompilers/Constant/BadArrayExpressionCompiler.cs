using BadScript2.Parser.Expressions.Constant;

/// <summary>
/// Contains Constant Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
///     Compiles the <see cref="BadArrayExpression" />.
/// </summary>
public class BadArrayExpressionCompiler : BadExpressionCompiler<BadArrayExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadArrayExpression expression)
    {
        context.Compile(expression.InitExpressions, false);
        context.Emit(BadOpCode.ArrayInit, expression.Position, expression.Length);
    }
}