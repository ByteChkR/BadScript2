using BadScript2.Parser.Expressions.Access;

/// <summary>
/// Contains Access Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadArrayAccessExpression" />.
/// </summary>
public class BadArrayAccessExpressionCompiler : BadExpressionCompiler<BadArrayAccessExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadArrayAccessExpression expression)
    {
        context.Compile(expression.Arguments);
        context.Compile(expression.Left);
        if (expression.NullChecked)
        {
            context.Emit(BadOpCode.LoadArrayAccessNullChecked, expression.Position, expression.ArgumentCount);
        }
        else
        {
            context.Emit(BadOpCode.LoadArrayAccess, expression.Position, expression.ArgumentCount);
        }
    }
}