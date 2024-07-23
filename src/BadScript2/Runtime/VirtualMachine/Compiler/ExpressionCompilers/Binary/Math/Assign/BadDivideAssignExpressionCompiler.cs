using BadScript2.Parser.Expressions.Binary.Math.Assign;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
///     Compiles the <see cref="BadDivideAssignExpression" />.
/// </summary>
public class BadDivideAssignExpressionCompiler : BadBinaryExpressionCompiler<BadDivideAssignExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadDivideAssignExpression expression)
    {
        context.Emit(BadOpCode.DivAssign, expression.Position);
    }
}