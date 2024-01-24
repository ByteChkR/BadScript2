using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block;

/// <summary>
///     Implements the Using Statement Expression
/// </summary>
public class BadUsingStatementExpression : BadExpression
{
    /// <summary>
    ///     The Expression defining the object
    /// </summary>
    private readonly BadExpression m_Expression;
    
    public BadExpression Expression => m_Expression;

    /// <summary>
    ///     The name of the variable that holds the object
    /// </summary>
    public readonly string Name;

    /// <summary>
    ///     Creates a new Using Statement Expression
    /// </summary>
    /// <param name="name">Name of the variable that holds the object</param>
    /// <param name="expression">The Expression defining the object</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadUsingStatementExpression(BadSourcePosition position, string name, BadExpression expression) : base(false, position)
    {
        Name = name;
        m_Expression = expression;
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return m_Expression.GetDescendantsAndSelf();
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        //Run expression
        foreach (BadObject o in context.Execute(m_Expression))
        {
            yield return o;
        }

        //Register finalizer in scope
        context.Scope.AddFinalizer(() => BadUsingExpression.Finalize(context, Name, Position));
    }
}