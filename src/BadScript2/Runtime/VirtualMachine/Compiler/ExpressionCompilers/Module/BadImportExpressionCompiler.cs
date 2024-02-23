using BadScript2.Parser.Expressions.Module;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Module;

/// <summary>
///     Compiles the <see cref="BadImportExpression" />.
/// </summary>
public class BadImportExpressionCompiler : BadExpressionCompiler<BadImportExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadImportExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Import, expression.Position, expression.Name, expression.Path);
    }
}