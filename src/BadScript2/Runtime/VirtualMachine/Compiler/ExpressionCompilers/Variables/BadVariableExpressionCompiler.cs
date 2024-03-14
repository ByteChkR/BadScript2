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
        context.Emit(BadOpCode.LoadVar, expression.Position, expression.Name);
    }
}