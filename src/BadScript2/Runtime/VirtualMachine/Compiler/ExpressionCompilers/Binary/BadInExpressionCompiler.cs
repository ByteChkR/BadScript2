using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadInExpressionCompiler : BadBinaryExpressionCompiler<BadInExpression>
{
    protected override bool IsLeftAssociative => false;

    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadInExpression expression)
    {
        yield return new BadInstruction(BadOpCode.HasProperty, expression.Position);
    }
}