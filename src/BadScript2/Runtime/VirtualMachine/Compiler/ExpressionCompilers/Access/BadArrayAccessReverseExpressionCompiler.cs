using BadScript2.Parser.Expressions.Access;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadArrayAccessReverseExpression" />.
/// </summary>
public class BadArrayAccessReverseExpressionCompiler : BadExpressionCompiler<BadArrayAccessReverseExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadArrayAccessReverseExpression expression)
    {
        context.Compile(expression.Arguments);
        context.Compile(expression.Left);
        if (expression.NullChecked)
        {
            context.Emit(BadOpCode.LoadArrayAccessReverseNullChecked, expression.Position, expression.ArgumentCount);
        }
        else
        {
            context.Emit(BadOpCode.LoadArrayAccessReverse, expression.Position, expression.ArgumentCount);
        }
    }
}