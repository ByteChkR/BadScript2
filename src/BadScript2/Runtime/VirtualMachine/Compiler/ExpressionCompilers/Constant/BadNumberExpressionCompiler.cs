using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
///     Compiles the <see cref="BadNumberExpression" />.
/// </summary>
public class BadNumberExpressionCompiler : BadExpressionCompiler<BadNumberExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadNumberExpression expression)
    {
        context.Emit(BadOpCode.Push, expression.Position, (BadObject)expression.Value);
    }
}