using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Assign;

/// <summary>
///     Implements the Subtract Assign Expression
/// </summary>
public class BadSubtractAssignExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Subtract Assignment Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadSubtractAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        bool hasReturn = false;
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

        left = left.Dereference();

        BadObject right = BadObject.Null;
        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        if (left is IBadNumber lNum && right is IBadNumber rNum)
        {
            hasReturn = true;

            BadObject r = BadObject.Wrap(lNum.Value - rNum.Value);
            leftRef.Set(r);

            yield return r;
        }
        else if (left.HasProperty(BadStaticKeys.SubtractAssignOperatorName))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context,
                         BadStaticKeys.SubtractAssignOperatorName
                     ))
            {
                yield return o;
            }

            hasReturn = true;
        }

        if (!hasReturn)
        {
            throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left} and {right}", Position);
        }
    }

    protected override string GetSymbol()
    {
        return "-=";
    }
}