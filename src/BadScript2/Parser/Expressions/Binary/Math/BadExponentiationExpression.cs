using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math;

/// <summary>
///     Implements the Exponentiation Expression
/// </summary>
public class BadExponentiationExpression : BadBinaryExpression
{
    /// <summary>
    ///     Creates a new Exponentiation Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">The Source Position</param>
    public BadExponentiationExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();
        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        foreach (BadObject? o in ExpWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }

    /// <summary>
    ///     Runs the Exponentiation Operator on the given objects.
    /// </summary>
    /// <param name="context">The Execution Context</param>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">The Source Position</param>
    /// <returns>Enumerable of which the last element is the result of the operation</returns>
    public static IEnumerable<BadObject> ExpWithOverride(
        BadExecutionContext? context,
        BadObject left,
        BadObject right,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.EXPONENTIATION_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context!,
                         BadStaticKeys.EXPONENTIATION_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Exp(left, right, position);
        }
    }

    /// <summary>
    ///     Implements the logic of the Exponentiation Operator
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">The Source Position</param>
    /// <returns>The Result of the Operation</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the given values are not of type IBadNumber</exception>
    public static BadObject Exp(BadObject left, BadObject right, BadSourcePosition position)
    {
        if (left is not IBadNumber lNum)
        {
            throw new BadRuntimeException($"Can not apply operator '*' to {left} and {right}", position);
        }

        if (right is IBadNumber rNum)
        {
            return BadObject.Wrap(System.Math.Pow((double)lNum.Value, (double)rNum.Value));
        }

        throw new BadRuntimeException($"Can not apply operator '*' to {left} and {right}", position);
    }


    /// <inheritdoc cref="BadBinaryExpression.GetSymbol" />
    protected override string GetSymbol()
    {
        return "**";
    }
}