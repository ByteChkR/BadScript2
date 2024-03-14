using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <summary>
///     Compiles the <see cref="BadVariableExpression" />.
/// </summary>
public class BadVariableExpressionCompiler : BadExpressionCompiler<BadVariableExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadVariableExpression expression)
    {
        yield return new BadInstruction(BadOpCode.LoadVar, expression.Position, expression.Name);
    }
}