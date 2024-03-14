using BadScript2.Parser.Expressions;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Compiles the <see cref="BadInstanceOfExpression" />.
/// </summary>
public class BadInstanceOfExpressionCompiler : BadBinaryExpressionCompiler<BadInstanceOfExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadInstanceOfExpression expression)
    {
        context.Emit(BadOpCode.InstanceOf, expression.Position);
    }
}