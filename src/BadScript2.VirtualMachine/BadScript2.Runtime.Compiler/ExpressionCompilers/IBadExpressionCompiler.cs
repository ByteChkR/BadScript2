using System.Collections.Generic;

using BadScript2.Parser.Expressions;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers;

public interface IBadExpressionCompiler
{
    IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadExpression expression);
}