using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary;

public abstract class BadBinaryExpressionCompiler<T> : BadExpressionCompiler<T>
    where T : BadBinaryExpression
{
    public abstract IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, T expression);

    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, T expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in CompileBinary(compiler, expression))
        {
            yield return instruction;
        }
    }
}