using BadScript2.Parser.Expressions.Binary.Logic.Assign;
/// <summary>
/// Contains Binary Self-Assignung Logic Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
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