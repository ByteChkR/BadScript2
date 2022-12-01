using System.Collections.Generic;

using BadScript2.Common;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.VirtualMachine;

public class BadCompiledFunction : BadFunction
{
    private readonly BadInstruction[] m_Instructions;
    private readonly BadScope m_ParentScope;
    private readonly BadSourcePosition m_Position;
    private readonly bool m_UseOverrides;

    public BadCompiledFunction(
        BadInstruction[] instructions,
        bool useOverrides,
        BadScope parentScope,
        BadSourcePosition position,
        BadWordToken? name,
        bool isConstant,
        params BadFunctionParameter[] parameters) : base(
        name,
        isConstant,
        parameters
    )
    {
        m_Instructions = instructions;
        m_UseOverrides = useOverrides;
        m_ParentScope = parentScope;
        m_Position = position;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        BadExecutionContext ctx = new BadExecutionContext(
            m_ParentScope.CreateChild(
                ToString(),
                caller.Scope,
                null,
                BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
            )
        );
        ApplyParameters(ctx, args, m_Position);
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(m_Instructions, m_UseOverrides);
        foreach (BadObject o in vm.Execute(ctx))
        {
            yield return o;
        }

        if (ctx.Scope.ReturnValue != null)
        {
            yield return ctx.Scope.ReturnValue;
        }
        else
        {
            yield return Null;
        }

        if (ctx.Scope.Error != null)
        {
            caller.Scope.SetErrorObject(ctx.Scope.Error);
        }
    }
}