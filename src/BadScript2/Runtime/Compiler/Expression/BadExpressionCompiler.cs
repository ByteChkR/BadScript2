using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;

namespace BadScript2.Runtime.Compiler.Expression
{
    public abstract class BadExpressionCompiler<T> : IBadExpressionCompiler
        where T : BadExpression
    {
        public bool CanCompile(BadExpression expression)
        {
            return expression is T;
        }

        int IBadExpressionCompiler.Compile(BadExpression expression, BadCompilerResult result)
        {
            if (CanCompile(expression))
            {
                return Compile((T)expression, result);
            }

            throw new BadRuntimeException("Cannot compile expression");
        }

        public abstract int Compile(T expr, BadCompilerResult result);
    }
}