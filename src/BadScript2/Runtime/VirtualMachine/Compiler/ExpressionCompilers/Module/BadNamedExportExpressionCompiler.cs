using BadScript2.Parser.Expressions.Module;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Module;

/// <summary>
///     Compiles the <see cref="BadNamedExportExpression" />.
/// </summary>
public class BadNamedExportExpressionCompiler : BadExpressionCompiler<BadNamedExportExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNamedExportExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Expression))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Export, expression.Position, expression.Name!);
    }
}