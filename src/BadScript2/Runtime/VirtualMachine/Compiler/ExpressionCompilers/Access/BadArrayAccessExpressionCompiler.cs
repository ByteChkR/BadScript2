using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

public class BadArrayAccessExpressionCompiler : BadExpressionCompiler<BadArrayAccessExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadArrayAccessExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Arguments, false))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        if (expression.NullChecked)
        {
            yield return new BadInstruction(
                BadOpCode.LoadArrayAccessNullChecked,
                expression.Position,
                expression.ArgumentCount
            );
        }
        else
        {
            yield return new BadInstruction(BadOpCode.LoadArrayAccess, expression.Position, expression.ArgumentCount);
        }
    }
}