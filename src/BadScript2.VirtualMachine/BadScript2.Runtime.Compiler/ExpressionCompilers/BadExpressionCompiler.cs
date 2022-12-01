using System.Collections.Generic;

using BadScript2.Parser.Expressions;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers;

public abstract class BadExpressionCompiler<T> : IBadExpressionCompiler
    where T : BadExpression
{
    IEnumerable<BadInstruction> IBadExpressionCompiler.Compile(BadCompiler compiler, BadExpression expression)
    {
        if (expression.GetType() != typeof(T))
        {
            throw new BadCompilerException("Invalid Expression Type");
        }

        return Compile(compiler, (T)expression);
    }

    public abstract IEnumerable<BadInstruction> Compile(BadCompiler compiler, T expression);
}