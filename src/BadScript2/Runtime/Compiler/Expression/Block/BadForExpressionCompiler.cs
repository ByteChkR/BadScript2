using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Block;

public class BadForExpressionCompiler : BadExpressionCompiler<BadForExpression>
{
    public override int Compile(BadForExpression expr, BadCompilerResult result)
    {
        int start = result.Emit(
            new BadInstruction(
                BadOpCode.CreateScope,
                expr.Position,
                "ForLoop",
                (decimal)(BadScopeFlags.Breakable | BadScopeFlags.Continuable)
            )
        );

        BadCompiler.CompileExpression(expr.VarDef, result);

        int vCond = BadCompiler.CompileExpression(expr.Condition, result);

        int falseJump = result.Emit(new BadInstruction(BadOpCode.JumpIfFalse, expr.Position, BadObject.Null));

        result.Emit(
            new BadInstruction(
                BadOpCode.CreateScope,
                expr.Position,
                "ForLoop_Inner",
                (decimal)(BadScopeFlags.Breakable | BadScopeFlags.Continuable)
            )
        );

        foreach (BadExpression expression in expr.Body)
        {
            BadCompiler.CompileExpression(expression, result);
        }

        result.Emit(new BadInstruction(BadOpCode.DestroyScope, expr.Position));

        BadCompiler.CompileExpression(expr.VarIncrement, result);

        result.Emit(new BadInstruction(BadOpCode.Jump, expr.Position, vCond));

        int end = result.Emit(new BadInstruction(BadOpCode.Nop, expr.Position));

        result.SetArgument(falseJump, 0, end);

        return start;
    }
}