using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Reader.Token;

namespace BadScript2.Runtime.Objects.Functions;

public class BadExpressionFunction : BadFunction
{
    private readonly List<BadExpression> m_Body;
    public readonly BadScope ParentScope;

    public readonly BadSourcePosition Position;

    public BadExpressionFunction(
        BadScope parentScope,
        BadWordToken? name,
        List<BadExpression> expressions,
        BadFunctionParameter[] parameters,
        BadSourcePosition position,
        bool isConstant) : base(name, isConstant, parameters)
    {
        m_Body = expressions;
        Position = position;
        ParentScope = parentScope;
    }

    public IEnumerable<BadExpression> Body => m_Body;


    public override BadFunction BindParentScope(BadScope scope)
    {
        return new BadExpressionFunction(scope, Name, m_Body, Parameters, Position, IsConstant);
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        BadExecutionContext ctx = new BadExecutionContext(
            ParentScope.CreateChild(
                $"function {Name}",
                caller.Scope,
                BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
            )
        );
        ApplyParameters(ctx, args, Position);

        foreach (BadObject o in ctx.Execute(Body))
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

    public override string ToString()
    {
        return base.ToString() + " at " + Position.GetPositionInfo();
    }
}