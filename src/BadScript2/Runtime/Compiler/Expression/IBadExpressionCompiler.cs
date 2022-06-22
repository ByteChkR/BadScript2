using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.Compiler.Expression;

public interface IBadExpressionCompiler
{
    bool CanCompile(BadExpression expression);
    int Compile(BadExpression expression, BadCompilerResult result);
}