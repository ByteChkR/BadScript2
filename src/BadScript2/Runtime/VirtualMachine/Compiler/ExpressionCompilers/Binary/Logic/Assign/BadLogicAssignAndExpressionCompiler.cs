using BadScript2.Parser.Expressions.Binary.Logic.Assign;

/// <summary>
/// Contains Binary Self-Assignung Logic Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <summary>
///     Compiles the <see cref="BadLogicAssignAndExpression" />.
/// </summary>
public class BadLogicAssignAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignAndExpression>
{
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLogicAssignAndExpression expression)
    {
        context.Emit(BadOpCode.AndAssign, expression.Position);
    }
}