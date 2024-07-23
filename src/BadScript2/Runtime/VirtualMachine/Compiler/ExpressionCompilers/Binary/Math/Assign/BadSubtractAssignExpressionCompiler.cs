using BadScript2.Parser.Expressions.Binary.Math.Assign;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
///     Compiles the <see cref="BadSubtractAssignExpression" />.
/// </summary>
public class BadSubtractAssignExpressionCompiler : BadBinaryExpressionCompiler<BadSubtractAssignExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadSubtractAssignExpression expression)
    {
        context.Emit(BadOpCode.SubAssign, expression.Position);
    }
}