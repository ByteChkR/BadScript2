using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadTernaryExpression" />.
/// </summary>
public class BadTernaryExpressionCompiler : BadExpressionCompiler<BadTernaryExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadTernaryExpression expression)
    {
        context.Compile(expression.Left);
        int jIfFalse = context.EmitEmpty();
        context.Compile(expression.TrueRet);
        int jEnd = context.EmitEmpty();
        context.ResolveEmpty(jIfFalse, BadOpCode.JumpRelativeIfFalse, expression.Position, context.InstructionCount - jIfFalse - 1);
        context.Compile(expression.FalseRet);
        context.ResolveEmpty(jEnd, BadOpCode.JumpRelative, expression.Position, context.InstructionCount - jEnd - 1);
    }
}