using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
///     Compiles the <see cref="BadLogicXOrExpression" />.
/// </summary>
public class BadLogicXOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicXOrExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLogicXOrExpression expression)
    {
        context.Emit(BadOpCode.XOr, expression.Position);
    }
}