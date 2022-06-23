using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.Compiler.Expression.Variables;

public class BadVariableDefinitionExpressionCompiler : BadExpressionCompiler<BadVariableDefinitionExpression>
{
    public override int Compile(BadVariableDefinitionExpression expr, BadCompilerResult result)
    {
        int start = result.Emit(new BadInstruction(BadOpCode.DefineVar, expr.Position, expr.Name));
        result.Emit(new BadInstruction(BadOpCode.PushScope, expr.Position));
        result.Emit(new BadInstruction(BadOpCode.Load, expr.Position, expr.Name));

        return start;
    }
}