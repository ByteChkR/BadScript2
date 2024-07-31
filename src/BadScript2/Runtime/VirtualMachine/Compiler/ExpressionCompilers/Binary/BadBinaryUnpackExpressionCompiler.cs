using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadBinaryUnpackExpression" />.
/// </summary>
public class BadBinaryUnpackExpressionCompiler : BadExpressionCompiler<BadBinaryUnpackExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadBinaryUnpackExpression expression)
    {
        context.Compile(expression.Left);
        context.Compile(expression.Right);
        context.Emit(BadOpCode.BinaryUnpack, expression.Position);
    }
}