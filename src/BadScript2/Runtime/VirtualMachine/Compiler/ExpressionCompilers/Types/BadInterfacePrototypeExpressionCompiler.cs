using BadScript2.Parser.Expressions.Types;
using BadScript2.Runtime.VirtualMachine.Compiler;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;

/// <summary>
///     Compiles the <see cref="BadInterfacePrototypeExpression" />.
/// </summary>
public class BadInterfacePrototypeExpressionCompiler : BadExpressionCompiler<BadInterfacePrototypeExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadInterfacePrototypeExpression expression)
    {
        BadCompiledInterfaceTemplate template = new BadCompiledInterfaceTemplate(expression);
        context.Emit(BadOpCode.CreateInterface, expression.Position, template);
    }
}