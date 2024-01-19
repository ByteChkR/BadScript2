using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

public class BadUsingExpressionCompiler : BadExpressionCompiler<BadUsingExpression>
{
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
		
        foreach (BadInstruction instruction in compiler.Compile(expression.Expressions)) //Compile the block
        {
            yield return instruction;
        }
		
		
        yield return new BadInstruction(BadOpCode.LoadVar, expression.Position, (BadObject)expression.Name); //Load the variable
        yield return new BadInstruction(BadOpCode.LoadMember, expression.Position, "Dispose"); //Load the Dispose function
        yield return new BadInstruction(BadOpCode.Invoke, expression.Position, 0); //Invoke the Dispose function
		
        yield return new BadInstruction(BadOpCode.DestroyScope, expression.Position); //Destroy the scope
    }
}