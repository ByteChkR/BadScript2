using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadTryCatchExpression" />.
/// </summary>
public class BadTryCatchExpressionCompiler : BadExpressionCompiler<BadTryCatchExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadTryCatchExpression expression)
    {
        List<BadInstruction> instructions = new List<BadInstruction>
        {
            new BadInstruction(
                BadOpCode.CreateScope,
                expression.Position,
                "TryScope",
                BadObject.Null,
                BadScopeFlags.CaptureThrow
            ),
        };
        int setThrowInstruction = instructions.Count;
        instructions.Add(new BadInstruction());
        instructions.AddRange(compiler.Compile(expression.TryExpressions));
        instructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));
        int jumpToEnd = instructions.Count;
        instructions.Add(new BadInstruction());
        int catchStart = instructions.Count;
        instructions.Add(new BadInstruction(BadOpCode.CreateScope, expression.Position, "CatchScope", BadObject.Null));
        instructions.Add(
            new BadInstruction(
                BadOpCode.DefVar,
                expression.Position,
                (BadObject)expression.ErrorName,
                true
            )
        );
        instructions.Add(new BadInstruction(BadOpCode.Swap, expression.Position));
        instructions.Add(new BadInstruction(BadOpCode.Assign, expression.Position));
        instructions.AddRange(compiler.Compile(expression.CatchExpressions));
        instructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));
        instructions[setThrowInstruction] =
            new BadInstruction(BadOpCode.SetThrowPointer, expression.Position, catchStart - 1);
        instructions[jumpToEnd] =
            new BadInstruction(BadOpCode.JumpRelative, expression.Position, instructions.Count - jumpToEnd - 1);


        //Simply append the instructions for the finally block
        instructions.Add(new BadInstruction(BadOpCode.CreateScope, expression.Position, "FinallyScope", BadObject.Null));
        instructions.AddRange(compiler.Compile(expression.FinallyExpressions));
        instructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}