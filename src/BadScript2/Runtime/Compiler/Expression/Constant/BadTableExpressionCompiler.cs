using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Reader.Token;

namespace BadScript2.Runtime.Compiler.Expression.Constant;

public class BadTableExpressionCompiler : BadExpressionCompiler<BadTableExpression>
{
    public override int Compile(BadTableExpression expr, BadCompilerResult result)
    {
        int start = -1;
        foreach (KeyValuePair<BadWordToken, BadExpression> elem in expr.Table)
        {
            int elemKey = result.Emit(new BadInstruction(BadOpCode.Push, elem.Key.SourcePosition, elem.Key.Text));
            if (start == -1)
            {
                start = elemKey;
            }

            BadCompiler.CompileExpression(elem.Value, result);
        }

        int create = result.Emit(new BadInstruction(BadOpCode.CreateTable, expr.Position, expr.Table.Count));
        if (start == -1)
        {
            start = create;
        }

        return start;
    }
}