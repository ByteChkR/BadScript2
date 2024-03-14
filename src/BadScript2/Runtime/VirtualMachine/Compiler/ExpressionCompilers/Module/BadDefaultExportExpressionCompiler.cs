using BadScript2.Parser.Expressions.Module;
using BadScript2.Parser.Operators.Module;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Module;

/// <summary>
///     Compiles the <see cref="BadDefaultExportExpression" />.
/// </summary>
public class BadDefaultExportExpressionCompiler : BadExpressionCompiler<BadDefaultExportExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadDefaultExportExpression expression)
    {
        context.Compile(expression.Expression);
        if (BadExportExpressionParser.IsNamed(expression.Expression, out string? name))
        {
            if(name == null)
            {
                throw new BadCompilerException("Named export expression is null");
            }
            context.Emit(BadOpCode.LoadVar, expression.Position, name);
        }
        context.Emit(BadOpCode.Export, expression.Position);
    }
}