using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
namespace BadScript2.Parser.Expressions.Binary;

/// <summary>
///     Implements the 'in' operator. The 'in' operator is used to check if a key is present in an instance of a type.
/// </summary>
public class BadInExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor for the 'in' operator
    /// </summary>
    /// <param name="left">Left Side of the Expression</param>
    /// <param name="right">Right Side of the Expression</param>
    /// <param name="position">The Source Position</param>
    public BadInExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        BadObject right = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        foreach (BadObject? o in InWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }

    /// <summary>
    ///     Implements the logic of the 'in' operator.
    /// </summary>
    /// <param name="ctx">The Calling Context</param>
    /// <param name="left">Property Key</param>
    /// <param name="right">Instance</param>
    /// <returns>Returns true if the left side is a property of the right side</returns>
    public static BadObject In(BadExecutionContext ctx, BadObject left, BadObject right)
    {
        if (left is not IBadString s)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Invalid Property Key");
        }

        return right.HasProperty(s.Value, ctx.Scope);
    }

    /// <summary>
    ///     Implements the logic of the 'in' operator but checks for an operator override first.
    /// </summary>
    /// <param name="context">The Execution Context</param>
    /// <param name="left">Property Key</param>
    /// <param name="right">Instance</param>
    /// <param name="position">The Source Position</param>
    /// <returns>Returns true if the left side is a property of the right side</returns>
    public static IEnumerable<BadObject> InWithOverride(
        BadExecutionContext context,
        BadObject left,
        BadObject right,
        BadSourcePosition position)
    {
        if (right.HasProperty(BadStaticKeys.IN_OPERATOR_NAME, context.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         right,
                         left,
                         context,
                         BadStaticKeys.IN_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return In(context, left, right);
        }
    }

    /// <inheritdoc cref="BadBinaryExpression.GetSymbol" />
    protected override string GetSymbol()
    {
        return "in";
    }
}