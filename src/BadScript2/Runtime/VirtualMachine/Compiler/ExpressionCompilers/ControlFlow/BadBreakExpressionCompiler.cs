using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

public class BadBreakExpressionCompiler : BadExpressionCompiler<BadBreakExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBreakExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Break, expression.Position);
    }
}