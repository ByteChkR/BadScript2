using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadForExpressionCompiler : BadExpressionCompiler<BadForExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadForExpression expression)
    {
        yield return new BadInstruction(BadOpCode.CreateScope, expression.Position, "ForLoop", BadObject.Null);

        foreach (BadInstruction instruction in compiler.Compile(expression.VarDef))
        {
            yield return instruction;
        }

        List<BadInstruction> instructions = compiler.Compile(expression.Condition).ToList();

        //jump to end if false
        int endJump = instructions.Count;
        instructions.Add(new BadInstruction());

        //Create Scope
        int loopScopeStart = instructions.Count;
        instructions.Add(
            new BadInstruction(
                BadOpCode.CreateScope,
                expression.Position,
                "ForLoopBody",
                BadObject.Null,
                BadScopeFlags.Breakable | BadScopeFlags.Continuable
            )
        );
        int setBreakInstruction = instructions.Count;
        instructions.Add(new BadInstruction());
        int setContinueInstruction = instructions.Count;
        instructions.Add(new BadInstruction());

        //Loop
        instructions.AddRange(compiler.Compile(expression.Body));

        //DestroyScope
        instructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));

        //jump to condition
        int continueJump = instructions.Count;

        //Increment
        instructions.AddRange(compiler.Compile(expression.VarIncrement));


        instructions.Add(new BadInstruction(BadOpCode.JumpRelative, expression.Position, -instructions.Count - 1));

        //set end jump
        int endJumpPos = instructions.Count - endJump - 1;
        instructions[endJump] = new BadInstruction(BadOpCode.JumpRelativeIfFalse, expression.Position, endJumpPos);


        //DestroyScope
        instructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));

        //Set Break/Continue Pointer
        //address to the end of the loop(relative to the create scope instruction)
        instructions[setBreakInstruction] = new BadInstruction(
            BadOpCode.SetBreakPointer,
            expression.Position,
            instructions.Count - loopScopeStart
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