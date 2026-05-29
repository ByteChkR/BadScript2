using BadScript2.Parser.Expressions.Types;
using BadScript2.Runtime.VirtualMachine.Compiler;

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
        BadCompiledClassTemplate template = new BadCompiledClassTemplate(expression);
        context.Emit(BadOpCode.CreateClass, expression.Position, template);
    }
}