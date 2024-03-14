using BadScript2.Parser.Expressions.ControlFlow;

/// <summary>
/// Contains Controlflow Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <summary>
///     Compiles the <see cref="BadBreakExpression" />.
/// </summary>
public class BadBreakExpressionCompiler : BadExpressionCompiler<BadBreakExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadBreakExpression expression)
    {
        context.Emit(BadOpCode.Break, expression.Position);
    }
}