using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block;

/// <summary>
/// Implements the Using Statement Expression
/// </summary>
public class BadUsingExpression : BadExpression
{
    /// <summary>
    /// The Expressions inside the Using Block
    /// </summary>
    private readonly BadExpression[] m_Expressions;
    /// <summary>
    /// The name of the variable that holds the object
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Creates a new Using Expression
    /// </summary>
    /// <param name="name">Name of the variable that holds the object</param>
    /// <param name="expressions">Expressions inside the Using Block</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadUsingExpression(string name, BadExpression[] expressions, BadSourcePosition position) : base(false, position)
    {
        Name = name;
        m_Expressions = expressions;
    }

    /// <summary>
    /// Creates a new Using Expression
    /// </summary>
    public IEnumerable<BadExpression> Expressions => m_Expressions;

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        for (int i = 0; i < m_Expressions.Length; i++)
        {
            m_Expressions[i] = BadConstantFoldingOptimizer.Optimize(m_Expressions[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return m_Expressions.SelectMany(expr => expr.GetDescendantsAndSelf());
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadExecutionContext usingContext = new BadExecutionContext(
            context.Scope.CreateChild("UsingBlock", context.Scope, null)
        );


        foreach (BadObject o in usingContext.Execute(m_Expressions))
        {
            yield return o;
        }

        BadObject obj = usingContext.Scope.GetVariable(Name).Dereference();

        if (!obj.HasProperty("Dispose"))
        {
            throw BadRuntimeException.Create(usingContext.Scope, "Object does not implement IDisposable", Position);
        }

        BadObject disposeFunc = obj.GetProperty("Dispose", usingContext.Scope).Dereference();
        foreach (BadObject? o in BadInvocationExpression.Invoke(disposeFunc, Array.Empty<BadObject>(), Position, usingContext))
        {
            yield return o;
        }

        yield return BadObject.Null;
    }
}