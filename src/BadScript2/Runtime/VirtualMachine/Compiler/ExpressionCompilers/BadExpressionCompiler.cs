using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

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