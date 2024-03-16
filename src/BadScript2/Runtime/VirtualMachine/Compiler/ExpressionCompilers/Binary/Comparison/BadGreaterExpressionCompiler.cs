using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <summary>
///     Compiles the <see cref="BadGreaterThanExpression" />.
/// </summary>
public class BadGreaterExpressionCompiler : BadBinaryExpressionCompiler<BadGreaterThanExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadGreaterThanExpression expression)
    {
        context.Emit(BadOpCode.Greater, expression.Position);
    }
}