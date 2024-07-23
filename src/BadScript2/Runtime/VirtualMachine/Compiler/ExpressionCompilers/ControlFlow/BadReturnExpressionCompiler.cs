using BadScript2.Parser.Expressions.ControlFlow;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <summary>
///     Compiles the <see cref="BadReturnExpression" />.
/// </summary>
public class BadReturnExpressionCompiler : BadExpressionCompiler<BadReturnExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadReturnExpression expression)
    {
        if (expression.Right != null)
        {
            context.Compile(expression.Right);
            context.Emit(BadOpCode.Return, expression.Position, expression.IsRefReturn);
        }
        else
        {
            context.Emit(BadOpCode.Return, expression.Position);
        }
    }
}