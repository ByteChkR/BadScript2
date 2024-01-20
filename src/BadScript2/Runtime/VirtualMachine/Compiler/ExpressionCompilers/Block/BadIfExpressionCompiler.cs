using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadIfExpressionCompiler : BadExpressionCompiler<BadIfExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadIfExpression expression)
    {
        List<List<BadInstruction>> branches = new List<List<BadInstruction>>();
        int totalInstructions = 0;

        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in expression.ConditionalBranches)
        {
            List<BadInstruction> branchInstructions = compiler.Compile(branch.Key).ToList();

            //Jump to next branch if false
            int jumpIndex = branchInstructions.Count;
            branchInstructions.Add(new BadInstruction());

            branchInstructions.Add(
                new BadInstruction(
                    BadOpCode.CreateScope,
                    expression.Position,
                    "IfScope",
                    BadObject.Null
                )
            );

            branchInstructions.AddRange(compiler.Compile(branch.Value));

            branchInstructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));

            //Write Relative Jump Expression (-1 for the end jump after the branch has been taken)
            branchInstructions[jumpIndex] = new BadInstruction(
                BadOpCode.JumpRelativeIfFalse,
                expression.Position,
                branchInstructions.Count - jumpIndex
            );
            branches.Add(branchInstructions);
            totalInstructions += branchInstructions.Count + 1;
        }

        List<BadInstruction> elseInstructions = new List<BadInstruction>();

        if (expression.ElseBranch != null)
        {
            elseInstructions.Add(
                new BadInstruction(
                    BadOpCode.CreateScope,
                    expression.Position,
                    "IfScope",
                    BadObject.Null
                )
            );
            totalInstructions++;

            foreach (BadInstruction instruction in compiler.Compile(expression.ElseBranch))
            {
                elseInstructions.Add(instruction);
                totalInstructions++;
            }

            elseInstructions.Add(new BadInstruction(BadOpCode.DestroyScope, expression.Position));
            totalInstructions++;
        }

        int currentInstruction = 0;

        foreach (List<BadInstruction> instructions in branches)
        {
            currentInstruction += instructions.Count + 1;
            instructions.Add(
                new BadInstruction(
                    BadOpCode.JumpRelative,
                    expression.Position,
                    totalInstructions - currentInstruction + 1
                )
            );
        }

        foreach (BadInstruction instruction in branches.SelectMany(instructions => instructions))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in elseInstructions)
        {
            yield return instruction;
        }
    }
}