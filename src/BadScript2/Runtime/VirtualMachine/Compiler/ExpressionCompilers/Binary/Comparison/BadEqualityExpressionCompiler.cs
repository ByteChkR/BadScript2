using BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
/// Contains Binary Comparison Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <summary>
///     Compiles the <see cref="BadEqualityExpression" />.
/// </summary>
public class BadEqualityExpressionCompiler : BadBinaryExpressionCompiler<BadEqualityExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadEqualityExpression expression)
    {
        context.Emit(BadOpCode.Equals, expression.Position);
    }
}