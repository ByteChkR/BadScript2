using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadUnaryUnpackExpression" />.
/// </summary>
public class BadUnaryUnpackExpressionCompiler : BadExpressionCompiler<BadUnaryUnpackExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadUnaryUnpackExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.UnaryUnpack, expression.Position);
    }
}