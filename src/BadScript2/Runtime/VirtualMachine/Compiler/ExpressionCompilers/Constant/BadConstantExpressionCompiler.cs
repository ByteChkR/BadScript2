using BadScript2.Parser.Expressions.Constant;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
/// Compiles the <see cref="BadConstantExpression" />.
/// </summary>
public class BadConstantExpressionCompiler : BadExpressionCompiler<BadConstantExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadConstantExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Push, expression.Position, expression.Value);
    }
}