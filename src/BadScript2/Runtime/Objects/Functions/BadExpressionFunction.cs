using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Reader.Token;

namespace BadScript2.Runtime.Objects.Functions;

/// <summary>
///     The Expression Function Implementation used if a function gets defined in the Source Code.
/// </summary>
public class BadExpressionFunction : BadFunction
{
    /// <summary>
    ///     The Function Body
    /// </summary>
    private readonly List<BadExpression> m_Body;

    /// <summary>
    ///     The Scope the function is defined in
    /// </summary>
    private readonly BadScope m_ParentScope;

    /// <summary>
    ///     The Source Position of the Function
    /// </summary>
    private readonly BadSourcePosition m_Position;

    /// <summary>
    ///     Creates a new Expression Function
    /// </summary>
    /// <param name="parentScope">The Scope the function is defined in</param>
    /// <param name="name">The (optional) function name</param>
    /// <param name="expressions">The Function Body</param>
    /// <param name="parameters">The parameter info</param>
    /// <param name="position">The Source Position of the Function</param>
    /// <param name="isConstant">Indicates if the function has no side effects and the result can be cached</param>
    public BadExpressionFunction(
        BadScope parentScope,
        BadWordToken? name,
        List<BadExpression> expressions,
        BadFunctionParameter[] parameters,
        BadSourcePosition position,
        bool isConstant) : base(name, isConstant, parameters)
    {
        m_Body = expressions;
        m_Position = position;
        m_ParentScope = parentScope;
    }

    /// <summary>
    ///     Enumeration of all expressions in the function body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;


    public override BadFunction BindParentScope(BadScope scope)
    {
        return new BadExpressionFunction(scope, Name, m_Body, Parameters, m_Position, IsConstant);
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        BadExecutionContext ctx = new BadExecutionContext(
            m_ParentScope.CreateChild(
                ToString(),
                caller.Scope, null,
                BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
            )
        );
        ApplyParameters(ctx, args, m_Position);

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
        return base.ToString() + " at " + m_Position.GetPositionInfo();
    }
}