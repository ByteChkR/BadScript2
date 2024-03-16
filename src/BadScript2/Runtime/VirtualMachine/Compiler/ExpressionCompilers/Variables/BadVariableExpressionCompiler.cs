using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <summary>
///     Compiles the <see cref="BadVariableExpression" />.
/// </summary>
public class BadVariableExpressionCompiler : BadExpressionCompiler<BadVariableExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadVariableExpression expression)
    {
        foreach (BadExpression parameter in expression.GenericParameters)
        {
            context.Compile(parameter);
        }
        context.Emit(BadOpCode.LoadVar, expression.Position, expression.Name, expression.GenericParameters.Count);
    }
}