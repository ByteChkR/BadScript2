using BadScript2.Parser.Expressions.Binary.Comparison;
/// <summary>
/// Contains Binary Comparison Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadEqualityExpressionCompiler : BadBinaryExpressionCompiler<BadEqualityExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadEqualityExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Equals, expression.Position);
    }
}