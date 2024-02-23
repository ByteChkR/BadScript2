using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
///     Compiles the <see cref="BadLogicNotExpression" />.
/// </summary>
public class BadLogicNotExpressionCompiler : BadExpressionCompiler<BadLogicNotExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadLogicNotExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Not, expression.Position);
    }
}