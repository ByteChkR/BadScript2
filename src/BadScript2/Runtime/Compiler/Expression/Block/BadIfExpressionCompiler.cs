using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Block
{
    public class BadIfExpressionCompiler : BadExpressionCompiler<BadIfExpression>
    {
        public override int Compile(BadIfExpression expr, BadCompilerResult result)
        {
            int start = -1;
            List<int> endJumps = new List<int>();
            foreach (KeyValuePair<BadExpression, BadExpression[]> kvp in expr.ConditionalBranches)
            {
                int condition = BadCompiler.CompileExpression(kvp.Key, result);
                if (start == -1)
                {
                    start = condition;
                }

                //Evaluate Condition
                //  If false, jump to end of body
                //  If true, evaluate body and jump to end of if
                int falseJump = result.Emit(new BadInstruction(BadOpCode.JumpIfFalse, kvp.Key.Position, BadObject.Null));


                result.Emit(
                    new BadInstruction(
                        BadOpCode.CreateScope,
                        expr.Position,
                        "IfBranch",
                        0
                    )
                );
                foreach (BadExpression expression in kvp.Value)
                {
                    BadCompiler.CompileExpression(expression, result);
                }

                result.Emit(new BadInstruction(BadOpCode.DestroyScope, expr.Position));

                endJumps.Add(result.Emit(new BadInstruction(BadOpCode.Jump, kvp.Key.Position, BadObject.Null)));

                int end = result.Emit(new BadInstruction(BadOpCode.Nop, kvp.Key.Position));
                result.SetArgument(falseJump, 0, end);
            }

            if (expr.ElseBranch != null)
            {
                result.Emit(
                    new BadInstruction(
                        BadOpCode.CreateScope,
                        expr.Position,
                        "IfBranch",
                        0
                    )
                );
                foreach (BadExpression expression in expr.ElseBranch)
                {
                    BadCompiler.CompileExpression(expression, result);
                }

                result.Emit(new BadInstruction(BadOpCode.DestroyScope, expr.Position));
            }

            int ifEnd = result.Emit(new BadInstruction(BadOpCode.Nop, expr.Position));

            foreach (int endJump in endJumps)
            {
                result.SetArgument(endJump, 0, ifEnd);
            }

            return start;
        }
    }
}