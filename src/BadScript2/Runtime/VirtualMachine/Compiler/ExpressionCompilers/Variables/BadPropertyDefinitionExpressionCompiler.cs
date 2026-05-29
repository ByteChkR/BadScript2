using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <summary>
///     Compiles the <see cref="BadPropertyDefinitionExpression" />.
/// </summary>
public class BadPropertyDefinitionExpressionCompiler : BadExpressionCompiler<BadPropertyDefinitionExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadPropertyDefinitionExpression expression)
    {
        bool hasTypeExpression = expression.TypeExpression != null;

        if (hasTypeExpression)
        {
            context.Compile(expression.TypeExpression!);
        }

        context.Emit(BadOpCode.DefineProperty,
                     expression.Position,
                     expression.Name.Text,
                     expression.GetExpression,
                     expression.SetExpression!,
                     expression.Attributes.ToArray(),
                     hasTypeExpression
                    );
    }
}
