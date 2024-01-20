using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;
/// <summary>
/// Contains Block Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadForEachExpressionCompiler : BadExpressionCompiler<BadForEachExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadForEachExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Target))
        {
            yield return instruction;
        }

        //DUPLICATE TARGET
        yield return new BadInstruction(BadOpCode.Dup, expression.Position);

        //Find out if GetEnumerator is implemented
        yield return new BadInstruction(BadOpCode.Push, expression.Position, (BadObject)"GetEnumerator");
        yield return new BadInstruction(BadOpCode.HasProperty, expression.Position);

        //Call GetEnumerator on target if it exists
        yield return new BadInstruction(BadOpCode.JumpRelativeIfFalse, expression.Position, 2);
        yield return new BadInstruction(BadOpCode.LoadMember, expression.Position, "GetEnumerator");
        yield return new BadInstruction(BadOpCode.Invoke, expression.Position, 0);

        //Create TempScope for the target
        yield return new BadInstruction(
            BadOpCode.CreateScope,
            expression.Position,
            "FOREACH_ENUMERATOR_SCOPE",
            BadObject.Null
        );

        //Write target into variable
        yield return new BadInstruction(BadOpCode.DefVar, expression.Position, (BadObject)"~ENUMERATOR~", true);
        yield return new BadInstruction(BadOpCode.Swap, expression.Position);
        yield return new BadInstruction(BadOpCode.Assign, expression.Position);


        IEnumerable<BadExpression> loopBody = expression.Body;
        loopBody = loopBody.Prepend(
            new BadAssignExpression(
                new BadVariableDefinitionExpression(
                    expression.LoopVariable.Text,
                    expression.LoopVariable.SourcePosition,
                    null,
                    true
                ),
                new BadInvocationExpression(
                    new BadMemberAccessExpression(
                        new BadVariableExpression("~ENUMERATOR~", expression.Position),
                        "GetCurrent",
                        expression.Position
                    ),
                    Array.Empty<BadExpression>(),
                    expression.Position
                ),
                expression.Position
            )
        );


        //Create While Expression
        BadWhileExpression whileExpr = new BadWhileExpression(
            new BadInvocationExpression(
                new BadMemberAccessExpression(
                    new BadVariableExpression("~ENUMERATOR~", expression.LoopVariable.SourcePosition),
                    "MoveNext",
                    expression.LoopVariable.SourcePosition
                ),
                Array.Empty<BadExpression>(),
                expression.Position
            ),
            loopBody.ToList(),
            expression.Position
        );

        //Console.WriteLine(whileExpr.ToString());

        foreach (BadInstruction instruction in compiler.Compile(whileExpr))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.DestroyScope, expression.Position);

        //  while(<target>.MoveNext())
        //  {
        //      const <LoopVariable> = <target>.GetCurrent();
        //      <Body>
        //  }
    }
}