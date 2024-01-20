using BadScript2.Parser.Expressions.Binary.Math;
/// <summary>
/// Contains Binary Math Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadAddExpressionCompiler : BadBinaryExpressionCompiler<BadAddExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadAddExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Add, expression.Position);
    }
}