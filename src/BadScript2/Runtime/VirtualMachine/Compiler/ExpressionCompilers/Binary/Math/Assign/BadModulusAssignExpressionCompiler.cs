using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
///     Compiles the <see cref="BadModulusAssignExpression" />.
/// </summary>
public class BadModulusAssignExpressionCompiler : BadBinaryExpressionCompiler<BadModulusAssignExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadModulusAssignExpression expression)
    {
        context.Emit(BadOpCode.ModAssign, expression.Position);
    }
}