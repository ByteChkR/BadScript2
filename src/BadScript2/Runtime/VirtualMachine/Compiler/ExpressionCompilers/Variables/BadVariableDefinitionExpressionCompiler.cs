using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <summary>
///     Compiles the <see cref="BadVariableDefinitionExpression" />.
/// </summary>
public class BadVariableDefinitionExpressionCompiler : BadExpressionCompiler<BadVariableDefinitionExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(
        BadCompiler compiler,
        BadVariableDefinitionExpression expression)
    {
        if (expression.TypeExpression == null)
        {
            yield return new BadInstruction(
                BadOpCode.DefVar,
                expression.Position,
                expression.Name,
                expression.IsReadOnly
            );
        }
        else
        {
            foreach (BadInstruction instruction in compiler.Compile(expression.TypeExpression))
            {
                yield return instruction;
            }

            yield return new BadInstruction(
                BadOpCode.DefVarTyped,
                expression.Position,
                expression.Name,
                expression.IsReadOnly
            );
        }
    }
}