using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Constant;

public class BadNullExpressionCompiler : BadExpressionCompiler<BadNullExpression>
{
    public override int Compile(BadNullExpression expr, BadCompilerResult result)
    {
        return result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.Null));
    }
}