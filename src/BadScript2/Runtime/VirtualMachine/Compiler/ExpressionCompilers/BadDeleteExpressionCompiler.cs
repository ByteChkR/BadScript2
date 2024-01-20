using BadScript2.Parser.Expressions;
/// <summary>
/// Contains Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;


/// <summary>
/// Compiles the <see cref="BadDeleteExpression" />.
/// </summary>
public class BadDeleteExpressionCompiler : BadExpressionCompiler<BadDeleteExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadDeleteExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Expression))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Delete, expression.Position);
    }
}