using BadScript2.Parser.Expressions.Binary.Logic;

/// <summary>
/// Contains Binary Logic Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
///     Compiles the <see cref="BadLogicAndExpression" />.
/// </summary>
public class BadLogicAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAndExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLogicAndExpression expression)
    {
        context.Emit(BadOpCode.And, expression.Position);
    }
}