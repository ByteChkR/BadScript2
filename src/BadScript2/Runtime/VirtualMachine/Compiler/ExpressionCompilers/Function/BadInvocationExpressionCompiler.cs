using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadInvocationExpressionCompiler : BadExpressionCompiler<BadInvocationExpression>
{
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