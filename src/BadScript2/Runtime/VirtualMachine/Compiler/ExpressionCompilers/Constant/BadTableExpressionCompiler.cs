using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

public class BadTableExpressionCompiler : BadExpressionCompiler<BadTableExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadTableExpression expression)
    {
        foreach (KeyValuePair<BadWordToken, BadExpression> kvp in expression.Table.ToArray().Reverse())
        {
            yield return new BadInstruction(BadOpCode.Push, kvp.Key.SourcePosition, (BadObject)kvp.Key.Text);

            foreach (BadInstruction instruction in compiler.Compile(kvp.Value))
            {
                yield return instruction;
            }
        }

        yield return new BadInstruction(BadOpCode.TableInit, expression.Position, expression.Length);
    }
}