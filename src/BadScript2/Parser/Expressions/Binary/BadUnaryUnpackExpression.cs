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


    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression? expression in Right.GetDescendantsAndSelf())
        {
            yield return expression;
        }
    }

    public static void Unpack(BadTable table, BadObject right, BadSourcePosition position)
    {
        if (right is not BadTable rightT)
        {
            throw new BadRuntimeException("Unpack operator requires 1 table", position);
        }

        foreach (KeyValuePair<BadObject, BadObject> o in rightT.InnerTable)
        {
            table.InnerTable[o.Key] = o.Value;
            table.PropertyInfos[o.Key] = rightT.PropertyInfos[o.Key];
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadTable result = context.Scope.GetTable();
        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        Unpack(result, right, Position);

        yield return result;
    }
}