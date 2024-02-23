using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
///     Compiles the <see cref="BadExponentiationExpression" />.
/// </summary>
public class BadExponentiationExpressionCompiler : BadBinaryExpressionCompiler<BadExponentiationExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadExponentiationExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Exp, expression.Position);
    }
}