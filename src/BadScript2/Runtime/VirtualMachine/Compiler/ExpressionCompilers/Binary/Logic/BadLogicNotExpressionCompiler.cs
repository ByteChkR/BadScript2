using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadLogicNotExpressionCompiler : BadExpressionCompiler<BadLogicNotExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadLogicNotExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Not, expression.Position);
    }
}