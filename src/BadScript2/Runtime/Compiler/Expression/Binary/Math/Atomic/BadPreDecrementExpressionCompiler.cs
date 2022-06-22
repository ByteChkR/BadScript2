using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math.Atomic
{
    public class BadPreDecrementExpressionCompiler : BadExpressionCompiler<BadPreDecrementExpression>
    {
        public override int Compile(BadPreDecrementExpression expr, BadCompilerResult result)
        {
            //compile left
            //duplicate leftRef for assign
            //dereference left
            //duplicate left for add

            // compile a
            int start = BadCompiler.CompileExpression(expr.Right, result); // Remains on stack as reference to Left
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For Assign
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For ADD

            // compile b
            result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, 1));
            result.Emit(new BadInstruction(BadOpCode.Sub, expr.Position));
            result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));

            return start;
        }
    }
}