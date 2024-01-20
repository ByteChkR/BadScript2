using BadScript2.Parser.Expressions.Binary.Math.Assign;
/// <summary>
/// Contains Binary Self-Assignung Math Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
/// Compiles the <see cref="BadAddAssignExpression" />.
/// </summary>
public class BadAddAssignExpressionCompiler : BadBinaryExpressionCompiler<BadAddAssignExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadAddAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.AddAssign, expression.Position);
    }
}