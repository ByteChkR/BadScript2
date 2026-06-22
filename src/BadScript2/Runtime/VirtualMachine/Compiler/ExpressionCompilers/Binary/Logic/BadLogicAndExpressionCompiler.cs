using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains Binary Logic Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
///     Compiles the <see cref="BadLogicAndExpression" />.
/// </summary>
public class BadLogicAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAndExpression>
{
    protected override bool EmitLeft => false;
    protected override bool EmitRight => false;

    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLogicAndExpression expression)
    {
        context.Compile(expression.Left);
        //If Top of the stack is false, then we don't need to run the right side of the expression and can directly jump to the end of the expression,
        var falseJump = context.EmitEmpty();
        context.Compile(expression.Right);
        var trueJump = context.EmitEmpty();
        
        var falseLocation = context.InstructionCount;
        context.Emit(BadOpCode.Push, expression.Position, BadObject.False);
        var endJump = context.EmitEmpty();
        
        var trueLocation = context.InstructionCount;
        context.Emit(BadOpCode.Push, expression.Position, BadObject.True);
        
        var endLocation = context.InstructionCount;

        //Jump to the false location if left is false
        context.ResolveEmpty(falseJump, BadOpCode.JumpRelativeIfFalse, expression.Position, falseLocation - falseJump - 1);
        //Jump to the end of the expression if left is true
        context.ResolveEmpty(trueJump, BadOpCode.JumpRelativeIfTrue, expression.Position, trueLocation - trueJump - 1);
        //Jump to the end of the expression
        context.ResolveEmpty(endJump, BadOpCode.JumpRelative, expression.Position, endLocation - endJump - 1);
    }
}