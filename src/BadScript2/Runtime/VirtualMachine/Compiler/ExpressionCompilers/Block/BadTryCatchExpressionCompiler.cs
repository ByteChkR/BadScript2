using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadTryCatchExpressionCompiler : BadExpressionCompiler<BadTryCatchExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadTryCatchExpression expression)
    {
        List<BadInstruction> instructions = new List<BadInstruction>();
        instructions.Add(
            new BadInstruction(
                BadOpCode.CreateScope,
                expression.Position,
                "TryScope",
                BadObject.Null,
                BadScopeFlags.CaptureThrow
            )
        );
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

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}