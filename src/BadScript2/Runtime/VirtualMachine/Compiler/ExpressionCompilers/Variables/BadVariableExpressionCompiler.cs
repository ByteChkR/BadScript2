using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadVariableExpressionCompiler : BadExpressionCompiler<BadVariableExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadVariableExpression expression)
    {
        yield return new BadInstruction(BadOpCode.LoadVar, expression.Position, (BadObject)expression.Name);
    }
}