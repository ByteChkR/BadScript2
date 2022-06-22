using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Comparison;

public class BadLessThanExpressionCompiler : BadExpressionCompiler<BadLessThanExpression>
{
    public override int Compile(BadLessThanExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Left, result);
        BadCompiler.CompileExpression(expr.Right, result);
        result.Emit(new BadInstruction(BadOpCode.LessThan, expr.Position));

        return start;
    }
}