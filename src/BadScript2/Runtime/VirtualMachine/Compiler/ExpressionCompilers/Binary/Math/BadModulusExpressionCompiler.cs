using BadScript2.Parser.Expressions.Binary.Math;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
///     Compiles the <see cref="BadModulusExpression" />.
/// </summary>
public class BadModulusExpressionCompiler : BadBinaryExpressionCompiler<BadModulusExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadModulusExpression expression)
    {
        context.Emit(BadOpCode.Mod, expression.Position);
    }
}