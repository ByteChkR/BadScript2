using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
///     Compiles the <see cref="BadExponentiationAssignExpression" />.
/// </summary>
public class BadExponentiationAssignExpressionCompiler : BadBinaryExpressionCompiler<BadExponentiationAssignExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadExponentiationAssignExpression expression)
    {
        context.Emit(BadOpCode.ExpAssign, expression.Position);
    }
}