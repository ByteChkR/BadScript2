using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <summary>
///     Compiles the <see cref="BadLogicAssignXOrExpression" />.
/// </summary>
public class BadLogicAssignXOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignXOrExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadLogicAssignXOrExpression expression)
    {
        yield return new BadInstruction(BadOpCode.XOrAssign, expression.Position);
    }
}