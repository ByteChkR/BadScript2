using BadScript2.Parser.Expressions.Binary.Math;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
///     Compiles the <see cref="BadSubtractExpression" />.
/// </summary>
public class BadSubtractExpressionCompiler : BadBinaryExpressionCompiler<BadSubtractExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadSubtractExpression expression)
    {
        context.Emit(BadOpCode.Sub, expression.Position);
    }
}