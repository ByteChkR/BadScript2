using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
///     Compiles the <see cref="BadMultiplyExpression" />.
/// </summary>
public class BadMultiplyExpressionCompiler : BadBinaryExpressionCompiler<BadMultiplyExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadMultiplyExpression expression)
    {
        context.Emit(BadOpCode.Mul, expression.Position);
    }
}