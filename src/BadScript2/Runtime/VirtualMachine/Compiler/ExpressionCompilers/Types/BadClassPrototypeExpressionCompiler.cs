using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions.Types;
/// <summary>
/// Contains Type Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;

public class BadClassPrototypeExpressionCompiler : BadExpressionCompiler<BadClassPrototypeExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadClassPrototypeExpression expression)
    {
        BadLogger.Warn(
            "Can not compile class prototypes, emitting eval instruction",
            BadLogMask.GetMask("Compiler", "EVAL"),
            expression.Position
        );

        yield return new BadInstruction(BadOpCode.Eval, expression.Position, expression);
    }
}