using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
///     Compiles the <see cref="BadTableExpression" />.
/// </summary>
public class BadTableExpressionCompiler : BadExpressionCompiler<BadTableExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadTableExpression expression)
    {
        foreach (KeyValuePair<BadWordToken, BadExpression> kvp in expression.Table.ToArray()
                                                                            .Reverse())
        {
            context.Emit(BadOpCode.Push, kvp.Key.SourcePosition, (BadObject)kvp.Key.Text);
            context.Compile(kvp.Value);
        }

        context.Emit(BadOpCode.TableInit, expression.Position, expression.Length);
    }
}