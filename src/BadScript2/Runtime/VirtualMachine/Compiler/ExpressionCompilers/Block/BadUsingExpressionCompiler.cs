using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadUsingExpression" />.
/// </summary>
public class BadUsingExpressionCompiler : BadExpressionCompiler<BadUsingExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadUsingExpression expression)
    {
        //Create a new scope for the using block
        //Compile the variable definition
        //Compile the block
        //Emit the call to the Dispose function
        yield return new BadInstruction(
            BadOpCode.CreateScope,
            expression.Position,
            "UsingScope",
            BadObject.Null
        );

        foreach (BadInstruction instruction in compiler.Compile(expression.Definition)) //Compile the Definition
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Expressions)) //Compile the block
        {
            yield return instruction;
        }


        yield return new BadInstruction(BadOpCode.AddDisposeFinalizer, expression.Position, expression.Name); //Add the finalizer
        yield return new BadInstruction(BadOpCode.DestroyScope, expression.Position); //Destroy the scope
    }
}