using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadUsingExpression" />.
/// </summary>
public class BadUsingExpressionCompiler : BadExpressionCompiler<BadUsingExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadUsingExpression expression)
    {
        context.Emit(BadOpCode.CreateScope, expression.Position, "UsingScope", BadObject.Null);
        context.Compile(expression.Definition);
        context.Compile(expression.Expressions);
        context.Emit(BadOpCode.AddDisposeFinalizer, expression.Position, expression.Name);
        context.Emit(BadOpCode.DestroyScope, expression.Position);
    }
}