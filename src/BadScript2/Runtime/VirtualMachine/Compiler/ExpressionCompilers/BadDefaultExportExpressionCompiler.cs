using BadScript2.Parser.Expressions.Module;
using BadScript2.Parser.Operators;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Compiles the <see cref="BadDefaultExportExpression" />.
/// </summary>
public class BadDefaultExportExpressionCompiler : BadExpressionCompiler<BadDefaultExportExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadDefaultExportExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Expression))
        {
            yield return instruction;
        }

        if (BadExportExpressionParser.IsNamed(expression.Expression, out string? name))
        {
            //Was named export but was overridden with the default keyword
            //Load variable onto the stack
            if (name == null)
            {
                throw new BadCompilerException("Named export expression is null");
            }

            yield return new BadInstruction(BadOpCode.LoadVar, expression.Position, (BadObject)name);
        }

        yield return new BadInstruction(BadOpCode.Export, expression.Position);
    }
}