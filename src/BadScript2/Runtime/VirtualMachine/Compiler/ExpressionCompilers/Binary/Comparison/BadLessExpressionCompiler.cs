using BadScript2.Parser.Expressions.Binary.Comparison;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <summary>
///     Compiles the <see cref="BadLessThanExpression" />.
/// </summary>
public class BadLessExpressionCompiler : BadBinaryExpressionCompiler<BadLessThanExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLessThanExpression expression)
    {
        context.Emit(BadOpCode.Less, expression.Position);
    }
}