using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <summary>
///     Compiles the <see cref="BadVariableDefinitionExpression" />.
/// </summary>
public class BadVariableDefinitionExpressionCompiler : BadExpressionCompiler<BadVariableDefinitionExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadVariableDefinitionExpression expression)
    {
        BadExpression[] attributes = expression.Attributes.ToArray();

        if (expression.TypeExpression == null)
        {
            context.Emit(BadOpCode.DefVar, expression.Position, expression.Name, expression.IsReadOnly, attributes);
        }
        else
        {
            context.Compile(expression.TypeExpression);
            context.Emit(BadOpCode.DefVarTyped,
                         expression.Position,
                         expression.Name,
                         expression.IsReadOnly,
                         attributes
                        );
        }
    }
}
