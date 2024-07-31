using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions.Types;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;

/// <summary>
///     Compiles the <see cref="BadInterfacePrototypeExpression" />.
/// </summary>
public class BadInterfacePrototypeExpressionCompiler : BadExpressionCompiler<BadInterfacePrototypeExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadInterfacePrototypeExpression expression)
    {
        BadLogger.Warn("Can not compile interface prototypes, emitting eval instruction",
                       BadLogMask.GetMask("Compiler", "EVAL"),
                       expression.Position
                      );

        context.Emit(BadOpCode.Eval, expression.Position, expression);
    }
}