using BadScript2.Parser.Expressions.Binary;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadInExpression" />.
/// </summary>
public class BadInExpressionCompiler : BadBinaryExpressionCompiler<BadInExpression>
{
    /// <inheritdoc />
    protected override bool IsLeftAssociative => false;
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadInExpression expression)
    {
        context.Emit(BadOpCode.HasProperty, expression.Position);
    }
}