using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <summary>
///     Compiles the <see cref="BadGreaterOrEqualExpression" />.
/// </summary>
public class BadGreaterOrEqualExpressionCompiler : BadBinaryExpressionCompiler<BadGreaterOrEqualExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadGreaterOrEqualExpression expression)
    {
        context.Emit(BadOpCode.GreaterEquals, expression.Position);
    }
}