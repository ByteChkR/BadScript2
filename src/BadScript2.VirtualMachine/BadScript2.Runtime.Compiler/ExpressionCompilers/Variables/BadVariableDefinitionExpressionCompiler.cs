using System.Collections.Generic;

using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Variables;

public class BadVariableDefinitionExpressionCompiler : BadExpressionCompiler<BadVariableDefinitionExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadVariableDefinitionExpression expression)
    {
        if (expression.TypeExpression == null)
        {
            yield return new BadInstruction(BadOpCode.DefVar, expression.Position, (BadObject)expression.Name, expression.IsReadOnly);
        }
        else
        {
            foreach (BadInstruction instruction in compiler.Compile(expression.TypeExpression))
            {
                yield return instruction;
            }

            yield return new BadInstruction(BadOpCode.DefVarTyped, expression.Position, (BadObject)expression.Name, expression.IsReadOnly);
        }
    }
}