using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains the Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions;

/// <summary>
///     Implements an expression that Deletes an object pointed to by BadObjectReference
/// </summary>
public class BadDeleteExpression : BadExpression
{
    /// <summary>
    ///     The Key to delete
    /// </summary>
    public readonly BadExpression Expression;

    /// <summary>
    ///     Creates a new Delete Expression
    /// </summary>
    /// <param name="expression">Key Expression</param>
    /// <param name="position">Source Position</param>
    public BadDeleteExpression(BadExpression expression, BadSourcePosition position) : base(false, position)
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

        if (obj is not BadObjectReference r)
        {
            throw BadRuntimeException.Create(context.Scope, $"Cannot delete {Expression}", Position);
        }

        r.Delete();

        yield return BadObject.Null;
    }
}