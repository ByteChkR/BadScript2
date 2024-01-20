using BadScript2.Parser.Expressions.ControlFlow;
/// <summary>
/// Contains Controlflow Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadBreakExpressionCompiler : BadExpressionCompiler<BadBreakExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBreakExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Break, expression.Position);
    }
}