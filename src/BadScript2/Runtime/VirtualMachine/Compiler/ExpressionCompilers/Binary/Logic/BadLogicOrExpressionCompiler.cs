using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
///     Compiles the <see cref="BadLogicOrExpression" />.
/// </summary>
public class BadLogicOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicOrExpression>
{
    /// <inheritdoc />
    protected override bool EmitLeft => false;

    /// <inheritdoc />
    protected override bool EmitRight => false;

    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLogicOrExpression expression)
    {
        context.Compile(expression.Left);
        var trueJump = context.EmitEmpty();
        context.Compile(expression.Right);
        var falseJump = context.EmitEmpty();
        
        var trueLocation = context.InstructionCount;
        context.Emit(BadOpCode.Push, expression.Position, BadObject.True);
        var endJump = context.EmitEmpty();
        
        var falseLocation = context.InstructionCount;
        context.Emit(BadOpCode.Push, expression.Position, BadObject.False);
        
        var endLocation = context.InstructionCount;
        
        //Jump to the true location if left is true
        context.ResolveEmpty(trueJump, BadOpCode.JumpRelativeIfTrue, expression.Position, trueLocation - trueJump - 1);
        //Jump to the false location if left is false
        context.ResolveEmpty(falseJump, BadOpCode.JumpRelativeIfFalse, expression.Position, falseLocation - falseJump - 1);
        //Jump to the end of the expression
        context.ResolveEmpty(endJump, BadOpCode.JumpRelative, expression.Position, endLocation - endJump - 1);
    }
}