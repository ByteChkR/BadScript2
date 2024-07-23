using BadScript2.Parser.Expressions;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Compiles the <see cref="BadTypeOfExpression" />.
/// </summary>
public class BadTypeOfExpressionCompiler : BadExpressionCompiler<BadTypeOfExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadTypeOfExpression expression)
    {
        context.Compile(expression.Expression);
        context.Emit(BadOpCode.TypeOf, expression.Position);
    }
}