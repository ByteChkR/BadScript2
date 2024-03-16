using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

public class BadSwitchExpressionCompiler: BadExpressionCompiler<BadSwitchExpression>
{
    public override void Compile(BadExpressionCompileContext context, BadSwitchExpression expression)
    {
        //1. Compile Value
        context.Compile(expression.Value);
        //2. Create a scope for the switch statement
        int switchScopeStart = context.InstructionCount;
        context.Emit(BadOpCode.CreateScope, expression.Position, "SwitchScope", BadObject.Null, BadScopeFlags.Breakable);
        int setBreakInstruction = context.EmitEmpty();
        //3. Compile the cases
        // a. Compile the case value
        // b. Compare the case value with the switch value
        // c. If the case value is equal to the switch value, jump to the next case body
        // d. If the case value is not equal to the switch value, jump to the next case
        // e. If no case value is equal to the switch value, jump to the end of the switch statement
        List<int> caseBodyJumps = new List<int>(); //List of jumps to the next case body
        List<int> caseEndJumps = new List<int>(); //List of jumps to the end of the switch statement(after the default case)
        foreach (KeyValuePair<BadExpression, BadExpression[]> @case in expression.Cases)
        {
            context.Emit(BadOpCode.Dup, expression.Position); //Duplicate the switch value
            context.Compile(@case.Key); //compile the case value
            context.Emit(BadOpCode.Equals, expression.Position);
            if (@case.Value.Length > 0)
            {
                int endJump = context.EmitEmpty(); //Jump to the next case if the case value is not equal to the switch value
                
                //We have a case body,
                //Resolve all caseBodyJumps to the start of the case body
                foreach (int caseBodyJump in caseBodyJumps)
                {
                    context.ResolveEmpty(caseBodyJump, BadOpCode.JumpRelativeIfTrue, expression.Position, context.InstructionCount - caseBodyJump - 1);
                }
                caseBodyJumps.Clear();
                context.Compile(@case.Value, false);
                
                //Resolve the endJump to the end of the case body
                context.ResolveEmpty(endJump, BadOpCode.JumpRelativeIfFalse, expression.Position, context.InstructionCount - endJump);
                caseEndJumps.Add(context.EmitEmpty());
            }
            else
            {
                //We dont have a case body
                //If the case value is equal to the switch value, jump to the next case body
                caseBodyJumps.Add(context.EmitEmpty());
            }
        }
        //Resolve all caseBodyJumps to the start of the default case
        foreach (int caseBodyJump in caseBodyJumps)
        {
            context.ResolveEmpty(caseBodyJump, BadOpCode.JumpRelativeIfTrue, expression.Position, context.InstructionCount - caseBodyJump - 1);
        }
        caseBodyJumps.Clear();
        if (expression.DefaultCase != null)
        {
            context.Compile(expression.DefaultCase, false);
        }
        //Resolve all caseEndJumps to the end of the switch statement
        foreach (int caseEndJump in caseEndJumps)
        {
            context.ResolveEmpty(caseEndJump, BadOpCode.JumpRelative, expression.Position, context.InstructionCount - caseEndJump - 1);
        }
        context.ResolveEmpty(setBreakInstruction, BadOpCode.SetBreakPointer, expression.Position, context.InstructionCount - switchScopeStart);
        context.Emit(BadOpCode.DestroyScope, expression.Position);
    }
}