using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
///     Compiles the <see cref="BadMultiplyAssignExpression" />.
/// </summary>
public class BadMultiplyAssignExpressionCompiler : BadBinaryExpressionCompiler<BadMultiplyAssignExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadMultiplyAssignExpression expression)
    {
        context.Emit(BadOpCode.MulAssign, expression.Position);
    }
}