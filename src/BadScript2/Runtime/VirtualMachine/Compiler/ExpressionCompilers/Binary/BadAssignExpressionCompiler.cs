using BadScript2.Parser.Expressions.Binary;

/// <summary>
/// Contains Binary Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadAssignExpression" />.
/// </summary>
public class BadAssignExpressionCompiler : BadExpressionCompiler<BadAssignExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadAssignExpression expression)
    {
        context.Compile(expression.Left);
        context.Compile(expression.Right);
        context.Emit(BadOpCode.Assign, expression.Position);
    }
}