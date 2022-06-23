using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math;

public class BadMultiplyExpressionCompiler : BadExpressionCompiler<BadMultiplyExpression>
{
    public override int Compile(BadMultiplyExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Left, result);
        BadCompiler.CompileExpression(expr.Right, result);

        result.Emit(new BadInstruction(BadOpCode.Mul, expr.Position));

        return start;
    }
}