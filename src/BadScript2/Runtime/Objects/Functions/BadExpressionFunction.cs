using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains Runtime Function Objects
/// </summary>
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
    public readonly BadScope ParentScope;

    /// <summary>
    ///     The Source Position of the Function
    /// </summary>
    public readonly BadSourcePosition Position;

    /// <summary>
    ///     The Function String
    /// </summary>
    private string? m_FuncString;

    /// <summary>
    ///     Creates a new Expression Function
    /// </summary>
    /// <param name="parentScope">The Scope the function is defined in</param>
    /// <param name="name">The (optional) function name</param>
    /// <param name="expressions">The Function Body</param>
    /// <param name="parameters">The parameter info</param>
    /// <param name="position">The Source Position of the Function</param>
    /// <param name="isConstant">Indicates if the function has no side effects and the result can be cached</param>
    /// <param name="isStatic">Is the Function Static</param>
    /// <param name="metaData">The Metadata of the Function</param>
    /// <param name="returnType">The Return type of the Function</param>
    /// <param name="isSingleLine">Indicates if the function is a single line function</param>
    public BadExpressionFunction(
        BadScope parentScope,
        BadWordToken? name,
        List<BadExpression> expressions,
        BadFunctionParameter[] parameters,
        BadSourcePosition position,
        bool isConstant,
        bool isStatic,
        BadMetaData? metaData,
        BadClassPrototype returnType,
        bool isSingleLine) : base(name, isConstant, isStatic, returnType, isSingleLine, parameters)
    {
        m_Body = expressions;
        Position = position;
        ParentScope = parentScope;
        MetaData = metaData ?? BadMetaData.Empty;
    }

    /// <summary>
    ///     Enumeration of all expressions in the function body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;


    /// <inheritdoc />
    public override BadMetaData MetaData { get; }

    /// <inheritdoc />
    public override BadFunction BindParentScope(BadScope scope)
    {
        return new BadExpressionFunction(
            scope,
            Name,
            m_Body,
            Parameters,
            Position,
            IsConstant,
            IsStatic,
            MetaData,
            ReturnType,
            IsSingleLine
        );
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        using BadExecutionContext ctx = new BadExecutionContext(
            ParentScope.CreateChild(
                ToString(),
                caller.Scope,
                null,
                BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
            )
        );

        ctx.Scope.FunctionObject = this;

        ApplyParameters(ctx, args, Position);

        if (m_Body.Count == 0)
        {
            yield return Null;

            yield break;
        }

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
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (m_FuncString == null)
        {
            m_FuncString = base.ToString() + " at " + Position.GetPositionInfo();
        }
        return m_FuncString;
    }
}