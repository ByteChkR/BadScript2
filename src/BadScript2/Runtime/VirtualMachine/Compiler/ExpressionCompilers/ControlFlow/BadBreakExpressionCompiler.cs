using BadScript2.Parser.Expressions.ControlFlow;
/// <summary>
/// Contains Controlflow Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <summary>
/// Compiles the <see cref="BadBreakExpression" />.
/// </summary>
public class BadBreakExpressionCompiler : BadExpressionCompiler<BadBreakExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBreakExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Break, expression.Position);
    }
}