using BadScript2.Parser.Expressions.Binary.Math.Atomic;
/// <summary>
/// Contains Atomic Logic Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
/// Compiles the <see cref="BadPostDecrementExpression" />.
/// </summary>
public class BadPostDecrementExpressionCompiler : BadExpressionCompiler<BadPostDecrementExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPostDecrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PostDec, expression.Position);
    }
}