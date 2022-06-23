using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.Compiler.Expression.ControlFlow;

public class BadThrowExpressionCompiler : BadExpressionCompiler<BadThrowExpression>
{
    public override int Compile(BadThrowExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Right, result);
        result.Emit(new BadInstruction(BadOpCode.Throw, expr.Position));

        return start;
    }
}