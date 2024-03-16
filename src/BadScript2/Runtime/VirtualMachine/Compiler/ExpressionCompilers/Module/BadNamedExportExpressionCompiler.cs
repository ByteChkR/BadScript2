using BadScript2.Parser.Expressions.Module;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Module;

/// <summary>
///     Compiles the <see cref="BadNamedExportExpression" />.
/// </summary>
public class BadNamedExportExpressionCompiler : BadExpressionCompiler<BadNamedExportExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadNamedExportExpression expression)
    {
        context.Compile(expression.Expression);
        context.Emit(BadOpCode.Export, expression.Position, expression.Name!);
    }
}