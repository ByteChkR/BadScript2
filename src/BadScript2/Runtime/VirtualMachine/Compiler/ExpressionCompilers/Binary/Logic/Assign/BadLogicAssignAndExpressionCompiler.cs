using BadScript2.Parser.Expressions.Binary.Logic.Assign;
/// <summary>
/// Contains Binary Self-Assignung Logic Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <summary>
/// Compiles the <see cref="BadLogicAssignAndExpression" />.
/// </summary>
public class BadLogicAssignAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignAndExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadLogicAssignAndExpression expression)
    {
        yield return new BadInstruction(BadOpCode.AndAssign, expression.Position);
    }
}