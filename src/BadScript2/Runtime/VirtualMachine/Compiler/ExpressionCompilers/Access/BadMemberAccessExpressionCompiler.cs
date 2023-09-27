using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

public class BadMemberAccessExpressionCompiler : BadExpressionCompiler<BadMemberAccessExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadMemberAccessExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        if (expression.NullChecked)
        {
            yield return new BadInstruction(
                BadOpCode.LoadMemberNullChecked,
                expression.Position,
                expression.Right.Text
            );
        }
        else
        {
            yield return new BadInstruction(BadOpCode.LoadMember, expression.Position, expression.Right.Text);
        }
    }
}