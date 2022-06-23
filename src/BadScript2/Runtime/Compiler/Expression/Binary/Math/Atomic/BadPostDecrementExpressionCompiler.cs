using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math.Atomic;

public class BadPostDecrementExpressionCompiler : BadExpressionCompiler<BadPostDecrementExpression>
{
    public override int Compile(BadPostDecrementExpression expr, BadCompilerResult result)
    {
        //compile left
        //duplicate leftRef for assign
        //dereference left
        //duplicate left for add

        // compile a
        int start = BadCompiler.CompileExpression(expr.Left, result); // Remains on stack as reference to Left
        result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For Assign
        result.Emit(new BadInstruction(BadOpCode.Dereference, expr.Position)); //For Add
        result.Emit(new BadInstruction(BadOpCode.Swap, expr.Position)); //Move Dereferenced to top of stack
        result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For ADD

        // compile b
        result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, 1));
        result.Emit(new BadInstruction(BadOpCode.Sub, expr.Position));
        result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));

        return start;
    }
}