using BadScript2.Parser.Expressions.Binary.Math;

/// <summary>
/// Contains Binary Math Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
///     Compiles the <see cref="BadAddExpression" />.
/// </summary>
public class BadAddExpressionCompiler : BadBinaryExpressionCompiler<BadAddExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadAddExpression expression)
    {
        context.Emit(BadOpCode.Add, expression.Position);
    }
}