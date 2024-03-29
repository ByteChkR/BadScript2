using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

/// <summary>
///     Implements the Return expression that is used to exit the current function with an Optional Return Value.
/// </summary>
public class BadReturnExpression : BadExpression
{
    /// <summary>
    ///     Constructor of the Return Expression
    /// </summary>
    /// <param name="right">The (optional) return value</param>
    /// <param name="position">Source Position of the Expression</param>
    /// <param name="isRefReturn">Indicates if the return value is meant to be a reference</param>
    public BadReturnExpression(BadExpression? right, BadSourcePosition position, bool isRefReturn) : base(
        false,
        position
    )
    {
        Right = right;
        IsRefReturn = isRefReturn;
    }

    /// <summary>
    ///     Indicates if the return value is meant to be a reference
    /// </summary>
    public bool IsRefReturn { get; }

    /// <summary>
    ///     The (optional) return value
    /// </summary>
    public BadExpression? Right { get; private set; }

    /// <summary>
    ///     Sets the return value
    /// </summary>
    /// <param name="expr">The return value</param>
    public void SetRight(BadExpression? expr)
    {
        Right = expr;
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        if (Right == null)
        {
            yield break;
        }

        foreach (BadExpression right in Right.GetDescendantsAndSelf())
        {
            yield return right;
        }
    }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        if (Right != null)
        {
            Right = BadConstantFoldingOptimizer.Optimize(Right);
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject value = BadObject.Null;

        if (Right == null)
        {
            context.Scope.SetReturnValue(value);

            yield return value;

            yield break;
        }

        foreach (BadObject obj in Right.Execute(context))
        {
            value = obj;

            yield return obj;
        }

        if (!IsRefReturn)
        {
            value = value.Dereference();
        }

        context.Scope.SetReturnValue(value);

        yield return value;
    }

    /// <inheritdoc cref="BadExpression.ToString" />
    public override string ToString()
    {
        return "return " + (Right?.ToString() ?? "");
    }
}