using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary;

/// <summary>
///     Implements the binary ... operator. This operator is used to unpack the right side into the left side overwriting
///     existing properties.
/// </summary>
public class BadBinaryUnpackExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor for the binary ... operator
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">The Source Position</param>
    public BadBinaryUnpackExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }


    public static BadTable Unpack(BadObject left, BadObject right, BadSourcePosition position)
    {
        BadTable result = new BadTable();

        if (left is not BadTable leftT || right is not BadTable rightT)
        {
            throw new BadRuntimeException("Unpack operator requires 2 tables", position);
        }

        foreach (KeyValuePair<BadObject, BadObject> o in leftT.InnerTable)
        {
            result.InnerTable[o.Key] = o.Value;
            result.PropertyInfos[o.Key] = leftT.PropertyInfos[o.Key];
        }

        foreach (KeyValuePair<BadObject, BadObject> o in rightT.InnerTable)
        {
            result.InnerTable[o.Key] = o.Value;
            result.PropertyInfos[o.Key] = rightT.PropertyInfos[o.Key];
        }

        return result;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        BadObject right = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        left = left.Dereference();
        right = right.Dereference();

        yield return Unpack(left, right, Position);
    }

    protected override string GetSymbol()
    {
        return "...";
    }
}