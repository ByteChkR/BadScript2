using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
///     Compiles the <see cref="BadLogicNotExpression" />.
/// </summary>
public class BadLogicNotExpressionCompiler : BadExpressionCompiler<BadLogicNotExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadLogicNotExpression expression)
    {
        context.Compile(expression.Right);
        context.Emit(BadOpCode.Not, expression.Position);
    }
}