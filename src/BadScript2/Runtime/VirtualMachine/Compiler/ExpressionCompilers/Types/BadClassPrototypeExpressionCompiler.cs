using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions.Types;

/// <summary>
/// Contains Type Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;

/// <summary>
///     Compiles the <see cref="BadClassPrototypeExpression" />.
/// </summary>
public class BadClassPrototypeExpressionCompiler : BadExpressionCompiler<BadClassPrototypeExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadClassPrototypeExpression expression)
    {
        BadLogger.Warn("Can not compile class prototypes, emitting eval instruction",
                       BadLogMask.GetMask("Compiler", "EVAL"),
                       expression.Position
                      );

        context.Emit(BadOpCode.Eval, expression.Position, expression);
    }
}