using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions;

/// <summary>
///     Implements the 'typeof' operator.
/// </summary>
public class BadTypeOfExpression : BadExpression
{
    /// <summary>
    ///     The Expression to get the type of
    /// </summary>
    public readonly BadExpression Expression;

    /// <summary>
    ///     Creates a new 'typeof' expression
    /// </summary>
    /// <param name="expression">Expression to get the type of</param>
    /// <param name="position">Source Position</param>
    public BadTypeOfExpression(BadExpression expression, BadSourcePosition position) : base(false, position)
    {
        Expression = expression;
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Expression.GetDescendantsAndSelf();
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject? obj = BadObject.Null;

        foreach (BadObject o in Expression.Execute(context))
        {
            obj = o;

            yield return o;
        }

        obj = obj.Dereference(Position);

        yield return obj.GetPrototype();
    }
}