using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
///     Compiles the <see cref="BadBooleanExpression" />.
/// </summary>
public class BadBooleanExpressionCompiler : BadExpressionCompiler<BadBooleanExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadBooleanExpression expression)
    {
        context.Emit(BadOpCode.Push, expression.Position, expression.Value ? BadObject.True : BadObject.False);
    }
}