using BadScript2.Common.Logging;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

namespace BadScript2.Interop.Common.Task;

public class BadAwaitExpressionCompiler : BadExpressionCompiler<BadAwaitExpression>
{
    public override void Compile(BadExpressionCompileContext context, BadAwaitExpression expression)
    {
        BadLogger.Warn($"Can not compile await expression '{expression}'",
            BadLogMask.GetMask("Compiler", "EVAL"),
            expression.Position
        );
        context.Emit(BadOpCode.Eval, expression.Position, expression);
    }
}