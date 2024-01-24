using BadScript2.Parser.Expressions.Block;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
/// Compiles the <see cref="BadUsingStatementExpression" />.
/// </summary>c
public class BadUsingStatementExpressionCompiler : BadExpressionCompiler<BadUsingStatementExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadUsingStatementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Expression)) //Compile the expression
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.AddDisposeFinalizer, expression.Position, expression.Name); //Add the finalizer
    }
}