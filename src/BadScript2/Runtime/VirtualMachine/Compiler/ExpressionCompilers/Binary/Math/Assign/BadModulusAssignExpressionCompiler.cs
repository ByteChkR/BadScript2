using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
///     Compiles the <see cref="BadModulusAssignExpression" />.
/// </summary>
public class BadModulusAssignExpressionCompiler : BadBinaryExpressionCompiler<BadModulusAssignExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadModulusAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.ModAssign, expression.Position);
    }
}