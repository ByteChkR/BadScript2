using BadScript2.Parser.Expressions.Module;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Module;

/// <summary>
///     Compiles the <see cref="BadImportExpression" />.
/// </summary>
public class BadImportExpressionCompiler : BadExpressionCompiler<BadImportExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadImportExpression expression)
    {
        context.Emit(BadOpCode.Import, expression.Position, expression.Name, expression.Path);
    }
}