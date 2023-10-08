using BadScript2.Common;
using BadScript2.Parser;
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

    private readonly string m_FuncString;

    /// <summary>
    ///     The Scope the function is defined in
    /// </summary>
    public readonly BadScope ParentScope;

    /// <summary>
    ///     The Source Position of the Function
    /// </summary>
    public readonly BadSourcePosition Position;

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
        bool isConstant,
        bool isStatic,
        BadMetaData? metaData) : base(name, isConstant, isStatic, parameters)
    {
        m_Body = expressions;
        Position = position;
        ParentScope = parentScope;
        MetaData = metaData ?? BadMetaData.Empty;
        m_FuncString = base.ToString() + " at " + Position.GetPositionInfo();
    }

    /// <summary>
    ///     Enumeration of all expressions in the function body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;


    public override BadMetaData MetaData { get; }

    public override BadFunction BindParentScope(BadScope scope)
    {
        return new BadExpressionFunction(scope, Name, m_Body, Parameters, Position, IsConstant, IsStatic, MetaData);
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        BadExecutionContext ctx = new BadExecutionContext(
            ParentScope.CreateChild(
                ToString(),
                caller.Scope,
                null,
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
        return m_FuncString;
    }
}