using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
namespace BadScript2.Parser.Expressions.Binary.Math;

/// <summary>
///     Implements the Negation Expression
/// </summary>
public class BadNegationExpression : BadExpression
{
    /// <summary>
    ///     Creates a new Negation Expression
    /// </summary>
    /// <param name="position">Position of the Expression</param>
    /// <param name="expression">The Expression to negate</param>
    public BadNegationExpression(BadSourcePosition position, BadExpression expression) : base(
        expression.IsConstant,
        position
    )
    {
        Expression = expression;
    }

    /// <summary>
    ///     The Expression to negate
    /// </summary>
    public BadExpression Expression { get; }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Expression.GetDescendantsAndSelf();
    }

    /// <summary>
    ///     Executes the Operator
    /// </summary>
    /// <param name="context">The Execution Context</param>
    /// <param name="left">Subexpression to negate</param>
    /// <param name="position">Position of the expression</param>
    /// <returns>Returns the result of the operation</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be applied</exception>
    public static IEnumerable<BadObject> NegateWithOverride(
        BadExecutionContext? context,
        BadObject left,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.MULTIPLY_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         left,
                         context!,
                         BadStaticKeys.MULTIPLY_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Negate(left, position);
        }
    }

    /// <summary>
    ///     Executes the Operator
    /// </summary>
    /// <param name="obj">Subexpression to negate</param>
    /// <param name="pos">Position of the expression</param>
    /// <returns>Returns the result of the operation</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be applied</exception>
    public static BadObject Negate(BadObject obj, BadSourcePosition pos)
    {
        if (obj is IBadNumber num)
        {
            return BadObject.Wrap(-num.Value);
        }

        throw new BadRuntimeException($"Can not apply operator '-' to {obj}", pos);
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Expression.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        foreach (BadObject o in NegateWithOverride(context, left, Position))
        {
            yield return o;
        }
    }


    /// <inheritdoc cref="BadExpression.ToString" />
    public override string ToString()
    {
        return $"-{Expression}";
    }
}