using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;

/// <summary>
/// Compiles the <see cref="BadInvocationExpression" />.
/// </summary>
public class BadInvocationExpressionCompiler : BadExpressionCompiler<BadInvocationExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadInvocationExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Arguments, false))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Invoke, expression.Position, expression.ArgumentCount);
    }
}