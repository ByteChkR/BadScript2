using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Assign;

/// <summary>
///     Implements the Exponentiation Assign Expression
/// </summary>
public class BadExponentiationAssignExpression : BadBinaryExpression
{
	/// <summary>
	///     Creates a new Exponentiation Assign Expression
	/// </summary>
	/// <param name="left">Left side of the Expression</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">The Source Position</param>
	public BadExponentiationAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) :
        base(left, right, position) { }

	/// <summary>
	///     Implements the logic of the Exponentiation Operator
	/// </summary>
	/// <param name="leftRef">Left Reference</param>
	/// <param name="left">Left Value</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">The Source Position</param>
	/// <param name="symbol">The Expression Symbol</param>
	/// <returns>The Result of the Operation</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the given values are not of type IBadNumber</exception>
	public static BadObject Exp(
        BadObjectReference leftRef,
        BadObject left,
        BadObject right,
        BadSourcePosition position,
        string symbol)
    {
        if (left is not IBadNumber lNum || right is not IBadNumber rNum)
        {
            throw new BadRuntimeException($"Can not apply operator '{symbol}' to {left} and {right}", position);
        }

        BadObject r = BadObject.Wrap(System.Math.Pow((double)lNum.Value, (double)rNum.Value));
        leftRef.Set(r);

        return r;
    }

	/// <summary>
	///     Runs the Exponentiation Operator on the given objects.
	/// </summary>
	/// <param name="context">The Execution Context</param>
	/// <param name="leftRef">Left Reference</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">The Source Position</param>
	/// <param name="symbol">The Expression Symbol</param>
	/// <returns>Enumerable of which the last element is the result of the operation</returns>
	public static IEnumerable<BadObject> ExpWithOverride(
        BadExecutionContext? context,
        BadObjectReference leftRef,
        BadObject right,
        BadSourcePosition position,
        string symbol)
    {
        BadObject left = leftRef.Dereference();

        if (left.HasProperty(BadStaticKeys.EXPONENTIATION_ASSIGN_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context!,
                         BadStaticKeys.MULTIPLY_ASSIGN_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Exp(leftRef, left, right, position, symbol);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        if (left is not BadObjectReference leftRef)
        {
            throw new BadRuntimeException($"Left side of {GetSymbol()} must be a reference", Position);
        }

        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        foreach (BadObject o in ExpWithOverride(context, leftRef, right, Position, GetSymbol()))
        {
            yield return o;
        }
    }

    protected override string GetSymbol()
    {
        return "**=";
    }
}