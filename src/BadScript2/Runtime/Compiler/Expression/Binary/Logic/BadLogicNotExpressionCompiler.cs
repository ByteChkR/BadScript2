using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Logic;

public class BadLogicNotExpressionCompiler : BadExpressionCompiler<BadLogicNotExpression>
{
    public override int Compile(BadLogicNotExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Right, result);

        result.Emit(new BadInstruction(BadOpCode.Not, expr.Position));

        return start;
    }
}