using BadScript2.Parser.Expressions.Binary;

/// <summary>
/// Contains Binary Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadAssignExpression" />.
/// </summary>
public class BadAssignExpressionCompiler : BadExpressionCompiler<BadAssignExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadAssignExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Assign, expression.Position);
    }
}