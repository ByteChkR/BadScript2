using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Function
{
    public class BadFunctionExpressionCompiler : BadExpressionCompiler<BadFunctionExpression>
    {
        public override int Compile(BadFunctionExpression expr, BadCompilerResult result)
        {
            int patchIndex1 = -1;
            if (expr.Name != null)
            {
                result.Emit(new BadInstruction(BadOpCode.DefineVar, expr.Position, expr.Name.Text));
                result.Emit(new BadInstruction(BadOpCode.PushScope, expr.Position));
                result.Emit(new BadInstruction(BadOpCode.Load, expr.Position, expr.Name.Text));
                patchIndex1 = result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.Null));
                result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));
            }

            int patchIndex2 = result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.Null));
            result.AddWorkItem(
                $"Compile Function: {expr.GetHeader()}",
                () =>
                {
                    BadLogger.Log($"Compiling {expr}", "Runtime");
                    BadCompiler.CompileFunction(expr, out int start, out int len, result);
                    BadCompiledFunction func = new BadCompiledFunction(
                        expr.Name,
                        result.GetScope(),
                        expr.Position,
                        result,
                        start,
                        len,
                        expr.Parameters.ToArray()
                    );
                    if (patchIndex1 != -1)
                    {
                        result.SetArgument(
                            patchIndex1,
                            0,
                            func
                        );
                    }

                    result.SetArgument(
                        patchIndex2,
                        0,
                        func
                    );
                }
            );


            return patchIndex1 == -1 ? patchIndex2 : patchIndex1;
        }
    }
}