using BadScript2.Parser.Expressions.Constant;
/// <summary>
/// Contains Constant Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadArrayExpressionCompiler : BadExpressionCompiler<BadArrayExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadArrayExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.InitExpressions, false))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.ArrayInit, expression.Position, expression.Length);
    }
}