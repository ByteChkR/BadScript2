using BadScript2.Parser.Expressions.Binary.Math.Atomic;

/// <summary>
/// Contains Atomic Logic Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
///     Compiles the <see cref="BadPostDecrementExpression" />.
/// </summary>
public class BadPostDecrementExpressionCompiler : BadExpressionCompiler<BadPostDecrementExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadPostDecrementExpression expression)
    {
        context.Compile(expression.Left);
        context.Emit(BadOpCode.PostDec, expression.Position);
    }
}