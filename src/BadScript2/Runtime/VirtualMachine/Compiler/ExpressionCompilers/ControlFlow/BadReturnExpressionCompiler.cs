using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

public class BadReturnExpressionCompiler : BadExpressionCompiler<BadReturnExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadReturnExpression expression)
    {
        if (expression.Right != null)
        {
            foreach (BadInstruction instruction in compiler.Compile(expression.Right))
            {
                yield return instruction;
            }

            yield return new BadInstruction(BadOpCode.Return, expression.Position, expression.IsRefReturn);
        }
        else
        {
            yield return new BadInstruction(BadOpCode.Return, expression.Position);
        }
    }
}