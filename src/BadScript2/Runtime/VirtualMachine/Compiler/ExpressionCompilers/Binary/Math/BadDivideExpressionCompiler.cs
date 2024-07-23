using BadScript2.Parser.Expressions.Binary.Math;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
///     Compiles the <see cref="BadDivideExpression" />.
/// </summary>
public class BadDivideExpressionCompiler : BadBinaryExpressionCompiler<BadDivideExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadDivideExpression expression)
    {
        context.Emit(BadOpCode.Div, expression.Position);
    }
}