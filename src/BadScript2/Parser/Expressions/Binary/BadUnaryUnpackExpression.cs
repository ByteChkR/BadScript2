using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary;

/// <summary>
///     Implements the unary ... operator. This operator is used to unpack a table into the current execution scope.
/// </summary>
public class BadUnaryUnpackExpression : BadExpression
{
    /// <summary>
    ///     The Right Side of the Expression
    /// </summary>
    public readonly BadExpression Right;

    /// <summary>
    ///     Constructor for the unary ... operator
    /// </summary>
    /// <param name="right">Right Side of the Expression</param>
    public BadUnaryUnpackExpression(BadExpression right) : base(right.IsConstant, right.Position)
    {
        Right = right;
    }


    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Right.GetDescendantsAndSelf();
    }

    /// <summary>
    ///     Implements the logic of the unary ... operator.
    /// </summary>
    /// <param name="table">The table to unpack into</param>
    /// <param name="right">The table to unpack</param>
    /// <param name="position">The Source Position</param>
    /// <exception cref="BadRuntimeException">Gets thrown if the right side is not a table</exception>
    public static void Unpack(BadTable table, BadObject right, BadSourcePosition position)
    {
        if (right is not BadTable rightT)
        {
            throw new BadRuntimeException("Unpack operator requires 1 table", position);
        }

        foreach (KeyValuePair<string, BadObject> o in rightT.InnerTable)
        {
            table.InnerTable[o.Key] = o.Value;
            table.PropertyInfos[o.Key] = rightT.PropertyInfos[o.Key];
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadTable result = context.Scope.GetTable();
        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference(Position);

        Unpack(result, right, Position);

        yield return result;
    }
}