using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadMemberAccessExpression" />.
/// </summary>
public class BadMemberAccessExpressionCompiler : BadExpressionCompiler<BadMemberAccessExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadMemberAccessExpression expression)
    {
        foreach (BadExpression? parameter in expression.GenericArguments)
        {
            context.Compile(parameter);
        }
        context.Compile(expression.Left);
        if (expression.NullChecked)
        {
            context.Emit(BadOpCode.LoadMemberNullChecked, expression.Position, expression.Right.Text, expression.GenericArguments.Count);
        }
        else
        {
            context.Emit(BadOpCode.LoadMember, expression.Position, expression.Right.Text, expression.GenericArguments.Count);
        }
    }
}