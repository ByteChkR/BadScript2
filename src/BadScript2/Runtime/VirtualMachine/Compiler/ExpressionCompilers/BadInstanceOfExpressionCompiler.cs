using BadScript2.Parser.Expressions;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;


/// <summary>
/// Compiles the <see cref="BadInstanceOfExpression" />.
/// </summary>
public class BadInstanceOfExpressionCompiler : BadBinaryExpressionCompiler<BadInstanceOfExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadInstanceOfExpression expression)
    {
        yield return new BadInstruction(BadOpCode.InstanceOf, expression.Position);
    }
}