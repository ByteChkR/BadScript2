using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions.Types;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;
/// <summary>
/// Compiles the <see cref="BadInterfacePrototypeExpression" />.
/// </summary>
public class BadInterfacePrototypeExpressionCompiler : BadExpressionCompiler<BadInterfacePrototypeExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(
        BadCompiler compiler,
        BadInterfacePrototypeExpression expression)
    {
        BadLogger.Warn(
            "Can not compile interface prototypes, emitting eval instruction",
            BadLogMask.GetMask("Compiler", "EVAL"),
            expression.Position
        );

        yield return new BadInstruction(BadOpCode.Eval, expression.Position, expression);
    }
}