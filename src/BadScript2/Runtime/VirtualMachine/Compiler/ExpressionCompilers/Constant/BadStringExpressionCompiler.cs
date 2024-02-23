using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
///     Compiles the <see cref="BadStringExpression" />.
/// </summary>
public class BadStringExpressionCompiler : BadExpressionCompiler<BadStringExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadStringExpression expression)
    {
        yield return new BadInstruction(
            BadOpCode.Push,
            expression.Position,
            (BadObject)expression.Value.Substring(1, expression.Value.Length - 2)
        );
    }
}