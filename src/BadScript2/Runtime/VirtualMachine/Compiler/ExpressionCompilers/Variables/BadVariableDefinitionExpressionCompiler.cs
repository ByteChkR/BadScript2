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
        if (expression.Attributes.Any())
        {
            throw new BadCompilerException("Attributes are not supported yet.");
        }
        if (expression.TypeExpression == null)
        {
            context.Emit(BadOpCode.DefVar, expression.Position, expression.Name, expression.IsReadOnly);
        }
        else
        {
            context.Compile(expression.TypeExpression);
            context.Emit(BadOpCode.DefVarTyped, expression.Position, expression.Name, expression.IsReadOnly);
        }
    }
}