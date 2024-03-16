using BadScript2.Parser.Expressions.Types;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;

/// <summary>
///     Compiles the <see cref="BadNewExpression" />.
/// </summary>
public class BadNewExpressionCompiler : BadExpressionCompiler<BadNewExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadNewExpression expression)
    {
        context.Compile(expression.Right.Arguments, false);
        context.Compile(expression.Right.Left);
        context.Emit(BadOpCode.New, expression.Position, expression.Right.ArgumentCount);
    }
}