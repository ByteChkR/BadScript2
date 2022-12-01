using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Logic.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Logic.Assign;

public class BadLogicAssignOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignOrExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicAssignOrExpression expression)
    {
        List<BadInstruction> instructions = new List<BadInstruction>();
        instructions.AddRange(compiler.Compile(expression.Left));
        instructions.Add(new BadInstruction(BadOpCode.Dup, expression.Position));
        int jump = instructions.Count;
        instructions.Add(new BadInstruction());
        instructions.AddRange(compiler.Compile(expression.Right));
        instructions.Add(new BadInstruction(BadOpCode.Assign, expression.Position));
        instructions[jump] = new BadInstruction(BadOpCode.JumpRelativeIfTrue, expression.Position, instructions.Count - jump - 1);
        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}