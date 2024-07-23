using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
///     Compiles the <see cref="BadNullExpression" />.
/// </summary>
public class BadNullExpressionCompiler : BadExpressionCompiler<BadNullExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadNullExpression expression)
    {
        context.Emit(BadOpCode.Push, expression.Position, BadObject.Null);
    }
}