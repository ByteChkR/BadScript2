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
        context.Compile(expression.Left);
        if (expression.NullChecked)
        {
            context.Emit(BadOpCode.LoadMemberNullChecked, expression.Position, expression.Right.Text);
        }
        else
        {
            context.Emit(BadOpCode.LoadMember, expression.Position, expression.Right.Text);
        }
    }
}