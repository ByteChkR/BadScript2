using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadWhileExpressionCompiler : BadExpressionCompiler<BadWhileExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadWhileExpression expression)
    {
        List<BadInstruction> instructions = new List<BadInstruction>();

        foreach (BadInstruction instruction in compiler.Compile(expression.Condition))
        {
            instructions.Add(instruction);
        }

        int endJump = instructions.Count;
        instructions.Add(new BadInstruction());

        int loopScopeStart = instructions.Count;

        //Break And continue pointers are relative to the "CreateScope" instruction
        //If break instruction is encountered
        //  Jump to loopScopeStart + offset to after loop(the destroy scope instruction is omitted because the vm handles the scopes)
        instructions.Add(
            new BadInstruction(
                BadOpCode.CreateScope,
                expression.Position,
                "WhileScope",
                BadObject.Null,
                BadScopeFlags.Breakable | BadScopeFlags.Continuable
            )
        );
        int setBreakInstruction = instructions.Count;
        instructions.Add(new BadInstruction());
        int setContinueInstruction = instructions.Count;
        instructions.Add(new BadInstruction());

        foreach (BadInstruction instruction in compiler.Compile(expression.Body))
        {
            instructions.Add(instruction);
        }

        instructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));

        //Jump back up to the loop condition
        int continueJump = instructions.Count;
        instructions.Add(new BadInstruction(BadOpCode.JumpRelative, expression.Position, -instructions.Count - 1));

        //Set the end jump to the end of the loop
        instructions[endJump] = new BadInstruction(
            BadOpCode.JumpRelativeIfFalse,
            expression.Position,
            instructions.Count - endJump
        );

        //address to the end of the loop(relative to the create scope instruction)
        instructions[setBreakInstruction] = new BadInstruction(
            BadOpCode.SetBreakPointer,
            expression.Position,
            instructions.Count - loopScopeStart - 1
        );

        //Address to start of the condition(relative to the start of the loop)
        instructions[setContinueInstruction] = new BadInstruction(
            BadOpCode.SetContinuePointer,
            expression.Position,
            continueJump - loopScopeStart - 1
        );

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}