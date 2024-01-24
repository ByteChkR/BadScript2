using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block;

/// <summary>
///     Implements the Using Block Expression
/// </summary>
public class BadUsingExpression : BadExpression
{
    /// <summary>
    ///     The Expressions inside the Using Block
    /// </summary>
    private readonly BadExpression[] m_Expressions;

    /// <summary>
    ///     The name of the variable that holds the object
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The definition of the object
    /// </summary>
    private BadExpression m_Definition;

    /// <summary>
    ///     Creates a new Using Expression
    /// </summary>
    /// <param name="name">Name of the variable that holds the object</param>
    /// <param name="expressions">Expressions inside the Using Block</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadUsingExpression(string name, BadExpression[] expressions, BadSourcePosition position, BadExpression definition) : base(false, position)
    {
        Name = name;
        m_Expressions = expressions;
        m_Definition = definition;
    }

    /// <summary>
    ///     Creates a new Using Expression
    /// </summary>
    public IEnumerable<BadExpression> Expressions => m_Expressions;
    
    /// <summary>
    /// The definition of the object
    /// </summary>
    public BadExpression Definition => m_Definition;

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        m_Definition = BadConstantFoldingOptimizer.Optimize(m_Definition);
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
        using BadExecutionContext usingContext = new BadExecutionContext(
            context.Scope.CreateChild("UsingBlock", context.Scope, null)
        );


        foreach (BadObject o in usingContext.Execute(m_Definition))
        {
            yield return o;
        }

        if (m_Expressions.Length != 0)
        {
            foreach (BadObject o in usingContext.Execute(m_Expressions))
            {
                yield return o;
            }
        }

        // ReSharper disable once AccessToDisposedClosure
        usingContext.Scope.AddFinalizer(() => Finalize(usingContext, Name, Position));

        yield return BadObject.Null;
    }

    /// <summary>
    ///     The Finalizer method of the Using Expression
    /// </summary>
    /// <param name="usingContext">The Using Context</param>
    /// <param name="name">The name of the variable that holds the object</param>
    /// <param name="position">The Source Position of the Expression</param>
    /// <exception cref="BadRuntimeException">Gets thrown if the object does not implement IDisposable</exception>
    public static void Finalize(BadExecutionContext usingContext, string name, BadSourcePosition position)
    {
        BadObject obj = usingContext.Scope.GetVariable(name).Dereference();

        if (!obj.HasProperty("Dispose"))
        {
            throw BadRuntimeException.Create(usingContext.Scope, "Object does not implement IDisposable", position);
        }

        BadObject disposeFunc = obj.GetProperty("Dispose", usingContext.Scope).Dereference();
        foreach (BadObject? o in BadInvocationExpression.Invoke(disposeFunc, Array.Empty<BadObject>(), position, usingContext))
        {
            //Do Nothing
        }
    }
}