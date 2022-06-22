using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math.Assign;

public class BadAddAssignExpressionCompiler : BadExpressionCompiler<BadAddAssignExpression>
{
    public override int Compile(BadAddAssignExpression expr, BadCompilerResult result)
    {
        // compile a
        int start = BadCompiler.CompileExpression(expr.Left, result); // Remains on stack as reference to Left
        result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For Assign
        result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For ADD

        // compile b
        BadCompiler.CompileExpression(expr.Right, result);
        result.Emit(new BadInstruction(BadOpCode.Add, expr.Position));
        result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));

        return start;
    }
}